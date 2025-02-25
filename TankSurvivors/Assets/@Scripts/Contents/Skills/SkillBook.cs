using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBook : MonoBehaviour
{
    [SerializeField]
    private List<ActionSkill> _actionSkillList = new List<ActionSkill>();
    public List<ActionSkill> ActionSkillList { get => _actionSkillList; }

    private List<SupportSkill> _supportSkillList = new List<SupportSkill>();
    public List<SupportSkill> SupportSKillList { get => _supportSkillList; }

    private ActionSkill _prevUseSkill = null;
    public ActionSkill PrevUseSkill { get => _prevUseSkill; set => _prevUseSkill = value; }


    public void SetActionSkill(List<int> skillList)
    { 
        for(int i =0; i < skillList.Count; i++) 
        {
            SkillData data = Managers.Instance.DataTableManager.DataTableSkill.GetSkillData(skillList[i]);
            ActionSkill skill = null;

            Define.eSkillType skillType = (Define.eSkillType)skillList[i];
            string className = skillType.ToString();
           
            Type type = Type.GetType(className);
            
            if(type != null)
                skill = gameObject.AddComponent(type) as ActionSkill;
 
            if (skill != null)
            {
                skill.SkillData = data;
                skill.SkillType = skillType;
                skill.Index = i + 1;

                _actionSkillList.Add(skill);
            }
        }
    }

    public void SetSupportSkill()
    {
        DataTableSupportSkill dataTableSupportSkill = Managers.Instance.DataTableManager.DataTableSupportSkill;
        List<int> supportSkillEnumList = Enum.GetValues(typeof(Define.eSupportSkillType)).Cast<int>().Where(id => id != 0 ).ToList();

        // 데이터 테이블 순서대로 서포트 스킬 생성 후 세팅
        for (int i = 0; i < supportSkillEnumList.Count; i++)
        {
            SupportSkillData supportSkillData = dataTableSupportSkill.GetSupportSkillData(supportSkillEnumList[i]);
           
            if (supportSkillData != null)
            {
                SupportSkill supportSkill = new SupportSkill();
                supportSkill.SupportSkillData = supportSkillData;
                supportSkill.SupportSkillType = (Define.eSupportSkillType)supportSkillEnumList[i];
                supportSkill.Index = i + 1;

                _supportSkillList.Add(supportSkill);
            }
        }
    }

    public void UpgradeSkill(int skillId)
    {
        SkillBase skill = GetSkill(skillId);

        if (skill.CurSkillLevel >= Define.MAX_SKILL_LEVEL)
            return;

        skill.SkillLevelUp();

        if (skill.CurSkillLevel > 1)
        {
            // 현재 스킬의 다음 스킬 존재하는지 확인
            int nextSkillId = 0;

            if (skill is ActionSkill)
            {
                ActionSkill actionSkill = skill as ActionSkill;
                nextSkillId = actionSkill.SkillData.skillId + 1;

                // 다음 스킬이 존재하는 경우 현재 스킬북에 담긴 스킬의 데이터를 다음 스킬 데이터로 덮어 씌어준다.
                SkillData nextActionSkillData = Managers.Instance.DataTableManager.DataTableSkill.GetSkillData(nextSkillId);
                actionSkill.SkillData = nextActionSkillData;

                actionSkill.RemoveCoolTime();
            }
            else if (skill is SupportSkill)
            {
                SupportSkill supportSkill = skill as SupportSkill;
                nextSkillId = ((SupportSkill)skill).SupportSkillData.skillId + 1;

                SupportSkillData nextSupportSkillData = Managers.Instance.DataTableManager.DataTableSupportSkill.GetSupportSkillData(nextSkillId);
                supportSkill.SupportSkillData = nextSupportSkillData;
            }
        }
    }

    private SkillBase GetSkill(int skillId)
    {
       // Action Skill Check
        for(int i =0; i < _actionSkillList.Count; i++)
        {
            if(_actionSkillList[i].SkillData.skillId == skillId)
            {
                return _actionSkillList[i];
            }
        }

        // Support Skill Check
        for(int i =0; i < _supportSkillList.Count; i++)
        {
            if(_supportSkillList[i].SupportSkillData.skillId == skillId)
            {
                return _supportSkillList[i];
            }
        }

        return null;
    }

    public ActionSkill GetCoolTimeEndSkill()
    {
        for (int i = _actionSkillList.Count - 1; i >= 0; i--)
        {
            if (_actionSkillList[i].RemainCoolTime <= 0f)
            {
                return _actionSkillList[i];
            }
        }
        return null;
    }

}
