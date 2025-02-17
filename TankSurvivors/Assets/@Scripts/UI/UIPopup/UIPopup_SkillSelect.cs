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

        if (gameObject.activeSelf == true)
        {
            gameObject.SetActive(false);
        }

        return true;
    }

    public void SetSkillSelect()
    {
        gameObject.SetActive(true);

        _uiListOwnedSkills.SetOwnedSkills();
        _uiListRandomSkills.SetRandomSkills(GetRandomSkills());
    }


    private List<SkillBase> GetRandomSkills()
    {
        List<SkillBase> skillData = new List<SkillBase>();

        return skillData;
    }
}
