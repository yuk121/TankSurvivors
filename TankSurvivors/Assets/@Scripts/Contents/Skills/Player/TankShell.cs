using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankShell : SkillBase
{
    public override void UseSkill(CreatureController owner)
    {
        base.UseSkill(owner);

        // TODO : ���� �߰�
        Transform spawnTrans = GameManager.Instance.Player.DummyFirePos;

        // ���� ȭ�� 
        Managers.Instance.ResourceManager.Instantiate(Define.DUMMY_FIREPOS_MUZZLE_PREFAB_PATH, spawnTrans, true);
        // ��ź
        GenerateProjectileSkill(owner, spawnTrans);
    }

    private void Update()
    {
        if (RemainCoolTime > 0)
        {
            RemainCoolTime -= Time.deltaTime;
        }
    }
}
