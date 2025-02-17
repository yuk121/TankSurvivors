using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIElement_RandomSkill : UI_Base
{
    #region Enum_UI
    enum eGameObject
    {
        UIList_Grade
    }

    enum eButton
    {
        UIElement_RandomSkill
    }

    enum eText
    {
        Text_SkillName,
        Text_SkillDesc
    }

    enum eImage
    {
        Image_Skill,
        Image_SkillDesc_Background
    }
    #endregion

    private SkillBase _skill = null;
    private Button _btnRandomSkill = null;
    private UIList_Grade _uiListGrade = null;
  
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        // Bind 
        BindOjbect(typeof(eGameObject));
        BindButton(typeof(eButton));
        BindText(typeof(eText));
        BindImage(typeof(eImage));

        //Get 
        _uiListGrade = GetObject((int)eGameObject.UIList_Grade).GetComponent<UIList_Grade>();
      
        _btnRandomSkill = GetButton((int)eButton.UIElement_RandomSkill);
        _btnRandomSkill.onClick.AddListener(OnClick_RandomSkill);
       
        return true;
    }


    public void SetRandomSkill(SkillBase skill)
    {
        _skill = skill;
        _uiListGrade.SetGrade(_skill.CurSkillLevel);
    }

    private void OnClick_RandomSkill()
    {
        PlayerController player = GameManager.Instance.Player;
        Managers.Instance.UIMananger.ClosePopup();
    }
}
