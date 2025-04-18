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

    private bool _bWait = false;

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

        _bWait = false;

        return true;
    }

    public void Set(Action pCallback)
    {
        if (_init == false)
            Init();

        // API Google Init
        APIManager.Instance.Init_Google();

        _callback = pCallback;
    }

    private void OnClick_GoogleLogin()
    {
        // 이중터치 방지
        if (_bWait == true)
            return;

        _bWait = true;

        // 구글 로그인 후 얻은 토큰으로 Firebase 인증
        APIManager.Instance.OnSignIn(Response_Success, Response_Fail);
    }

    private void Response_Success()
    {
        _bWait = false;

        if (_callback != null)
            _callback.Invoke();
    }

    private void Response_Fail()
    {
        _bWait = false;
    }

    private void OnClick_Cancel()
    {
        Application.Quit();
    }
}
