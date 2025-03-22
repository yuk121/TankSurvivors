using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPopup_Notification : UI_Popup
{
    #region Enum_UI

    private enum eGameObject
    {

    }

    private enum eButton
    {
        Button_OK,
        Button_Cancel
    }

    private enum eText
    {
        Text_Body
    }
    #endregion

    private Button _btnOK;
    private Button _btnCancel;
    private TMP_Text _txtBody;

    private Action _callBackOK = null;
    private Action _callBackCancel = null;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        // Bind
        BindButton(typeof(eButton));
        BindText(typeof(eText));

        // Get 
        _btnOK = GetButton((int)eButton.Button_OK);
        _btnCancel = GetButton((int)eButton.Button_Cancel);

        _txtBody = GetText((int)eText.Text_Body);

        //
        _btnOK.onClick.AddListener(OnClick_OK);
        _btnCancel.onClick.AddListener(OnClick_Cancel);

        return true;
    }

    public void SetMessageOKCancel(string message, Action pOKCallback = null, Action pCancelCallback = null)
    {
        if (_init == false)
            Init();

        string localization = Managers.Instance.DataTableManager.DataTableLocalization.GetLocalString(message);
        string noti = string.IsNullOrEmpty(localization) == true ? message : localization;

        _txtBody.text = $"{noti}";

        if (pOKCallback != null)
            _callBackOK = pOKCallback;

        if (pCancelCallback != null)
            _callBackCancel = pCancelCallback;

        _btnOK.gameObject.SetActive(true);
        _btnCancel.gameObject.SetActive(true);
    }

    public void SetMessageOK(string message, Action pOKCallback = null)
    {
        if (_init == false)
            Init();

        string localization = Managers.Instance.DataTableManager.DataTableLocalization.GetLocalString(message);
        string noti = string.IsNullOrEmpty(localization) == true ? message : localization;

        _txtBody.text = $"{noti}";

        if (pOKCallback != null)
            _callBackOK = pOKCallback;

        _btnOK.gameObject.SetActive(true);
        _btnCancel.gameObject.SetActive(false);
    }

    private void OnClick_OK()
    {
        SoundManager.Instance.PlayButtonSound();

        if (_callBackOK != null)
            _callBackOK.Invoke();

        Managers.Instance.UIMananger.ClosePopup();
    }

    private void OnClick_Cancel()
    {
        SoundManager.Instance.PlayButtonSound();

        if (_callBackCancel != null)
            _callBackCancel.Invoke();

        Managers.Instance.UIMananger.ClosePopup();
    }


}
