using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossZombieAttack01 : ActionSkill
{
    // Update is called once per frame
    void Update()
    {
        if (RemainCoolTime > 0)
        {
            RemainCoolTime -= Time.deltaTime;
        }
    }
}
