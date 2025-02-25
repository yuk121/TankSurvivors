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

    Define.eMonsterFSMState _prevState = Define.eMonsterFSMState.None;

    Define.eMonsterGrade _grade = Define.eMonsterGrade.Normal;

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

        ObjectType = Define.eObjectType.Enemy;
        _trans = transform;
        _rb = GetComponent<Rigidbody>();
        _isAlive = true;

        // 스킬들을 1레벨로 설정
        for(int i = 0; i < _skillBook.ActionSkillList.Count; i++)
        {
            _skillBook.UpgradeSkill(_skillBook.ActionSkillList[i].SkillData.skillId);
        }
        
        // 몬스터 등급 결정
        if(gameObject.name.Contains("Normal"))
        {
            _grade = Define.eMonsterGrade.Normal;
        }
        else if(gameObject.name.Contains("Elite"))
        {
            _grade = Define.eMonsterGrade.Elite;
        }
        else
        {
            _grade = Define.eMonsterGrade.Boss;
        }

        InChase();

        return true;
    }

    public override void FixedUpdateController()
    {
        base.FixedUpdateController();

        if (_isAlive == false)
            return;

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

        if (_isAlive == false)
            return;

        switch (_state)
        {
            case Define.eMonsterFSMState.Skill:
                ModifySkill();
                break;
            case Define.eMonsterFSMState.AttackIdle:
                ModifyAttackIdle();
                break;
            case Define.eMonsterFSMState.Pause:
                ModifyPause();
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

        // 죽은상태면 움직이지 않는다.
        if (_isAlive == false)
            return;

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
        ActionSkill skill = _skillBook.GetCoolTimeEndSkill();

        if (skill == null)
        {
            Debug.LogError("### Monster Skill is CoolTime !! ");
            OutSkill();
            InAttackIdle();
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
        _prevState = _state;

        if(_prevState == Define.eMonsterFSMState.Skill)
        {
            _animController.Pause();
        }

        _state = Define.eMonsterFSMState.Pause;    
        _lastAttackTime = Time.time;
    }

    private void ModifyPause()
    {
        if(GameManager.Instance.Pause == false)
        {
            OutPause();
        }
    }

    private void OutPause()
    {
        switch(_prevState)
        {
            case Define.eMonsterFSMState.Chase:
                InChase();
                break;
            case Define.eMonsterFSMState.Skill:
                _animController.Resume();
                break;
        }

        _state = _prevState;
    }

    public void BranchInPause()
    {
        InPause();
    }
    #endregion

    public override void OnDead()
    {
        // TODO : 죽는 소리 추가
        _isAlive = false;
        // 죽는 애니메이션 재생 
        _animController.Play(Define.eCreatureAnimState.Dead, true);
    }

    public void AnimeEvent_DeadEnd()
    {
        DropItem();

        // 죽은 개체 카운트 증가
        GameManager.Instance.GameData.killCount++;

        // 죽은 개체는 풀에 다시 넣어준다.
        Managers.Instance.ObjectManager.Despawn(this);
    }

    private void DropItem()
    {
        // 경험치 젬 또는 아이템을 떨궈준다.
        // Test용 타입 
        Define.eObjectType dropItemType = Define.eObjectType.Gem;

        // TODO : 드랍 아이템을 무엇으로 줄지 결정해줘야함
        DropItemData dropItemData = GameManager.Instance.GetRandomDropItem(_grade);

        // TODO : dropItemType => dropItemData.dropItemType으로 바꿀것
        switch(dropItemType)
        {
            case Define.eObjectType.Gem:
                // implementID => dropItemData.dropItemId로 바꿀것
                Managers.Instance.ObjectManager.Spawn<DropItemGem>(_trans.position, 50001);
                break;

            case Define.eObjectType.Bomb:
                break;

            case Define.eObjectType.Magnet:
                break;

            case Define.eObjectType.HpRecorvery:
                break;

            case Define.eObjectType.Box:
                break;
        }
    }

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
