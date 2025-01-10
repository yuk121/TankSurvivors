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

    protected SkillBook _skillBook = new SkillBook();
    protected AnimationController _animController;
  
    [SerializeField]
    protected float _curHp;
    protected bool _isAlive = true;

    public override bool Init()
    {
        base.Init();

        _animController = Utils.GetOrAddComponent<AnimationController>(gameObject);
        
        // ���� ��ų ������ 
        _skillBook.SetSkillBook(_creatureData.skillList);

        // TODO : ��� ����� �߰��� ��� ����ؼ� ����
        _curHp = _creatureData.maxHp;

        return true;
    }

    public bool CheckAlive()
    {
        return _isAlive;
    }

    public override void UpdateController()
    {
        base.UpdateController();

        // ��ų ��Ÿ�� ����
        if(_skillBook.SkillList.Count > 0)
        {
            for(int i =0; i < _skillBook.SkillList.Count; i++)
            {
                //Debug.Log($"#CreatureController : {_skillBook.SkillList[i]} remain CoolTime : {_skillBook.SkillList[i].RemainCoolTime}");
                if (_skillBook.SkillList[i].RemainCoolTime > 0)
                {
                    _skillBook.SkillList[i].RemainCoolTime -= Time.deltaTime;
                }    
            }
        }
    }

    public SkillBase GetCoolTimeEndSkill()
    {
        for(int i = _skillBook.SkillList.Count -1; i >= 0; i--)
        {
            if(_skillBook.SkillList[i].RemainCoolTime <= 0f)
            {
                return _skillBook.SkillList[i];     
            }
        }

        return null;
    }

    public virtual void OnDamaged(BaseController attacker , float  damage) 
    {
        if (_curHp <= 0)
            return;

        // TODO : �¾����� �ð������� �����ֱ� ���� RimMeshRenderer ��¦ ������ �����ϱ�
        // MaterialPropertyBlock? Ȱ���ϱ�
        _curHp -= damage;

        if(_curHp <= 0)
        {
            _curHp = 0f;
           
            OnDead();
        }
    }

    public virtual void OnDead() { }
}
