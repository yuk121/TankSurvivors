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
    public Define.eMonsterGrade Grade { get => _grade; }

    Transform _trans = null;
    Rigidbody _rb = null;


    // Monster Info
    readonly float CHECK_DELAY = 0.5f;
    float _checkTime = 0f;
    bool _isChaseInit = true;

    // Player Info
    PlayerController _target = null;
    Transform _targetTrans = null;

    // Attack Idle
    float _lastAttackTime;

    bool _isTargetNear;
    bool _skillWait = false;

    // Damaged Color
    private SkinnedMeshRenderer[] _meshRenderers;
    private MaterialPropertyBlock _materialProperty;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = Define.eObjectType.Enemy;
        _trans = transform;
        _rb = GetComponent<Rigidbody>();
        GetComponent<Collider>().enabled = true;

        // renderer
        _meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        _materialProperty = new MaterialPropertyBlock();

        _isAlive = true;

        // ��ų���� 1������ ����
        for(int i = 0; i < _skillBook.ActionSkillList.Count; i++)
        {
            _skillBook.UpgradeSkill(_skillBook.ActionSkillList[i].SkillData.skillId);
        }
        
        // ���� ��� ����
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

    // ���� ���¿� ���缭 �޼ҵ� ����
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

        // �������¸� �������� �ʴ´�.
        if (_isAlive == false)
            return;

        if (_target == null || _target.CheckAlive() == false)
            return;

        // ù ������ ��� �Ǵ� ���� �ð����� Ÿ���� ��ó�� �ִ��� Ȯ��
        if (_isChaseInit == true || Time.time > _checkTime)
        {
            if(_isChaseInit == true)
            {
                _isChaseInit = false;
            }

            // ��Ÿ���� ���� ��ų�� �ߵ� ���� �ȿ� �����ϴ��� Ȯ��
            ActionSkill skill = _skillBook.GetCoolTimeEndSkill();

            // ��ų �ߵ� �Ÿ� üũ
            _isTargetNear = CheckSkillRangePlayer(skill.SkillData.detectRange);
            _checkTime = Time.time + CHECK_DELAY;
        }

        // ��ų �ߵ� ������ �÷��̾ �ִ� ���
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
        _state = Define.eMonsterFSMState.Skill;

        //��ų ����߿��� �ε����� ���� ��ų ���� ��Ÿ�� üũ �� ���
        ActionSkill skill = _skillBook.GetCoolTimeEndSkill();

        if (skill == null)
        {
            Debug.LogError("### Monster Skill is CoolTime");
            OutSkill();
            InAttackIdle();
            return;
        }

        skill.UseSkill(this);
        string skillAnimState = $"Skill{skill.Index}"; 
        _animController.Play(skillAnimState);

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
        // ��ų ���� �� ��ų ���� ���� �ȿ� �ִ��� Ȯ��
        bool isNear = CheckSkillRangePlayer(_skillBook.PrevUseSkill.SkillData.attackRange);

        if (isNear) // ��ų ���� ����
        {
            // ������
            int stageLevel = GameManager.Instance.GameData.stageInfo.stageLevel;
            float damageSkill = _skillBook.PrevUseSkill.SkillData.damage;
            float damageSkillIncRate = _skillBook.PrevUseSkill.SkillData.damageIncRate;
            float damageFinal = (float)(stageLevel * damageSkillIncRate) + damageSkill;

            // TODO : ���� �߰�

            _target.OnDamaged(this , damageFinal);
        }
    }

    public void AnimEvent_SkillEnd()
    {
        _skillBook.PrevUseSkill.StopAllCoroutines();
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
        // �Ϲ� ���� ������ üũ
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

    public override void OnDamaged(BaseController attacker, float damage)
    {
        base.OnDamaged(attacker, damage);

        // Render
        SetDamagedColor();
        Invoke("SetDefaultColor", 0.2f);
    }

    public override void OnDead()
    {
        // TODO : �״� �Ҹ� �߰�
        GetComponent<Collider>().enabled = false;
        _isAlive = false;
        // �״� �ִϸ��̼� ��� 
        _animController.Play(Define.eCreatureAnimState.Dead, true);
    }

    public void AnimeEvent_DeadEnd()
    {
        DropItem();

        // ���� ��ü ī��Ʈ ����
        GameManager.Instance.GameData.killCount++;

        // ���� ��ü�� Ǯ�� �ٽ� �־��ش�.
        Managers.Instance.ObjectManager.Despawn(this);
    }

    private void DropItem()
    {
        // ����ġ �� �Ǵ� �������� �����ش�.
        DropItemData dropItemData = GameManager.Instance.GetRandomDropItem(_grade);

        if (dropItemData == null)
            return;

        switch(dropItemData.dropItemType)
        {
            case Define.eObjectType.Gem:
                Managers.Instance.ObjectManager.Spawn<DropItemGem>(_trans.position, dropItemData.dropItemId);
                break;

            case Define.eObjectType.Bomb:
                Managers.Instance.ObjectManager.Spawn<DropItemBomb>(_trans.position, dropItemData.dropItemId);
                break;

            case Define.eObjectType.Magnet:
                Managers.Instance.ObjectManager.Spawn<DropItemMagnet>(_trans.position, dropItemData.dropItemId);
                break;

            case Define.eObjectType.HpRecorvery:
                Managers.Instance.ObjectManager.Spawn<DropItemHeart>(_trans.position, dropItemData.dropItemId);
                break;

            case Define.eObjectType.Box:
                Managers.Instance.ObjectManager.Spawn<DropItemBox>(_trans.position, dropItemData.dropItemId);
                break;
        }
    }

    // ��ų ���� �ȿ� �÷��̾ �����ϴ��� Ȯ���ϱ�
    private bool CheckSkillRangePlayer(float range)
    {
        Debug.DrawRay(_trans.position + (Vector3.up * 0.5f), _trans.forward * range, Color.red);
        RaycastHit rayHit = new RaycastHit();
        bool isNear = Physics.Raycast(_trans.position + (Vector3.up * 0.5f), _trans.forward, out rayHit, range, 1 << LayerMask.NameToLayer("Player"));

        if (isNear == false)
            return false;

        // ���� üũ
        Vector3 targetDir = (_targetTrans.position - _trans.position).normalized;
        Vector3 nowViewAngle = _trans.forward;
        float detectAngle = Vector3.Angle(nowViewAngle, targetDir);

        // �������� ������ ��� ������ �����ϴ� ���
        if (detectAngle > Define.ENEMY_VIEW_ANGLE)
            return false;

        // �ʿ�� ��ֹ� üũ
        
        return true;
    }

    void SetDamagedColor()
    {
        // �ǰݽ� ��� ������ �Ǵ� ȿ��
        if (_materialProperty != null)
        {
            foreach (Renderer render in _meshRenderers)
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
