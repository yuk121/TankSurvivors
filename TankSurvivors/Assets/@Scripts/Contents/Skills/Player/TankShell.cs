using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankShell : SkillBase
{
    public override void UseSkill(CreatureController owner)
    {
        base.UseSkill(owner);

        // TODO : 사운드 추가 + 포구 위치에 화염 이펙트?

        Transform spawnPos = GameManager.Instance.Player.DummyFirePos;
        GenerateProjectileSkill(owner,spawnPos);
    }

    private void Update()
    {
        if (RemainCoolTime > 0)
        {
            RemainCoolTime -= Time.deltaTime;
        }
    }
}
