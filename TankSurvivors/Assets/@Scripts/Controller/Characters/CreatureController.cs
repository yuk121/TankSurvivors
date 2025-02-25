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
    public float CurHp { get => _curHp; }
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

    public void SkillUpgrade(int skillId)
    {
        _skillBook.UpgradeSkill(skillId);
    }

    public override void UpdateController()
    {
        base.UpdateController();
    }

    public virtual void OnDamaged(BaseController attacker , float  damage) 
    {
        if (_curHp <= 0)
            return;

        _curHp -= damage;

        if(_curHp <= 0)
        {
            _curHp = 0f;
           
            OnDead();
        }
    }

    public virtual void OnDead() { }
}
