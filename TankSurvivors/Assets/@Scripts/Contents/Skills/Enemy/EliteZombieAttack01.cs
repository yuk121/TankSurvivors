using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteZombieAttack01 : ActionSkill
{
    public override void UseSkill(CreatureController owner)
    {
        base.UseSkill(owner);
    }

    // Update is called once per frame
    void Update()
    {
        if (RemainCoolTime > 0)
        {
            RemainCoolTime -= Time.deltaTime;
        }
    }
}
