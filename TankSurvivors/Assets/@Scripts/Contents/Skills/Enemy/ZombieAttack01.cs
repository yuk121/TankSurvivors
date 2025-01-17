using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttack01 : SkillBase
{
    private void Update()
    {
        if (RemainCoolTime > 0)
        {
            RemainCoolTime -= Time.deltaTime;
        }
    }
}
