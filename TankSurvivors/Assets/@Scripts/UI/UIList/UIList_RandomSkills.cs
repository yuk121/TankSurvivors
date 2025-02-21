using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIList_RandomSkills : UI_Base
{
    #region Enum_UI
    enum eGameObject
    {
        UIElement_RandomSkill
    }
    #endregion

    GameObject _uiElementRandomSkillPrefab = null;
    Transform _trans = null;
  
    List<UIElement_RandomSkill> _randomSkillList = new List<UIElement_RandomSkill>();

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        _trans = transform;
        
        // Bind
        BindOjbect(typeof(eGameObject));

        // Get
        _uiElementRandomSkillPrefab = GetObject((int)eGameObject.UIElement_RandomSkill);
        _uiElementRandomSkillPrefab.gameObject.SetActive(false);

        //
        if (_randomSkillList.Count < 1)
        {
            for (int i = 0; i < Define.MAX_SKILL_SELECT_COUNT; i++)
            {
                UIElement_RandomSkill skill = Managers.Instance.UIMananger.InstantiateUI<UIElement_RandomSkill>(_trans);
                _randomSkillList.Add(skill);
            }
        }
        return true;
    }

    public void HideRandomSkillAll()
    {
        for(int i = 0; i < _randomSkillList.Count; i++)
        {
            _randomSkillList[i].gameObject.SetActive(false);
        }
    }

    public void SetRandomSkills(List<SkillBase> skillList)
    {
        if(_init == false)
        {
            Init();
        }

        HideRandomSkillAll();

        for(int i =0; i < skillList.Count; i++)
        {
            _randomSkillList[i].gameObject.SetActive(true);
            _randomSkillList[i].SetRandomSkill(skillList[i]);
        }
    }
}
