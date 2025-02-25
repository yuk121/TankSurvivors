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

        if (_skill is ActionSkill) // ��ų ����
        {
            ActionSkill actionSkill = _skill as ActionSkill;
            
            // ��ų �̹���
            _imgSkill.sprite = Managers.Instance.ResourceManager.Load<Sprite>(actionSkill.SkillData.skillImage);
           
            // ��ų �̸�
            _txtSkillName.text = Managers.Instance.DataTableManager.DataTableLocalization.GetLocalString(actionSkill.SkillData.skillLocalName);
         
            // ��ų ����
            _txtSkillDesc.text = Managers.Instance.DataTableManager.DataTableLocalization.GetLocalString(actionSkill.SkillData.skillLocalDesc);

            // ��ų ������ �´� ��� ����
            _imgActionSkillBg.gameObject.SetActive(true);
            _imgSupportSkillBg.gameObject.SetActive(false);

            // ��ų id 
            _skillId = actionSkill.SkillData.skillId;
        }
        else if(_skill is SupportSkill) // ����Ʈ ��ų ���� 
        {
            SupportSkill supportSkill = _skill as SupportSkill;
            _imgSkill.sprite = Managers.Instance.ResourceManager.Load<Sprite>(supportSkill.SupportSkillData.skillImage);

            _txtSkillName.text = Managers.Instance.DataTableManager.DataTableLocalization.GetLocalString(supportSkill.SupportSkillData.skillLocalName);
         
            _txtSkillDesc.text = Managers.Instance.DataTableManager.DataTableLocalization.GetLocalString(supportSkill.SupportSkillData.skillLocalDesc);

            _imgActionSkillBg.gameObject.SetActive(false);
            _imgSupportSkillBg.gameObject.SetActive(true);

            _skillId = supportSkill.SupportSkillData.skillId;
        }

        // ��ų ������ �°� �� ���� �����ֱ�
        _uiListGrade.SetGrade(_skill.CurSkillLevel);
    }

    private void OnClick_RandomSkill()
    {
        // ������ ��ų�� id�� ���� ��ų ���׷��̵�
        PlayerController player = GameManager.Instance.Player;
        player.SkillUpgrade(_skillId);
        
        Managers.Instance.UIMananger.ClosePopup();
    }
}
