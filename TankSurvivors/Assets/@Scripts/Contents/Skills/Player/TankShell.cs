using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankShell : ActionSkill
{
    [SerializeField]
    private float _projectileBaseAnlge = 22.5f;

    [Range(1,5)]
    [SerializeField]
    private int _testProjectileCount = 1;

    public override void UseSkill(CreatureController owner)
    {
        base.UseSkill(owner);

        // TODO : 사운드 추가
        Transform spawnTrans = GameManager.Instance.Player.DummyFirePos;

        // 포구 화염 
        ParticleController muzzleParticle = Managers.Instance.ResourceManager.Instantiate(Define.DUMMY_FIREPOS_MUZZLE_PREFAB_PATH, pooling: true) 
            .GetComponent<ParticleController>();
        muzzleParticle.Init(spawnTrans.position, spawnTrans.forward);

        // 포탄 생성
        int projectileCount = Mathf.Max(SkillData.startCreateCount, _testProjectileCount);

        if (projectileCount < 2)
        {
            GenerateProjectileSkill(owner, spawnTrans.position, spawnTrans.forward, Vector3.zero);
        }
        else
        {
            float startAngle = -(_projectileBaseAnlge * (projectileCount - 1) / 2); // 첫 발사체 시작 각도
            
            for (int i = 0; i < projectileCount; i++)
            {
                float angle = startAngle + (i * _projectileBaseAnlge);
            
                Quaternion rotation = Quaternion.Euler(0, angle, 0); 
                Vector3 spawnDir = rotation.eulerAngles;
                GenerateProjectileSkill(owner, spawnTrans.position,spawnTrans.forward, spawnDir);
            }
        }
    }

    private void Update()
    {
        if (RemainCoolTime > 0)
        {
            RemainCoolTime -= Time.deltaTime;
        }
    }
}
