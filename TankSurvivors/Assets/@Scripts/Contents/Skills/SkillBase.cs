using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBase : MonoBehaviour
{
    private const int MAX_SKILLLEVEL = 5;

    private SkillData _skillData = new SkillData();
    public SkillData SkillData { get => _skillData; set => _skillData = value; }

    private Define.eSkillType _skillType = Define.eSkillType.None;
    public Define.eSkillType SkillType  { get => _skillType; set => _skillType = value; }

    private int _curSkillLevel = 0;
    public int CurSkillLevel { get => _curSkillLevel; }

    private float _remainCoolTime = 0;        // 쿨타임 남은 시간
    public float RemainCoolTime { get => _remainCoolTime; set => _remainCoolTime = value; }

    private int _Index;    // 스킬 순서
    public int Index { get => _Index; set => _Index = value; }

    public void StartCoolTime()
    {
        _remainCoolTime = SkillData.coolTime;
    }

    public void RemoveCoolTime()
    {
        _remainCoolTime = 0f;
    }

    public void SkillLevelup()
    {
        if (_curSkillLevel <= MAX_SKILLLEVEL)
            _curSkillLevel++;
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
}
