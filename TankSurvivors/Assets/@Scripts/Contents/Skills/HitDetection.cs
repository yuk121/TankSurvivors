using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetection : BaseController
{
    private SkillData _skillData;
    private CreatureController _owner;

    public void SetData(SkillData skillData, CreatureController owner)
    {
        _skillData = skillData;
        _owner = owner;
    }

    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1f);
        
        foreach(Collider collider in colliders)
        {
            MonsterController mon = collider.GetComponent<MonsterController>();

            if(mon != null) 
            {
                float damage = _skillData.damage;
                mon.OnDamaged(_owner, damage);
                //  해당 프로젝타일 풀링
            }
        }
    }
}
