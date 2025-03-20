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

    private TMP_Text _txtStamina;
    private TMP_Text _txtGold;
    private TMP_Text _txtCash;
    private TMP_Text _txtUserLevel;
    private TMP_Text _txtStage;
    private TMP_Text _txtRequiredStamina;

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

        _txtStamina = GetText((int)eText.Text_Stamina);
        _txtGold = GetText((int)eText.Text_Gold);
        _txtCash = GetText((int)eText.Text_Cash);
        _txtUserLevel = GetText((int)eText.Text_UserLevel);
        _txtStage = GetText((int)eText.Text_Stage);
        _txtRequiredStamina = GetText((int)eText.Text_RequiredStamina);

        // 
        _btnStageSelect.onClick.AddListener(OnClick_StageSelect);
        _btnStageStart.onClick.AddListener(OnClick_StageStart);

        SetUserInfo();
        SetStage();

        return true;
    }

    public void SetUserInfo()
    {
        UserData userData = Managers.Instance.UserDataManager.UserData;

        _txtStamina.text = $"{userData._userStaminaCurrent}/{userData._userStaminaMax}";
        _txtGold.text = $"{userData._userCurrency.gold}";
        _txtCash.text = $"{userData._userCurrency.GetCash()}";
        _txtUserLevel.text = $"{Managers.Instance.UserDataManager.GetUserLevel()}";
    }

    public void SetStage()
    {
        int stage = Managers.Instance.UserDataManager.GetLastSelectStage();
        StageData stageData = Managers.Instance.DataTableManager.DataTableStage.GetStageInfo(stage);
        string stageName = Managers.Instance.DataTableManager.DataTableLocalization.GetLocalString(stageData.stageLocalizeName);
       
        _txtStage.text = $"{stageName}";
        _txtRequiredStamina.text = $" X {Define.STAGE_ENTER_STAMINA}";
    }

    private void OnClick_StageSelect()
    {
        SoundManager.Instance.PlayButtonSound();

        UIPopup_StageSelect popup = Managers.Instance.UIMananger.OpenPopup<UIPopup_StageSelect>();
        popup.Set();
    }
    private void OnClick_StageStart()
    {
        SoundManager.Instance.PlayButtonSound();

        int requiredStamina = Define.STAGE_ENTER_STAMINA;
        bool checkStamina = Managers.Instance.UserDataManager.CheckStamina(requiredStamina);
    
        // 스태미나 부족 팝업창 띄우기
        if (checkStamina == false)
        {
            UIPopup_NotificationBar popup = Managers.Instance.UIMananger.OpenPopup<UIPopup_NotificationBar>();
            popup.SetMessage("스테미나 부족");
            return;
        }

        // 현재 선택된 스테이지 값 건네주기
        int stageIndex = Managers.Instance.UserDataManager.GetLastSelectStage();
        GameManager.Instance.StageStart(stageIndex);
    }
}
