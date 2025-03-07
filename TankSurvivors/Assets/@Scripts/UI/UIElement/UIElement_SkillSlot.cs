using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIElement_SkillSlot : UI_Base
{
    #region Enum UI
    private enum eGameObject
    {
        UIList_Grade
    }
    private enum eImage
    {
        Image_Skill
    }
    private UIList_Grade _grade;

    #endregion
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        //Bind
        BindObject(typeof(eGameObject));
        BindImage(typeof(eImage));  

        // Get
        _grade = GetObject((int)eGameObject.UIList_Grade).GetComponent<UIList_Grade>();

        return true;
    }

    public void SetSkillSlot(ActionSkill actionSkill)
    {
        if(_init == false) 
        {
            Init();
        }

        GetImage((int)eImage.Image_Skill).sprite = Managers.Instance.ResourceManager.Load<Sprite>(actionSkill.SkillData.skillImage);

        _grade.SetGrade(actionSkill.CurSkillLevel);
    }

    public void SetSkillSlot(SupportSkill supportSkill)
    {
        if (_init == false)
        {
            Init();
        }

        GetImage((int)eImage.Image_Skill).sprite = Managers.Instance.ResourceManager.Load<Sprite>(supportSkill.SupportSkillData.skillImage);

        _grade.SetGrade(supportSkill.CurSkillLevel);
    }
}
