using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIPopup_NotificationBar : UI_Base
{
    #region Enum UI
    private enum eText
    {
        Text_Notfication
    }

    private enum eImage
    {
        Image_Bg
    }

    private const float SHOW_POPUP_DURATION = 2f;
    private const float HIDE_POPUP_DURATION = 1f;
    private Image _imgBg;
    private TMP_Text _txtNotification;
    private Coroutine _corClosePopup;

    #endregion
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        // Bind
        BindImage(typeof(eImage));
        BindText(typeof(eText));

        // Get
        _imgBg = GetImage((int)eImage.Image_Bg);
        _txtNotification = GetText((int)eText.Text_Notfication);

        return true;
    }

    public void SetMessage(string message, Action pCallback = null)
    {
        if (_init == false)
            Init();

        _imgBg.color = new Color(_imgBg.color.r, _imgBg.color.g, _imgBg.color.b, 1f);

        string localization = Managers.Instance.DataTableManager.DataTableLocalization.GetLocalString(message);
        string noti = string.IsNullOrEmpty(localization) == true ? message : localization;
       
        _txtNotification.text = $"{noti}";

        if (_corClosePopup != null)
            StopCoroutine(_corClosePopup);

        _corClosePopup = StartCoroutine(CorClosePopup(pCallback));
    }

    private IEnumerator CorClosePopup(Action pCallback)
    {
        yield return new WaitForSeconds(SHOW_POPUP_DURATION);

        _imgBg.DOFade(0f, HIDE_POPUP_DURATION).SetEase(Ease.Linear);
        _imgBg.DOKill();

        if (pCallback != null)
            pCallback.Invoke();

        Managers.Instance.UIMananger.ClosePopup();
    }
}
