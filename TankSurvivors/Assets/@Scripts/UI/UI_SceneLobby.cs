using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SceneLobby : UI_Scene
{
    #region Enum UI
    private enum eGameObject
    {

    }
    private enum eButton
    {
        Button_StageSelect,
        Button_StageStart
    }
    private enum eImage
    {

    }

    private enum eText
    {
        Text_Stamina,
        Text_Gold,
        Text_Cash,
        Text_UserLevel,
        Text_Stage,
        Text_RequiredStamina
    }
    #endregion

    private Button _btnStageSelect;
    private Button _btnStageStart;

    private TMP_Text m_txtStamina;
    private TMP_Text m_txtGold;
    private TMP_Text m_txtCash;
    private TMP_Text m_txtUserLevel;
    private TMP_Text m_txtStage;
    private TMP_Text m_txtRequiredStamina;

    public override bool Init()
    {
        if(base.Init() == false)
        {
            return false;
        }
        _sceneState = eGameManagerState.Lobby;

        // Bind
        BindButton(typeof(eButton));
        BindText(typeof(eText));

        // Get
        _btnStageSelect = GetButton((int)eButton.Button_StageSelect);
        _btnStageStart = GetButton((int)eButton.Button_StageStart);

        m_txtStamina = GetText((int)eText.Text_Stamina);
        m_txtGold = GetText((int)eText.Text_Gold);
        m_txtCash = GetText((int)eText.Text_Cash);
        m_txtUserLevel = GetText((int)eText.Text_UserLevel);
        m_txtStage = GetText((int)eText.Text_Stage);
        m_txtRequiredStamina = GetText((int)eText.Text_RequiredStamina);

        // 
        _btnStageSelect.onClick.AddListener(OnClick_StageSelect);
        _btnStageStart.onClick.AddListener(OnClick_StageStart);
        
        return true;
    }

    public void Set()
    {
            
    }

    private void OnClick_StageSelect()
    {
        UIPopup_StageSelect popup = Managers.Instance.UIMananger.OpenPopup<UIPopup_StageSelect>();
        popup.Set();
    }
    private void OnClick_StageStart()
    {
        bool checkStamina = CheckStamina();
        if (checkStamina == false)
        {
            // 스태미나 부족 팝업창 띄우기
        }

        // 스태미나 차감 

        // 현재 선택된 스테이지 값 건네주기
        int stageIndex = 1; // 임시
        GameManager.Instance.StageStart(stageIndex);
    }

    private bool CheckStamina()
    {
        bool pass = false;

        return pass; 
    }
}
