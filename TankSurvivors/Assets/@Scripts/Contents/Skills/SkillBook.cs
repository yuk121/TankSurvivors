using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBook : MonoBehaviour
{
    [SerializeField]
    private List<SkillBase> _skillList = new List<SkillBase>();
    public List<SkillBase> SkillList { get => _skillList; }

    private SkillBase _prevUseSkill = new SkillBase();
    public SkillBase PrevUseSkill { get => _prevUseSkill; set => _prevUseSkill = value; }


    public void SetSkillBook(List<int> skillList)
    { 
        for(int i =0; i < skillList.Count; i++) 
        {
            SkillData data = Managers.Instance.DataTableManager.DataTableSkill.GetSkillData(skillList[i]);
            
            Define.eSkillType skillType = (Define.eSkillType)skillList[i];
            string className = skillType.ToString(); 
            SkillBase skill = gameObject.AddComponent(Type.GetType(className)) as SkillBase;

            if (skill != null)
            {
                skill.SkillData = data;
                skill.SkillType = skillType;
                skill.Index = i + 1;

                _skillList.Add(skill);
            }
        }
    }

    public void UpgradeSkill(int skillId)
    {
        SkillBase skill = GetSkill(skillId);

        skill.SkillLevelup();

        if (skill.CurSkillLevel > 1)
        {
            // 현재 스킬의 다음 스킬 존재하는지 확인
            int nextSkillId = skill.SkillData.skillId + 1;
            bool isMax = CheckMaxLevelSkill(nextSkillId);

            if (isMax)
                return;

            // 다음 스킬이 존재하는 경우 현재 스킬북에 담긴 스킬의 데이터를 다음 스킬 데이터로 덮어 씌어준다.
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

    public SkillBase GetCoolTimeEndSkill()
    {
        for (int i = _skillList.Count - 1; i >= 0; i--)
        {
            if (_skillList[i].RemainCoolTime <= 0f)
            {
                return _skillList[i];
            }
        }

        return null;
    }

}
