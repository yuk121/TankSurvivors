using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup_SkillSelect : UI_Base
{
    #region Enum_UI
    enum eGameObject
    {
        UIList_OwnedSkills,
        UIList_RandomSkills
    }
    #endregion

    UIList_OwnedSkills _uiListOwnedSkills = null;
    UIList_RandomSkills _uiListRandomSkills = null;
    List<UIElement_RandomSkill> _prevGetRandomSkillList = new List<UIElement_RandomSkill>(); // 중복 방지용
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }
        // Bind
        BindOjbect(typeof(eGameObject));

        // Get
        _uiListOwnedSkills = GetObject((int)eGameObject.UIList_OwnedSkills).GetComponent<UIList_OwnedSkills>();
        _uiListRandomSkills = GetObject((int)eGameObject.UIList_RandomSkills).GetComponent<UIList_RandomSkills>();

        return true;
    }

    public void SetSkillSelect()
    {
        if(_init == false)
        {
            Init();
        }

        gameObject.SetActive(true);

        _uiListOwnedSkills.SetOwnedSkills();
        _uiListRandomSkills.SetRandomSkills(GetRandomSkills());
    }


    private List<SkillBase> GetRandomSkills()
    {
        List<SkillBase> skillList = new List<SkillBase>();
        List<SkillBase> skillRandomList = new List<SkillBase>();

        // 플레이어가 사용가능한 action skill 목록 가져오기
        List<ActionSkill> actionSkillList = GameManager.Instance.Player.GetActionSkillList();
        // 플레이어가 배울수 있는 support skill 목록 가져오기
        List<SupportSkill> supportSkillList = GameManager.Instance.Player.GetSupportSkillList();
        
        for(int i =0; i < actionSkillList.Count; i++)
        {
            if(actionSkillList[i].CurSkillLevel < Define.MAX_SKILL_LEVEL)
            {
                skillList.Add(actionSkillList[i]);
            }
        }

        for(int i = 0; i < supportSkillList.Count; i++)
        {
            if(supportSkillList[i].CurSkillLevel < Define.MAX_SKILL_LEVEL)
            {
                skillList.Add(supportSkillList[i]);
            }
        }

        int RandomSkillCount = Mathf.Min(skillList.Count, Define.MAX_SKILL_SELECT_COUNT);
        
        while(skillRandomList.Count < RandomSkillCount)
        {
            int rand = Random.Range(0, skillList.Count);

            skillRandomList.Add(skillList[rand]);
            skillList.RemoveAt(rand);
        }

        return skillRandomList;
    }
}
