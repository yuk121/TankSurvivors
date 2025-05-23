using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UI_SceneTitle : UI_Scene
{
    #region Enum UI
    private enum eGameObject
    {
        Image_DownloadProgressBarBg
    }

    private enum eButton
    {
        Button_Start
    }
    private enum eImage
    {
        Image_TitleLogo,
        Image_TotuchToStart,
        Image_DownloadProgressBar
    }

    private enum eText
    {
        Text_ProcessState,
        Text_TouchToStart
    }
    #endregion
    private GameObject _downloadProgressBg = null;
    private Button _btnStart = null;
    private Image _imgTouchToStart = null;
    private Image _imgDownloadProgressBar = null;
    private TMP_Text _txtTouchToStart = null;
    private TMP_Text _txtProcessState = null;
    private GameObject _touchToStart = null;

    private bool _bWait= true;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }
        _sceneState = GameManager.Instance.GetSceneState();

        // Bind
        BindObject(typeof(eGameObject));
        BindButton(typeof(eButton));
        BindImage(typeof(eImage));
        BindText(typeof(eText));

        // Get
        _downloadProgressBg = GetObject((int)eGameObject.Image_DownloadProgressBarBg);
        _btnStart = GetButton((int)eButton.Button_Start);
        _imgTouchToStart = GetImage((int)eImage.Image_TotuchToStart);
        _imgDownloadProgressBar = GetImage((int)eImage.Image_DownloadProgressBar);

        _txtTouchToStart = GetText((int)eText.Text_TouchToStart);
        _txtProcessState = GetText((int)eText.Text_ProcessState);

        //
        _downloadProgressBg.SetActive(false);

        _btnStart.interactable = false;
        _btnStart.onClick.AddListener(OnClick_Start);

        _touchToStart = _imgTouchToStart.gameObject;
        _touchToStart.SetActive(false);
        _imgTouchToStart.gameObject.SetActive(false);
        _imgDownloadProgressBar.fillAmount = 0f;
       
        return true;
    }

    private void ShowTouchToStart()
    {
        // 프로세스 텍스트 비활성화
        _txtProcessState.gameObject.SetActive(false);
        // 스타티 버튼 이미지 활성화
        _imgTouchToStart.gameObject.SetActive(true);

        // 버튼 활성화
        _btnStart.interactable = true;
        _touchToStart.SetActive(true);

        // Tween 
        _imgTouchToStart.DOFade(0f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        _txtTouchToStart.DOFade(0f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
    }

    private void OnClick_Start()
    {
        _imgTouchToStart.DOKill();
        GameManager.Instance.StartGame();
    }

    private void Update()
    {
        if (GameManager.Instance == null)
            return;

        if (Managers.Instance.ResourceManager.IsDownloadStart == true)
        {
            if (_downloadProgressBg.activeSelf == false)
                _downloadProgressBg.SetActive(true);

            _imgDownloadProgressBar.fillAmount = GameManager.Instance.DownloadProgressValue;
        }
        else
        {
            if (_downloadProgressBg.activeSelf == true)
                _downloadProgressBg.SetActive(false);
        }

        if(GameManager.Instance.IsStartProcessEnd == true && _bWait == true)
        {
            _bWait = false;
            ShowTouchToStart();
        }
        else
        {
            _txtProcessState.text = GameManager.Instance.ProcessState;
        }
    }
}
