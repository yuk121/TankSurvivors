using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CreatureController
{
    [SerializeField]
    private Transform _tankBody = null;
    [SerializeField]
    private Transform _tankHead = null;
    [SerializeField]
    private Transform _dummyFirePos = null;
    public Transform DummyFirePos { get => _dummyFirePos; }

    private Vector2 _moveDir = Vector2.zero;
    public Vector2 MoveDir { get { return _moveDir; } set { _moveDir = value; } }
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

        // �⺻ ������ �̸� 1������ ����
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

        // �̵�
        Vector3 newPos = new Vector3(transform.position.x + dir.x, transform.position.y, transform.position.z + dir.y);
        _tankRigid.MovePosition(newPos);

        // ȸ��
        if(moveDir != Vector3.zero)
            _tankBody.rotation = Quaternion.Euler(0, Mathf.Atan2(moveDir.x, moveDir.y) * Mathf.Rad2Deg, 0);
    }

    public override void OnDead()
    {
        _isAlive = false;
        gameObject.SetActive(false);
    }

    // �÷��̾��� ��ų ��Ͽ� ���缭 �ڵ����� ��ų ���
    private void UseAutoSkill()
    {
        for (int i = 0; i < _skillBook.SkillList.Count; i++)
        {
            // ��ų ������ 0�̶�� ���
            if (_skillBook.SkillList[i].CurSkillLevel < 1)
                continue;

            // ��ų ��Ÿ�� Ȯ���ϱ�
            if (_skillBook.SkillList[i].RemainCoolTime > 0)
                continue;

            if(_skillBook.SkillList[i].SkillType == Define.eSkillType.TankShell)
            {
                // �⺻������ ��ź ��ų�� ��� ��ũ�� �Ӹ��� ������ ����Ѵ�.
                // ���� ��ó�� �� ��ġ Ž��
                MonsterController mc = GetNearEnemy();
                // ������ ���� ���� �Ӹ����� ����
                _tankHead.LookAt(mc.transform);
            }

            // ��ų ���
            _skillBook.SkillList[i].UseSkill();
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
                curDis = minDis;
                nearEnemy = mc;
            }
        }

        return nearEnemy;
    }

    // �ֺ��� �ִ� �ڿ� �ݱ�
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
        // �ǰݽ� ��� ������ �Ǵ� ȿ��
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
        // ���� �������� ���ƿ�
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
