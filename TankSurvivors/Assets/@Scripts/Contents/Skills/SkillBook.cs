using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBook
{
    private List<SkillBase> _skillList = new List<SkillBase>();

    public void SetSkillList(List<int> skillList)
    {
        for(int i =0; i < skillList.Count; i++) 
        {
            SkillBase skill = new SkillBase();
        }
    }
}
