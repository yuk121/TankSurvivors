using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CreatureController : BaseController
{
    protected CreatureData _creatureData;
    public CreatureData CreatureData
    {
        get => _creatureData;
        set => _creatureData = value;
    }

    protected SkillBook _skillBook;
    protected AnimationController _animController;
  
    [SerializeField]
    protected float _curHp;
    public float CurHp { get { return _curHp; } }
    
    protected bool _isAlive = true;
    public bool IsAlive { get => _isAlive; }

    public override bool Init()
    {
        base.Init();

        _animController = Utils.GetOrAddComponent<AnimationController>(gameObject);

        _skillBook = Utils.GetOrAddComponent<SkillBook>(gameObject);        
        // 보유 스킬 데이터 
        _skillBook.SetActionSkill(_creatureData.skillList);

        // 서포트 스킬 데이터
        _skillBook.SetSupportSkill();

        // TODO : 장비 기능이 추가될 경우 계산해서 적용
        _curHp = _creatureData.maxHp;

        return true;
    }

    public bool CheckAlive()
    {
        return _isAlive;
    }

    public List<ActionSkill> GetActionSkillList()
    {
        List<ActionSkill> skillList = null;
        skillList = _skillBook.ActionSkillList;
        return skillList;
    }

    public List<SupportSkill> GetSupportSkillList()
    {
        List<SupportSkill> supportSkillList = null;
        supportSkillList = _skillBook.SupportSKillList;
        return supportSkillList;
    }
    
    public SkillBase GetOwnedSkillData(int skillId)
    {
        return _skillBook.GetSkill(skillId);
    }

    public void SkillUpgrade(int skillId)
    {
        _skillBook.UpgradeSkill(skillId);
        SkillUpgradeComplete();
    }

    // 다음 스킬 레벨과 현재 스킬 레벨간의 float 증가량을 가져오는 메소드
    public float GetSupportSkillValueFloatInc(int skillId)
    {
        float inc = 0f;
        float curValue = 0f; 
        float nextValue = 0f;

        SupportSkillData curSkillData = Managers.Instance.DataTableManager.DataTableSupportSkill.GetSupportSkillData(skillId);

        SupportSkillData nextSkillData = Managers.Instance.DataTableManager.DataTableSupportSkill.GetSupportSkillData(skillId+1);

        curValue = curSkillData.value;
        nextValue = nextSkillData == null ? 0f : nextSkillData.value;
        
        inc = nextValue == 0 ? nextValue : nextValue - curValue;

        return inc;
    }

    public override void UpdateController()
    {
        base.UpdateController();
    }

    public virtual void OnDamaged(BaseController attacker , float  damage) 
    {
        if (_curHp <= 0)
            return;

        _curHp -= (int)damage;

        if(_curHp <= 0)
        {
            _curHp = 0f;
           
            OnDead();
        }
    }

    public virtual void OnDead() { }
    public virtual void SkillUpgradeComplete() { }

}
