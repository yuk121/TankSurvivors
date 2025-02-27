using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetection : BaseController
{
    private SkillData _skillData;
    private CreatureController _owner;
    private float _radius;

    public void SetData(SkillData skillData, CreatureController owner, float radius)
    {
        _skillData = skillData;
        _owner = owner;
        _radius = radius;
    }

    public void OnUpdateRadius(float radius)
    {
        _radius = radius;
    }

    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);
        
        foreach(Collider collider in colliders)
        {
            MonsterController mon = collider.GetComponent<MonsterController>();

            if(mon != null) 
            {
                float damage = _skillData.damage;
                mon.OnDamaged(_owner, damage);
            }
        }
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
