using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPopup_GameResult_Defeat : UI_Popup
{
    #region Enum_UI

    private enum eGameObject
    {

    }

    private enum eButton
    {
        Button_OK,
        Button_Retry
    }

    private enum eText
    {
        Text_Stage,
        Text_Time,
        Text_KillCount
    }
    #endregion

    public override bool Init()
    {
        if(base.Init() == false)
        {
            return false;
        }

        _bAllowEscapeKey = false;

        //Bind
        BindButton(typeof(eButton));
        BindText(typeof(eText));

        //Get
        GetButton((int)eButton.Button_OK).onClick.AddListener(OnClick_OK);
        GetButton((int)eButton.Button_Retry).onClick.AddListener(OnClick_Retry);

        return true;
    }

    public void Set()
    {
        if (_init == false)
            Init();

        GameData gameData = GameManager.Instance.GameData;
        
        // Stage
        GetText((int)eText.Text_Stage).text = $"Stage {gameData.stageInfo.stageIndex}";

        // Time
        int min = (int)(gameData.curTime / 60f);
        int sec = (int)(gameData.curTime % 60f);

        GetText((int)eText.Text_Time).text = $"{min:D2} : {sec:D2}";

        // KillCount
        GetText((int)eText.Text_KillCount).text = $"{gameData.killCount}";
    }

    private void OnClick_OK()
    {
        SoundManager.Instance.PlayButtonSound();
        GameManager.Instance.GoLobby();
        Managers.Instance.UIMananger.ClosePopup();
    }

    private void OnClick_Retry()
    {
        SoundManager.Instance.PlayButtonSound();
        GameManager.Instance.RetryStage();
        Managers.Instance.UIMananger.ClosePopup();
    }
}
