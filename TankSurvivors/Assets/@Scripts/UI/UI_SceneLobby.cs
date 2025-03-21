using System;
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
        Button_StageStart,
        Button_Setting,
    }
    private enum eImage
    {

    }

    private enum eText
    {
        Text_StaminaCurrent,
        Text_StaminaMax,
        Text_Gold,
        Text_Cash,
        Text_UserLevel,
        Text_Stage,
        Text_RequiredStamina,
        Text_StaminaRecoveryTime
    }
    #endregion

    private Button _btnStageSelect;
    private Button _btnStageStart;
    private Button _btnSetting;

    private TMP_Text _txtStaminaCurrent;
    private TMP_Text _txtStaminaMax;
    private TMP_Text _txtGold;
    private TMP_Text _txtCash;
    private TMP_Text _txtUserLevel;
    private TMP_Text _txtStage;
    private TMP_Text _txtRequiredStamina;
    private TMP_Text _txtStaminaRecoveryTime;

    private UserData _userData = null;
    private long _currentTimestamp = 0L;
    private long _remainTime = 0L;
    private int _recoveryStaminaValue = 0;
    private int _recoveryRemainTimeMin = 0;
    private int _recoveryRemainTimeSec = 0;

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
        _btnSetting = GetButton((int)eButton.Button_Setting);

        _txtStaminaCurrent = GetText((int)eText.Text_StaminaCurrent);
        _txtStaminaMax = GetText((int)eText.Text_StaminaMax);
        _txtGold = GetText((int)eText.Text_Gold);
        _txtCash = GetText((int)eText.Text_Cash);
        _txtUserLevel = GetText((int)eText.Text_UserLevel);
        _txtStage = GetText((int)eText.Text_Stage);
        _txtRequiredStamina = GetText((int)eText.Text_RequiredStamina);
        _txtStaminaRecoveryTime = GetText((int)eText.Text_StaminaRecoveryTime);

        // 
        _btnStageSelect.onClick.AddListener(OnClick_StageSelect);
        _btnStageStart.onClick.AddListener(OnClick_StageStart);
        _btnSetting.onClick.AddListener(OnClick_Setting);

        SetUserInfo();
        SetStage();

        return true;
    }

    private void SetUserInfo()
    {
        _userData = Managers.Instance.UserDataManager.UserData;

        SetStaminaInfo();
        _txtGold.text = $"{_userData._userCurrency.gold}";
        _txtCash.text = $"{_userData._userCurrency.GetCash()}";
        _txtUserLevel.text = $"{Managers.Instance.UserDataManager.GetUserLevel()}";
    }

    private void SetStaminaInfo()
    {
        if (_userData == null)
            return;

        _txtStaminaCurrent.text = $"{_userData._userStaminaCurrent}";
        _txtStaminaMax.text = $"/ {_userData._userStaminaMax}";
    }

    private void SetStage()
    {
        int stage = Managers.Instance.UserDataManager.GetLastSelectStage();
        StageData stageData = Managers.Instance.DataTableManager.DataTableStage.GetStageInfo(stage);
        string stageName = Managers.Instance.DataTableManager.DataTableLocalization.GetLocalString(stageData.stageLocalizeName);

        Image stageIcon = _btnStageSelect.GetComponent<Image>();
        stageIcon.sprite = Managers.Instance.ResourceManager.Load<Sprite>(stageData.stageIcon);
       
        _txtStage.text = $"{stageName}";
        _txtRequiredStamina.text = $" X {Define.STAGE_ENTER_STAMINA}";
    }

    private void OnClick_StageSelect()
    {
        SoundManager.Instance.PlayButtonSound();

        UIPopup_StageSelect popup = Managers.Instance.UIMananger.OpenPopup<UIPopup_StageSelect>();
        popup.Set();
    }

    private void Update_LobbyUI()
    {
        SetStage();
    }

    private void OnClick_StageStart()
    {
        SoundManager.Instance.PlayButtonSound();

        int requiredStamina = Define.STAGE_ENTER_STAMINA;
        bool checkStamina = Managers.Instance.UserDataManager.CheckStamina(requiredStamina);
    
        // ���¹̳� ���� �˾�â ����
        if (checkStamina == false)
        {
            UIPopup_NotificationBar popup = Managers.Instance.UIMananger.OpenPopup<UIPopup_NotificationBar>();
            popup.SetMessage("���׹̳� ����");
            return;
        }

        // ���� ���õ� �������� �� �ǳ��ֱ�
        int stageIndex = Managers.Instance.UserDataManager.GetLastSelectStage();
        GameManager.Instance.StageStart(stageIndex);
    }

    private void OnClick_Setting()
    {
        SoundManager.Instance.PlayButtonSound();

        UIPopup_Setting popup = Managers.Instance.UIMananger.OpenPopupWithTween<UIPopup_Setting>();
        popup.Set();
    }

    public void Update()
    {
        if (CheckStaminaFull() == false)
        {
            if (_txtStaminaRecoveryTime.gameObject.activeSelf == false)
            {
                _txtStaminaRecoveryTime.gameObject.SetActive(true);
            }

            // ���������� ���׹̳ʰ� ���� �ð����κ��� �󸶳� �������� Ȯ���ϱ�
            _currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - _userData._lastStaminaChageTimestamp;
            // ȸ������ ���� �ð�
            _remainTime = (long)Define.STAMINA_RECOVERY_TIME - (_currentTimestamp % (long)Define.STAMINA_RECOVERY_TIME);
           
            // ���׹̳� ȸ����
            _recoveryStaminaValue = (int)(_currentTimestamp / Define.STAMINA_RECOVERY_TIME);
            
            // ���׹̳� ȸ��
            if (_recoveryStaminaValue > 0)
            {
                // �ִ밪�� ���� �ʵ��� ȸ�� (ȸ�� �� ���� �ִ밪 ���� ũ�ٸ� ���¹̳� �ִ밪���� ���� ���¹̳����� �� ��ŭ�� �����ְ� �ƴ� ��� �״�� ȸ��) 
                _recoveryStaminaValue = _userData._userStaminaCurrent + _recoveryStaminaValue > _userData._userStaminaMax ? 
                    _userData._userStaminaMax - _userData._userStaminaCurrent : _recoveryStaminaValue;

                Managers.Instance.UserDataManager.RecoveryStamina(_recoveryStaminaValue);
                SetStaminaInfo();
            }
            // �ð� ǥ��
            _recoveryRemainTimeMin = (int)(_remainTime / 60f);
            _recoveryRemainTimeSec = (int)(_remainTime % 60f);
            _txtStaminaRecoveryTime.text = $"{_recoveryRemainTimeMin:D2} : {_recoveryRemainTimeSec:D2}";          
        }
        else
        {
            if(_txtStaminaRecoveryTime.gameObject.activeSelf == true)
            {
                _txtStaminaRecoveryTime.gameObject.SetActive(false);
            }
        }
    }

    private bool CheckStaminaFull()
    {
        _userData = Managers.Instance.UserDataManager.UserData;

        bool isFull = true;
        
        if (_userData._userStaminaMax > _userData._userStaminaCurrent )
        {
            isFull = false;
        }

        return isFull;
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.Update_LobbyUI += Update_LobbyUI;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.Update_LobbyUI -= Update_LobbyUI;
    }
}
