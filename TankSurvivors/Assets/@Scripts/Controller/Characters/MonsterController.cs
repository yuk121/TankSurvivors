using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : CreatureController
{
    // Monster Info
    [SerializeField]
    Transform _rayPos = null;
    Define.eCreatureAnimState _creaturState = Define.eCreatureAnimState.None;
    Transform _trans = null;
    Rigidbody _rb = null;

    // Player Info
    PlayerController _target = null;
    Transform _targetTrans = null;

    // Attack
    float _detectDistance = 1f;
    float _attackCoolTime = 5f;
    float _curTime = 0f;

    bool _firstDetect = true;


    public Define.eCreatureAnimState CreatureState
    {
        get { return _creaturState; }
        set { _creaturState = value; }
    }

    public override bool Init()
    {
        if(base.Init() == false) 
            return false;
       
        // �ʱ�ȭ
        _animController.Play(Define.eCreatureAnimState.Walk);
        _creaturState = Define.eCreatureAnimState.Walk;
        _objectType = eObjectType.Enemy;
        _trans = transform;
        _rb = GetComponent<Rigidbody>();

        return true;
    }

    // ���� ���¿� ���缭 �޼ҵ� ����
    public override void UpdateController()
    {
        base.UpdateController();

        switch(_creaturState)
        {
            case Define.eCreatureAnimState.Idle:
                UpdateIdle();
                break;
            case Define.eCreatureAnimState.Walk:
                UpdateWalk();
                break;
            case Define.eCreatureAnimState.Dead:
                UpdateDead();
                break;
            case Define.eCreatureAnimState.Attack:
                UpdateAttack();
                break;
            case Define.eCreatureAnimState.Skill_01:
                UpdateSkill_01();
                break;
        }
    }

    protected virtual void UpdateIdle() { }
    protected virtual void UpdateWalk() { }
    protected virtual void UpdateDead() { }
    protected virtual void UpdateAttack()
    {
        _curTime += Time.deltaTime;

        // ���� �ð����� ����
        if (_curTime >= _attackCoolTime)
        {
            CheckPlayerNear();
            _curTime = 0;
        }
    }
    protected virtual void UpdateSkill_01() { }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (_creaturState != Define.eCreatureAnimState.Walk)
            return;

        _target = Managers.Instance.ObjectManager.Player;
        _targetTrans = _target.transform;

        if (_target == null) 
            return;

        Vector3 dir = _target.transform.position - _trans.position;
        Vector3 monPos = transform.position + dir.normalized * Time.deltaTime * _creatureData.MoveSpeed;

        _rb.MovePosition(monPos);
        _trans.LookAt(_targetTrans);
      
        CheckPlayerNear();
    }

    // �÷��̾ ������ �ִ��� Ž���ϴ� �޼ҵ�
    private void CheckPlayerNear()
    {
        RaycastHit rayHit = new RaycastHit();
        bool isNear = Physics.Raycast(_rayPos.position, _trans.forward, out rayHit, _detectDistance, 1 << LayerMask.NameToLayer("Player"));

        Debug.DrawRay(_rayPos.position, _trans.forward, Color.red);

        // ���� ���� : ���� �� ���� ��� ���� -> �з��� ������ Ǯ�� -> �ȱ� ��� ���� -> ������ ������ �� -> ���ݸ�� ���� (�ݺ� �� �ִϸ��̼� �̻��ϰ� ����)

        if (isNear)
        {
            // ������ �ִ� ��� Walk -> Attack���� ����
            _creaturState = Define.eCreatureAnimState.Attack;
            _animController.Play(Define.eCreatureAnimState.Attack);
        }
        else
        {
            // ������ �ִٰ� �־����� ��� Attack -> Walk�� ����
            if (_creaturState == Define.eCreatureAnimState.Attack)
            {
                _creaturState = Define.eCreatureAnimState.Walk;
                _animController.Play(Define.eCreatureAnimState.Walk);

                _curTime = 0;
            }
        }
    }

    // ���� �ִϸ��̼� �÷��� �� ��ϵ� �̺�Ʈ
    public void AnimEvent_Attack()
    {
        // ���� ����� ������ Ÿ�ֿ̹� �ٽ� �ѹ� �Ÿ� üũ
        RaycastHit rayHit = new RaycastHit();
        bool isNear = Physics.Raycast(_rayPos.position, _trans.forward, out rayHit, _detectDistance, 1 << LayerMask.NameToLayer("Player"));

        if(isNear) // ���� ���� ����
        {

        }
        else // ���� ���� ����
        {

        }
    }
}
