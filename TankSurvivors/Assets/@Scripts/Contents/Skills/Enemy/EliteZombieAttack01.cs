using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteZombieAttack01 : ActionSkill
{
    // Start is called before the first frame update
    void Start()
    {
        
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
