using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankShell : SkillBase
{
    public override void UseSkill()
    {
        base.UseSkill();

        Transform spawnPos = GameManager.Instance.Player.DummyFirePos;
        GenerateSkillPrefab(spawnPos);
    }

}
