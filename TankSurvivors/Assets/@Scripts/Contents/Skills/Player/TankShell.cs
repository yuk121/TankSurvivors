using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankShell : ActionSkill
{
    public override void UseSkill(CreatureController owner)
    {
        base.UseSkill(owner);

        // TODO : 사운드 추가
        Transform spawnTrans = GameManager.Instance.Player.DummyFirePos;

        // 포구 화염 
        ParticleController muzzleParticle = Managers.Instance.ResourceManager.Instantiate(Define.DUMMY_FIREPOS_MUZZLE_PREFAB_PATH, pooling: true) 
            .GetComponent<ParticleController>();
        muzzleParticle.Init(spawnTrans.position, spawnTrans.forward);
        
        // 포탄
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
