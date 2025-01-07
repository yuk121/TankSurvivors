using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : CreatureController
{
    // Monster Info
    Define.eCreatureAnimState _creaturState = Define.eCreatureAnimState.None;
    public Define.eCreatureAnimState CreatureState
    {
        get { return _creaturState; }
        set { _creaturState = value; }
    }
    Transform _trans = null;
    Rigidbody _rb = null;


    // Monster Info
    readonly float CHECK_DELAY = 0.5f;
    float _checkTime = 0f;
    float _viewAngle = 60;

    // Player Info
    PlayerController _target = null;
    Transform _targetTrans = null;

    float _detectDistance = 1f;

    // Attack Idle
    float _lastAttackTime;
    float _idleCoolTime = 1f;

    bool _isTargetNear;
    bool _skillWait = false;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        _objectType = Define.eObjectType.Enemy;
        _trans = transform;
        _rb = GetComponent<Rigidbody>();

        // ��ų
        _skillBook.SetSkillBook(CreatureData.skillList);

        InChase();

        return true;
    }

    public override void FixedUpdateController()
    {
        base.FixedUpdateController();

        switch (_creaturState)
        {
            case Define.eCreatureAnimState.Chase:
                ModifyChase();
                break;
        }
    }

    // ���� ���¿� ���缭 �޼ҵ� ����
    public override void UpdateController()
    {
        base.UpdateController();

        switch (_creaturState)
        {
            case Define.eCreatureAnimState.Skill:
                ModifySkill();
                break;
            case Define.eCreatureAnimState.AttackIdle:
                ModifyAttackIdle();
                break;
        }
    }

    #region Chase
    private void InChase()
    {
        _creaturState = Define.eCreatureAnimState.Chase;
        _animController.Play(Define.eCreatureAnimState.Chase);
        _checkTime = Time.time + CHECK_DELAY;
    }

    private void ModifyChase()
    {
        _target = Managers.Instance.ObjectManager.Player;
        _targetTrans = _target.transform;

        if (_target == null)
            return;

        // ���� �ð����� Ÿ���� ��ó�� �ִ��� Ȯ��
        if (Time.time > _checkTime)
        {
           _isTargetNear = CheckPlayerNear();
            _checkTime = Time.time + CHECK_DELAY;
        }

        if (_isTargetNear)
        {
            // ��ų ���
            OutChase();
            InSkill();

            return;
        }
        else
        {
            // ����
            Vector3 dir = _target.transform.position - _trans.position;
            dir.y = 0;
      
            Vector3 monPos = transform.position + dir.normalized * Time.deltaTime * _creatureData.moveSpeed;

            _rb.MovePosition(monPos);
            _trans.forward = dir;
        }

    }

    private void OutChase()
    {

    }
    #endregion

    #region Skill
    private void InSkill()
    {
        _skillWait = true;
        _creaturState = Define.eCreatureAnimState.Skill;

        // ����� ��ų ��� ������

    }

    private void ModifySkill()
    {
        if (_skillWait == false)
        {
            OutSkill();
            InAttackIdle();

            return;
        }
    }

    private void OutSkill()
    {

    }

    public void AnimEvent_Skill()
    {
        bool isNear = CheckPlayerNear();

        if (isNear) // ��ų ���� ����
        {
            // ������
        }

        _skillWait = false;
    }
    #endregion

    #region AttackIdle
    private void InAttackIdle()
    {
        _creaturState = Define.eCreatureAnimState.AttackIdle;
        _animController.Play(Define.eCreatureAnimState.AttackIdle);

        _lastAttackTime = Time.time;
    }

    private void ModifyAttackIdle()
    {
        // �Ϲ� ���� ������ üũ
        if (Time.time > _lastAttackTime + _idleCoolTime)
        {
            OutAttackIdle();
            InChase();

            return;
        }
    }

    private void OutAttackIdle()
    {

    }
    #endregion

    // �÷��̾ ������ �ִ��� Ž���ϴ� �޼ҵ�
    private bool CheckPlayerNear()
    {
        int checkCount = 0;
        // �Ÿ� üũ

        Debug.DrawRay(_trans.position + Vector3.up, _trans.forward *_detectDistance, Color.red);
        RaycastHit rayHit = new RaycastHit();
        bool isNear = Physics.Raycast(_trans.position + Vector3.up, _trans.forward, out rayHit, _detectDistance, 1 << LayerMask.NameToLayer("Player"));

        if(isNear)
          checkCount++;

        isNear = false;

        // ���� üũ
        Vector3 targetDir = (_targetTrans.position - _trans.position).normalized;
        Vector3 nowViewAngle = _trans.forward;
        float detectAngle = Vector3.Angle(nowViewAngle, targetDir);

        if (detectAngle <= _viewAngle)
            checkCount++;

        // �ʿ�� ��ֹ� üũ
      
        return checkCount > 1;
    }
}
