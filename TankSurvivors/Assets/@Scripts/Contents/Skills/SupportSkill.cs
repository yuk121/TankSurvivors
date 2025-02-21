using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportSkill : SkillBase
{
    // SupportSkill
    private SupportSkillData _supportSkillData = new SupportSkillData();
    public SupportSkillData SupportSkillData { get => _supportSkillData; set => _supportSkillData = value; }

    private Define.eSupportSkillType _eSupportSkillType = Define.eSupportSkillType.None;
    public Define.eSupportSkillType SupportSkillType { get => _eSupportSkillType; set => _eSupportSkillType = value; }
    public int CurSkillLevel { get; set; } = 0;
    public int Index { get; set; } = 0;

    public void SkillLevelUp()
    {
        if (CurSkillLevel <= Define.MAX_SKILL_LEVEL)
            CurSkillLevel++;
    }
}
