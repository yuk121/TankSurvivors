using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CreatureController : BaseController
{
    protected float _speed = 1.0f;
    protected int _hp = 100;
    protected int _maxHp = 100;
    protected AnimationController _animController;

    public override bool Init()
    {
        base.Init();

        Utils.GetOrAddComponent<AnimationController>(gameObject);


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
