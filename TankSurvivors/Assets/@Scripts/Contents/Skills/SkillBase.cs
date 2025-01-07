using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBase
{
    private SkillData _skillData = new SkillData();
    public SkillData SkillData { get => _skillData; set => _skillData = value; }

    private int _skillLevel;
    public int SkillLevel { get => _skillLevel; }

    public void SkillUpdate(int skillId)
    {
        SkillData data = new SkillData();
    }
}
