using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : ActionSkill
{
    public override void UseSkill(CreatureController owner)
    {
        base.UseSkill(owner);

        int createCount = SkillData.startCreateCount;
       
        for (int i = 0; i < createCount; i++)
        {
            // 플레이어 기준으로 지뢰 생성
            Vector3 spawnPos = Utils.GetPlayerNearCirclePos(_owner.transform.position, 5,10);

            spawnPos.y = 0f;
            
            HitDetection mine = Managers.Instance.ObjectManager.Spawn<HitDetection>(spawnPos, SkillData.skillId, Vector3.forward);

            mine.SetData(SkillData, _owner, SkillData.attackRange, Define.eSkillType.Mine);
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
