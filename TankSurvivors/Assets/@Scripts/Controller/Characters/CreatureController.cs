using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CreatureController : BaseController
{
    protected int _hp;
    protected CreatureData _creatureData;
    public CreatureData CreatureData
    {
        get => _creatureData;
        set => _creatureData = value;
    }

    protected SkillBook _skillBook;
    protected AnimationController _animController;

    public override bool Init()
    {
        base.Init();

        _animController = Utils.GetOrAddComponent<AnimationController>(gameObject);

        return true;
    }

    public virtual void OnDamaged(BaseController attacker , int  damage) 
    {
        if (_hp <= 0)
            return;

        _hp -= damage;

        if( _hp <= 0)
        {
            _hp = 0;
           
            OnDead();
        }
    }

    public virtual void OnDead() { }
}
