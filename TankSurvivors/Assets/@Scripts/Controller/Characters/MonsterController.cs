using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : CreatureController
{
    private const float ATTACKIDlE_INTERVAL = 0.5f;
    // Monster Info
    Define.eMonsterFSMState _state = Define.eMonsterFSMState.None;
    public Define.eMonsterFSMState State
    {
        get { return _state; }
        set { _state = value; }
    }
    Transform _trans = null;
    Rigidbody _rb = null;


    // Monster Info
    readonly float CHECK_DELAY = 0.5f;
    float _checkTime = 0f;
    float _viewAngle = 60;
    bool _isChaseInit = true;

    // Player Info
    PlayerController _target = null;
    Transform _targetTrans = null;

    float _detectDistance = 1f;

    // Attack Idle
    float _lastAttackTime;

    bool _isTargetNear;
    bool _skillWait = false;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        _objectType = Define.eObjectType.Enemy;
        _trans = transform;
        _rb = GetComponent<Rigidbody>();

        InChase();

        return true;
    }

    public override void FixedUpdateController()
    {
        base.FixedUpdateController();

        switch (_state)
        {
            case Define.eMonsterFSMState.Chase:
                ModifyChase();
                break;
        }
    }

    // 몬스터 상태에 맞춰서 메소드 실행
    public override void UpdateController()
    {
        base.UpdateController();

        switch (_state)
        {
            case Define.eMonsterFSMState.Skill:
                ModifySkill();
                break;
            case Define.eMonsterFSMState.AttackIdle:
                ModifyAttackIdle();
                break;
            case Define.eMonsterFSMState.Pause:

                break;
        }
    }

    #region Chase
    private void InChase()
    {
        _isTargetNear = false;
        _isChaseInit = true;
        
        _state = Define.eMonsterFSMState.Chase;
        _animController.Play(Define.eCreatureAnimState.Chase);
        _checkTime = Time.time + CHECK_DELAY;
    }

    private void ModifyChase()
    {
        _target = Managers.Instance.ObjectManager.Player;
        _targetTrans = _target.transform;

        if (_target == null || _target.CheckAlive() == false)
        {
            // TODO : Puase 상태로 보내기
            return;
        }

        // 첫 추적인 경우 또는 일정 시간마다 타겟이 근처에 있는지 확인
        if (_isChaseInit == true || Time.time > _checkTime)
        {
            if(_isChaseInit == true)
            {
                _isChaseInit = false;
            }    
         
            _isTargetNear = CheckPlayerNear();
            _checkTime = Time.time + CHECK_DELAY;
        }

        // 타겟이 근처에 있는 경우
        if (_isTargetNear)
        {
            // 스킬 사용
            OutChase();
            InSkill();

            return;
        }
        else
        {
            // 추적
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
        _state = Define.eMonsterFSMState.Skill;

        //스킬 목록중에서 인덱스가 높은 스킬 부터 쿨타임 체크 후 사용
        SkillBase skill = GetCoolTimeEndSkill();

        if (skill == null)
        {
            Debug.LogError("### Monster Skill is Null !! ");
            return;
        }

        string skillAnimState = $"Skill{skill.Index}"; 
        _animController.Play(skillAnimState);

        // 스킬 쿨타임 설정
        skill.StartCoolTime();
        _skillBook.PrevUseSkill = skill;
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

    public void AnimEvent_SkillDamage()
    {
        bool isNear = CheckPlayerNear();

        if (isNear) // 스킬 판정 성공
        {
            // 데미지
            // TODO : 스킬 데미지 산정 방식 필요
            int stageLevel = GameManager.Instance.GameData.stageInfo.stageLevel;
            float damage = (float)(stageLevel * 0.1) + _skillBook.PrevUseSkill.SkillData.damage;

            // TODO : 사운드 추가

            _target.OnDamaged(this ,damage);
        }
    }

    public void AnimEvent_SkillEnd()
    {
        _skillWait = false;
    }
    #endregion

    #region AttackIdle
    private void InAttackIdle()
    {
        _state = Define.eMonsterFSMState.AttackIdle;
        _animController.Play(Define.eCreatureAnimState.AttackIdle,true);

        _lastAttackTime = Time.time;
    }

    private void ModifyAttackIdle()
    {
        // 일반 공격 딜레이 체크
        if (Time.time > _lastAttackTime + ATTACKIDlE_INTERVAL)
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

    #region Pause
    private void InPause()
    {
        _state = Define.eMonsterFSMState.AttackIdle;
        _animController.Play(Define.eCreatureAnimState.AttackIdle, true);

        _lastAttackTime = Time.time;
    }

    private void ModifyPause()
    {
        // 일반 공격 딜레이 체크

    }

    private void OutPause()
    {

    }
    #endregion

    // 플레이어가 주위에 있는지 탐색하는 메소드
    private bool CheckPlayerNear()
    {
        int checkCount = 0;
        // 거리 체크

        //Debug.DrawRay(_trans.position + Vector3.up, _trans.forward *_detectDistance, Color.red);
        RaycastHit rayHit = new RaycastHit();
        bool isNear = Physics.Raycast(_trans.position + Vector3.up, _trans.forward, out rayHit, _detectDistance, 1 << LayerMask.NameToLayer("Player"));

        if(isNear)
          checkCount++;

        isNear = false;

        // 각도 체크
        Vector3 targetDir = (_targetTrans.position - _trans.position).normalized;
        Vector3 nowViewAngle = _trans.forward;
        float detectAngle = Vector3.Angle(nowViewAngle, targetDir);

        if (detectAngle <= _viewAngle)
            checkCount++;

        // 필요시 장애물 체크
      
        return checkCount > 1;
    }
}
