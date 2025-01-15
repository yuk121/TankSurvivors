using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBook
{
    private List<SkillBase> _skillList = new List<SkillBase>();
    public List<SkillBase> SkillList { get => _skillList; }

    private SkillBase _prevUseSkill = new SkillBase();
    public SkillBase PrevUseSkill { get => _prevUseSkill; set => _prevUseSkill = value; }


    public void SetSkillBook(List<int> skillList)
    { 
        for(int i =0; i < skillList.Count; i++) 
        {
            SkillBase skill = new SkillBase();
            SkillData data = Managers.Instance.DataTableManager.DataTableSkill.GetSkillData(skillList[i]);

            skill.SkillData = data;
            skill.Index = i+1;

            _skillList.Add(skill);
        }
    }

    public void UpgradeSkill(int skillId)
    {
        SkillBase skill = GetSkill(skillId);

        skill.SkillLevelup();

        if (skill.CurSkillLevel > 1)
        {
            // ���� ��ų�� ���� ��ų �����ϴ��� Ȯ��
            int nextSkillId = skill.SkillData.skillId + 1;
            bool isMax = CheckMaxLevelSkill(nextSkillId);

            if (isMax)
                return;

            // ���� ��ų�� �����ϴ� ��� ���� ��ų�Ͽ� ��� ��ų�� �����͸� ���� ��ų �����ͷ� ���� �����ش�.
            SkillData nextSkillData = Managers.Instance.DataTableManager.DataTableSkill.GetSkillData(nextSkillId);
            skill.SkillData = nextSkillData;

            skill.RemoveCoolTime();
        }
    }

    private bool CheckMaxLevelSkill(int nextSkillId)
    {
        bool isMax = false;
        SkillData nextSkillData = Managers.Instance.DataTableManager.DataTableSkill.GetSkillData(nextSkillId);

        if(nextSkillData == null)
            isMax = true;

        return isMax;
    }

    private SkillBase GetSkill(int skillId)
    {
        for(int i =0; i < _skillList.Count; i++)
        {
            if(_skillList[i].SkillData.skillId == skillId)
            {
                return _skillList[i];
            }
        }

        return null;
    }
}
