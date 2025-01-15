using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CreatureController
{
    [SerializeField]
    private Transform _tankBody = null;
    [SerializeField]
    private Transform _dummyFirePos = null;

    private Vector2 _moveDir = Vector2.zero;
    public Vector2 MoveDir { get { return _moveDir; } set { _moveDir = value; } }
    private Vector3 _turretDir = Vector3.zero;
    private Rigidbody _tankRigid = null;

    // Damaged Color
    private MeshRenderer[] _meshRenderers;
    private MaterialPropertyBlock _materialProperty;

    public override bool Init()
    {
        if(base.Init() == false)
            return false;
            
        _tankRigid = GetComponent<Rigidbody>();

        // renderer
        _meshRenderers = GetComponentsInChildren<MeshRenderer>();
        _materialProperty = new MaterialPropertyBlock();

        // 기본 공격은 미리 1레벨로 설정
        _skillBook.UpgradeSkill(_skillBook.SkillList[0].SkillData.skillId);

        return true;
    }

    public override void FixedUpdateController()
    {
        base.FixedUpdateController();
        PlayerMove();
    }

    public override void UpdateController()
    {
        base.UpdateController();
        UseAutoSkill();
        CollectEnv();
    }

    private void PlayerMove()
    {
        Vector3 moveDir = Managers.Instance.ObjectManager.Player.MoveDir;
        Vector3 dir = moveDir * Time.deltaTime * _creatureData.moveSpeed;

        // 이동
        Vector3 newPos = new Vector3(transform.position.x + dir.x, transform.position.y, transform.position.z + dir.y);
        _tankRigid.MovePosition(newPos);

        // 회전
        if(moveDir != Vector3.zero)
            _tankBody.rotation = Quaternion.Euler(0, Mathf.Atan2(moveDir.x, moveDir.y) * Mathf.Rad2Deg, 0);
    }

    public override void OnDead()
    {
        _isAlive = false;
        gameObject.SetActive(false);
    }

    // 플레이어의 스킬 목록에 맞춰서 자동으로 스킬 사용
    private void UseAutoSkill()
    {
        for(int i = 0; i < _skillBook.SkillList.Count; i++)
        {
            // 스킬 레벨이 0이라면 통과
            if (_skillBook.SkillList[i].CurSkillLevel < 1)
                continue;

            // 스킬 쿨타임 확인하기
            if (_skillBook.SkillList[i].RemainCoolTime > 0)
                continue;

            // 스킬 사용했으니 쿨타임 채워지도록 함
            _skillBook.SkillList[i].StartCoolTime();

            // 프리팹을 이용해서 오브젝트 생성
            string prefabName = _skillBook.SkillList[i].SkillData.prefabName;
            string skillPrefabPath = $"PlayerPrefab/{prefabName}.prefab";
            GameObject go = Managers.Instance.ResourceManager.Instantiate(skillPrefabPath, pooling: true);
            go.name = _skillBook.SkillList[i].SkillData.prefabName;

            // 스킬에 맞는 컴포넌트 붙여주기
            Define.eSkillType skillType = (Define.eSkillType)_skillBook.SkillList[i].SkillData.skillId;

            switch (skillType)
            {
                case Define.eSkillType.TankShell:
                    {
                        go.transform.position = _dummyFirePos.position;
                        go.transform.forward = _dummyFirePos.forward;

                        go.AddComponent<TankShell>();
                        TankShell tankShell = go.GetComponent<TankShell>();
                        tankShell.Init(_skillBook.SkillList[i].SkillData, null);
                    }
                    break;

                case Define.eSkillType.SubTank:
                    break;

                case Define.eSkillType.ElectircField:
                    break;

                case Define.eSkillType.Mine:
                    break;

            }
        }
    }
    // 주변에 있는 자원 줍기
    private void CollectEnv()
    {

    }

    public override void OnDamaged(BaseController attacker, float damage)
    {
        base.OnDamaged(attacker, damage);

        SetDamagedColor();
        Invoke("SetDefaultColor", 0.2f);
    }

    void SetDamagedColor()
    {
        // 피격시 잠깐 빨갛게 되는 효과
        if (_materialProperty != null)
        {
            foreach(Renderer render in _meshRenderers)
            {
                _materialProperty.SetColor("_Color", Color.red);
                render.SetPropertyBlock(_materialProperty);
            }
        }
    }

    void SetDefaultColor()
    {
        // 원래 색상으로 돌아옴
        if (_materialProperty != null)
        {
            foreach (Renderer render in _meshRenderers)
            {
                _materialProperty.SetColor("_Color", Color.white);
                render.SetPropertyBlock(_materialProperty);
            }
        }
    }
}
