using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CreatureController
{
    private const float ENV_COLLECT_DISTANCE_BASIC = 1.5f;

    [Header("BASE")]
    [SerializeField]
    private Transform _tankBody = null;
    [SerializeField]
    private Transform _tankHead = null;
    [SerializeField]
    private Transform _dummyFirePos = null;
    public Transform DummyFirePos { get => _dummyFirePos; }

    [Header("UI")]
    [SerializeField]
    private Transform _dummyIndicatorPos = null;
    public Transform DummyIndicatorPos { get => _dummyIndicatorPos; }
    [SerializeField]
    private UI_PlayerHpBar _playerHpBar = null;

    private Vector2 _moveDir = Vector2.zero;
    public Vector2 MoveDir { get { return _moveDir; } set { _moveDir = value; } }
    private Rigidbody _tankRigid = null;

    // Damaged Color
    private MeshRenderer[] _meshRenderers;
    private MaterialPropertyBlock _materialProperty;

    // Stat
    public int Exp { get; set; } = 0;
    public int Level { get; set; } = 1;

    public override bool Init()
    {
        if(base.Init() == false)
            return false;

        ObjectType = Define.eObjectType.Player;
        _tankRigid = GetComponent<Rigidbody>();

        // UI
        _playerHpBar.SetPlayer(this);

        // renderer
        _meshRenderers = GetComponentsInChildren<MeshRenderer>();
        _materialProperty = new MaterialPropertyBlock();

        // 기본 공격은 미리 1레벨로 설정
        _skillBook.UpgradeSkill(_skillBook.SkillList[0].SkillData.skillId);
        
        // Stat 초기화
        Exp = 0;
        Level = 1;

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
    
        // TODO : 움직이는 소리 추가
    }

    public override void OnDead()
    {
        // TODO : 터지는 효과 추가
        _isAlive = false;
        gameObject.SetActive(false);
    }

    // 플레이어의 스킬 목록에 맞춰서 자동으로 스킬 사용
    private void UseAutoSkill()
    {
        for (int i = 0; i < _skillBook.SkillList.Count; i++)
        {
            // 스킬 레벨이 0이라면 통과
            if (_skillBook.SkillList[i].CurSkillLevel < 1)
                continue;

            // 스킬 쿨타임 확인하기
            if (_skillBook.SkillList[i].RemainCoolTime > 0)
                continue;

            if(_skillBook.SkillList[i].SkillType == Define.eSkillType.TankShell)
            {
                // 기본공격인 포탄 스킬인 경우 탱크의 머리도 움직여 줘야한다.
                // 가장 근처의 적 위치 탐색
                MonsterController mon = GetNearEnemy();

                if (mon != null && mon.CheckAlive() == true)
                {
                    // 감지된 적을 향해 머리방향 변경
                    Vector3 dir = mon.transform.position - _tankHead.position;
                    dir.y = 0;
                    _tankHead.transform.forward = dir;
                }
            }

            // 스킬 사용
            _skillBook.SkillList[i].UseSkill(this);
        }
    }

    private MonsterController GetNearEnemy()
    {
        MonsterController nearEnemy = null;

        float minDis = float.MaxValue;
        float curDis = 0f;

        foreach(MonsterController mc in Managers.Instance.ObjectManager.Monsters)
        {
            curDis = Vector3.Distance(_tankBody.position, mc.transform.position);
            
            if(curDis < minDis)
            {
                minDis = curDis;
                nearEnemy = mc;
            }
        }

        return nearEnemy;
    }

    // 주변에 있는 자원 줍기
    private void CollectEnv()
    {
        List<DropItemController> findDropItems = GridManager.Instance.GatherDropObjects(transform.position, ENV_COLLECT_DISTANCE_BASIC + 0.5f);
        float collectSqr = ENV_COLLECT_DISTANCE_BASIC * ENV_COLLECT_DISTANCE_BASIC;

        foreach(DropItemController item in findDropItems)
        {
            Vector3 dir = item.transform.position - transform.position;
       
            if(dir.sqrMagnitude <= collectSqr)
            {
                switch(item.ItemType)
                {
                    case Define.eObjectType.Bomb:
                    case Define.eObjectType.HpRecorvery:
                    case Define.eObjectType.Magnet:
                    case Define.eObjectType.Box:
                        item.GetItem();
                        break;

                    default:
                        item.GetItem();
                        break;
                }
            }
        }

    }

    public override void OnDamaged(BaseController attacker, float damage)
    {
        // TODO : 맞는 소리 추가

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
