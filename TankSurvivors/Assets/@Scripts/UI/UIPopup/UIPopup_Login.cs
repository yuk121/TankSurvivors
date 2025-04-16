using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup_Login : UI_Popup
{
    #region Enum UI
    private enum eGameObject
    {

    }

    private enum eButton
    {
        Button_Google,
        Button_Cancel,
    }

    #endregion

    private Button _btnGoogleLogin;
    private Button _btnCancel;
    private Action _callback;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        // Bind 
        BindButton(typeof(eButton));

        // Get
        _btnGoogleLogin = GetButton((int)eButton.Button_Google);
        _btnCancel = GetButton((int)eButton.Button_Cancel);

        // 
        _btnGoogleLogin.onClick.AddListener(OnClick_GoogleLogin);
        _btnCancel.onClick.AddListener(OnClick_Cancel);

        return true;
    }

    public void Set(Action pCallback)
    {
        if (_init == false)
            Init();

        APIManager.Instance.Init();

        _callback = pCallback;
    }

    private void OnClick_GoogleLogin()
    {
        // 구글 로그인 후 얻은 토큰으로 Firebase 인증
        APIManager.Instance.OnGamesSignIn(_callback);
    }

    private void OnClick_Cancel()
    {
        Application.Quit();
    }
}
