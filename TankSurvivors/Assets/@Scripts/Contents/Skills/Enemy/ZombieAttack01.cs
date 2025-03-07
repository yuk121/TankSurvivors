using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttack01 : ActionSkill
{
    public override void UseSkill(CreatureController owner)
    {
        base.UseSkill(owner);
    }

    private void Update()
    {
        if (RemainCoolTime > 0)
        {
            RemainCoolTime -= Time.deltaTime;
        }
    }
}
