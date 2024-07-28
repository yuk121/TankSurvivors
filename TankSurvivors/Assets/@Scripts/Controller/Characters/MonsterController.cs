using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : CreatureController
{
    // Monster Info
    Define.eCreatureAnimState _creaturState = Define.eCreatureAnimState.None;
    Transform _trans = null;
    Rigidbody _rb = null;

    // Player Info
    PlayerController _target = null;
    Transform _targetTrans = null;

    public Define.eCreatureAnimState CreatureState
    {
        get { return _creaturState; }
        set { _creaturState = value; }
    }

    public override bool Init()
    {
        if(base.Init() == false) 
            return false;
       
        // 초기화
        _animController.Play(Define.eCreatureAnimState.Walk);
        _creaturState = Define.eCreatureAnimState.Walk;
        _objectType = eObjectType.Enemy;
        _trans = transform;
        _rb = GetComponent<Rigidbody>();

        return true;
    }

    // 몬스터 상태에 맞춰서 메소드 실행
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
    protected virtual void UpdateAttack() { }
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
    }
}
