using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankShell : SkillBase
{
    public override void UseSkill(CreatureController owner)
    {
        base.UseSkill(owner);

        // TODO : ���� �߰� + ���� ��ġ�� ȭ�� ����Ʈ?

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
