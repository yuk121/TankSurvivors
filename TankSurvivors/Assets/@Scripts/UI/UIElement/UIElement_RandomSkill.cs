using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIElement_RandomSkill : UI_Base
{
    #region Enum_UI
    enum eGameObject
    {
        UIList_Grade,
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
        Image_ActionSkillBg,
        Image_SupportSkillBg
    }
    #endregion

    private SkillBase _skill = null;
    private Button _btnRandomSkill = null;
    private Image _imgSkill = null;
    private Image _imgActionSkillBg = null;
    private Image _imgSupportSkillBg = null;
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

        _imgSkill = GetImage((int)eImage.Image_Skill);
        _imgActionSkillBg = GetImage((int)eImage.Image_ActionSkillBg);
        _imgSupportSkillBg = GetImage((int)eImage.Image_SupportSkillBg);

        _btnRandomSkill = GetButton((int)eButton.UIElement_RandomSkill);
        _btnRandomSkill.onClick.AddListener(OnClick_RandomSkill);
       
        return true;
    }


    public void SetRandomSkill(SkillBase skill)
    {
        if (_init == false)
            Init();

        _skill = skill;
        
        if(_skill is ActionSkill)
        {
            ActionSkill actionSkill = _skill as ActionSkill;
            _imgSkill.sprite = Managers.Instance.ResourceManager.Load<Sprite>(actionSkill.SkillData.skillImage);

            _imgActionSkillBg.gameObject.SetActive(true);
            _imgSupportSkillBg.gameObject.SetActive(false);     
        }
        else if(_skill is SupportSkill)
        {
            SupportSkill supportSkill = _skill as SupportSkill;
            _imgSkill.sprite = Managers.Instance.ResourceManager.Load<Sprite>(supportSkill.SupportSkillData.skillImage);

            _imgActionSkillBg.gameObject.SetActive(false);
            _imgSupportSkillBg.gameObject.SetActive(true);
        }

        _uiListGrade.SetGrade(_skill.CurSkillLevel);
    }

    private void OnClick_RandomSkill()
    {
        PlayerController player = GameManager.Instance.Player;
        Managers.Instance.UIMananger.ClosePopup();
    }
}
