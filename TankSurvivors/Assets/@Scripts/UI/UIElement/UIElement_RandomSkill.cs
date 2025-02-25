using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    private TMP_Text _txtSkillName = null;
    private TMP_Text _txtSkillDesc = null;

    private int _skillId = -1;

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

        _txtSkillName = GetText((int)eText.Text_SkillName);
        _txtSkillDesc = GetText((int)eText.Text_SkillDesc);
        
        return true;
    }


    public void SetRandomSkill(SkillBase skill)
    {
        if (_init == false)
            Init();

        _skill = skill;
        _skillId = -1;

        if (_skill is ActionSkill) // 스킬 정보
        {
            ActionSkill actionSkill = _skill as ActionSkill;
            
            // 스킬 이미지
            _imgSkill.sprite = Managers.Instance.ResourceManager.Load<Sprite>(actionSkill.SkillData.skillImage);
           
            // 스킬 이름
            _txtSkillName.text = Managers.Instance.DataTableManager.DataTableLocalization.GetLocalString(actionSkill.SkillData.skillLocalName);
         
            // 스킬 설명
            _txtSkillDesc.text = Managers.Instance.DataTableManager.DataTableLocalization.GetLocalString(actionSkill.SkillData.skillLocalDesc);

            // 스킬 종류에 맞는 배경 선택
            _imgActionSkillBg.gameObject.SetActive(true);
            _imgSupportSkillBg.gameObject.SetActive(false);

            // 스킬 id 
            _skillId = actionSkill.SkillData.skillId;
        }
        else if(_skill is SupportSkill) // 서포트 스킬 정보 
        {
            SupportSkill supportSkill = _skill as SupportSkill;
            _imgSkill.sprite = Managers.Instance.ResourceManager.Load<Sprite>(supportSkill.SupportSkillData.skillImage);

            _txtSkillName.text = Managers.Instance.DataTableManager.DataTableLocalization.GetLocalString(supportSkill.SupportSkillData.skillLocalName);
         
            _txtSkillDesc.text = Managers.Instance.DataTableManager.DataTableLocalization.GetLocalString(supportSkill.SupportSkillData.skillLocalDesc);

            _imgActionSkillBg.gameObject.SetActive(false);
            _imgSupportSkillBg.gameObject.SetActive(true);

            _skillId = supportSkill.SupportSkillData.skillId;
        }

        // 스킬 레벨에 맞게 별 갯수 보여주기
        _uiListGrade.SetGrade(_skill.CurSkillLevel);
    }

    private void OnClick_RandomSkill()
    {
        // 선택한 스킬의 id를 통해 스킬 업그레이드
        PlayerController player = GameManager.Instance.Player;
        player.SkillUpgrade(_skillId);
        
        Managers.Instance.UIMananger.ClosePopup();
    }
}
