using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_SceneLoading : UI_Scene
{
    #region Enum UI
    private enum eImage
    {
        Image_TitleLogo
    }

    [Header("Logo")]
    [SerializeField]
    private Vector3 _startPos = Vector3.zero;
    [SerializeField]
    private Vector3 _endPos = Vector3.zero; 

    private Image _imgTitleLogo = null;
    private RectTransform _rectTrans = null;
    #endregion
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        _sceneState = eGameManagerState.None;

        // Bind
        BindImage(typeof(eImage));

        // Get
        _imgTitleLogo = GetImage((int)eImage.Image_TitleLogo);

        //
        _rectTrans = _imgTitleLogo.rectTransform;
        _rectTrans.anchoredPosition = _startPos;
        _imgTitleLogo.color = new Color(_imgTitleLogo.color.r, _imgTitleLogo.color.g, _imgTitleLogo.color.b, 0f);

        return true;
    }
    public void ShowTitleLogo()
    {
        if (_init == false)
            Init();

        _rectTrans.DOAnchorPos(_endPos, 1f).SetEase(Ease.OutQuad);
        _imgTitleLogo.DOFade(1f, 1f).SetEase(Ease.OutQuad);
    }

    public IEnumerator HideTitleLogo()
    {
        yield return new WaitForSeconds(2f);
        _rectTrans.DOAnchorPos(_startPos, 1f).SetEase(Ease.InQuad);
        yield return _imgTitleLogo.DOFade(0f, 0.5f).SetEase(Ease.InQuad).WaitForCompletion();

        _rectTrans.DOKill();
    }
}
