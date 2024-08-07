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
    protected virtual void UpdateAttack()
    {
        _curTime += Time.deltaTime;

        // 일정 시간마다 공격
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

    // 플레이어가 주위에 있는지 탐색하는 메소드
    private void CheckPlayerNear()
    {
        RaycastHit rayHit = new RaycastHit();
        bool isNear = Physics.Raycast(_rayPos.position, _trans.forward, out rayHit, _detectDistance, 1 << LayerMask.NameToLayer("Player"));

        Debug.DrawRay(_rayPos.position, _trans.forward, Color.red);

        // 현재 문제 : 감지 후 공격 모션 시작 -> 밀려서 감지가 풀림 -> 걷기 모션 시작 -> 빠르게 감지가 됨 -> 공격모션 시작 (반복 시 애니메이션 이상하게 보임)

        if (isNear)
        {
            // 가까이 있는 경우 Walk -> Attack으로 변경
            _creaturState = Define.eCreatureAnimState.Attack;
            _animController.Play(Define.eCreatureAnimState.Attack);
        }
        else
        {
            // 가까이 있다가 멀어지는 경우 Attack -> Walk로 변경
            if (_creaturState == Define.eCreatureAnimState.Attack)
            {
                _creaturState = Define.eCreatureAnimState.Walk;
                _animController.Play(Define.eCreatureAnimState.Walk);

                _curTime = 0;
            }
        }
    }

    // 공격 애니메이션 플레이 시 등록된 이벤트
    public void AnimEvent_Attack()
    {
        // 공격 모션이 적절한 타이밍에 다시 한번 거리 체크
        RaycastHit rayHit = new RaycastHit();
        bool isNear = Physics.Raycast(_rayPos.position, _trans.forward, out rayHit, _detectDistance, 1 << LayerMask.NameToLayer("Player"));

        if(isNear) // 공격 판정 성공
        {

        }
        else // 공격 판정 실패
        {

        }
    }
}
