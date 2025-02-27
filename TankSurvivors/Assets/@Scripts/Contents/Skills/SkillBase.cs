using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SkillBase
{
    public int CurSkillLevel { get; set; }
    public int Index { get; set; }

    public abstract void SkillLevelUp();
    public abstract void OnUpdatedSkill();
}
