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
    protected bool _isAlive = true;

    public override bool Init()
    {
        base.Init();

        _animController = Utils.GetOrAddComponent<AnimationController>(gameObject);

        _skillBook = Utils.GetOrAddComponent<SkillBook>(gameObject);        
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
