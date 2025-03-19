using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankShell : ActionSkill
{
    [SerializeField]
    private float _projectileBaseAnlge = 15f;

    public override void UseSkill(CreatureController owner)
    {
        base.UseSkill(owner);

        // ���� 
        SoundManager.Instance.Play(SkillData.castSound, Define.eSoundType.SFX, -15f);

        Transform spawnTrans = GameManager.Instance.Player.DummyFirePos;

        // ���� ȭ�� 
        ParticleController muzzleParticle = Managers.Instance.ResourceManager.Instantiate(Define.DUMMY_FIREPOS_MUZZLE_PREFAB_PATH, pooling: true) 
            .GetComponent<ParticleController>();
        muzzleParticle.Init(spawnTrans.position, spawnTrans.forward);

        // ��ź ����
        int projectileCount =SkillData.startCreateCount;

        if (projectileCount < 2)
        {
            GenerateProjectileSkill(owner, spawnTrans.position, spawnTrans.forward);
        }
        else
        {
            float startAngle = -(_projectileBaseAnlge * (projectileCount - 1) / 2); // ù �߻�ü ���� ����
            
            for (int i = 0; i < projectileCount; i++)
            {
                float angle = startAngle + (i * _projectileBaseAnlge);
            
                Quaternion rotation = Quaternion.AngleAxis(angle, spawnTrans.up); 
                Vector3 newDir = rotation * spawnTrans.forward;
                GenerateProjectileSkill(owner, spawnTrans.position, newDir);
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
