using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSkill : MonoBehaviour, SkillBase
{
    // Skill
    private SkillData _skillData = new SkillData();
    public SkillData SkillData { get => _skillData; set => _skillData = value; }

    private Define.eSkillType _skillType = Define.eSkillType.None;
    public Define.eSkillType SkillType { get => _skillType; set => _skillType = value; }
    private float _remainCoolTime = 0;        // 쿨타임 남은 시간
    public float RemainCoolTime { get => _remainCoolTime; set => _remainCoolTime = value; }
    public int CurSkillLevel { get; set; } = 0;
    public int Index { get; set; } = 0;

    public void StartCoolTime()
    {
        _remainCoolTime = SkillData.coolTime;
    }

    public void RemoveCoolTime()
    {
        _remainCoolTime = 0f;
    }

    public virtual void UseSkill(CreatureController owner)
    {
        // 스킬 사용했으니 쿨타임 적용
        StartCoolTime();
    }

    protected virtual void GenerateProjectileSkill(CreatureController owenr, Transform spawnTrans)
    {
        Projectile projectile = Managers.Instance.ObjectManager.Spawn<Projectile>(spawnTrans.position, _skillData.skillId, spawnTrans.forward);
        projectile.Init(owenr, _skillData, _skillType);
    }

    public void SkillLevelUp()
    {
        if (CurSkillLevel <= Define.MAX_SKILL_LEVEL)
            CurSkillLevel++;
    }
}
