using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPopup_FileDownload : UI_Popup
{
    #region Enum UI
    private enum eText
    {
        Text_FileCapacity
    }
    private enum eButton
    {
        Button_OK,
        Button_Cancel,
    }

    #endregion

    private Action _callback = null;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        // Bind 
        BindButton(typeof(eButton));
        BindText(typeof(eText));

        GetButton((int)eButton.Button_OK).onClick.AddListener(OnClick_OK);
        GetButton((int)eButton.Button_Cancel).onClick.AddListener(OnClick_Cancel);

        return false;
    }

    public void Set(float fileCapacity,Action pCallback)
    {
        if (_init == false)
            Init();

        GetText((int)eText.Text_FileCapacity).text = $"{fileCapacity} MB";
        _callback = pCallback;
    }
    private void OnClick_OK()
    {
        if (_callback != null)
            _callback.Invoke();
      
        ClosePopup();
    }

    private void OnClick_Cancel()
    {
        ClosePopup();
        Application.Quit();
    }

}
