using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankShell : ActionSkill
{
    public override void UseSkill(CreatureController owner)
    {
        base.UseSkill(owner);

        // TODO : ���� �߰�
        Transform spawnTrans = GameManager.Instance.Player.DummyFirePos;

        // ���� ȭ�� 
        ParticleController muzzleParticle = Managers.Instance.ResourceManager.Instantiate(Define.DUMMY_FIREPOS_MUZZLE_PREFAB_PATH, pooling: true) 
            .GetComponent<ParticleController>();
        muzzleParticle.Init(spawnTrans.position, spawnTrans.forward);
        
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
