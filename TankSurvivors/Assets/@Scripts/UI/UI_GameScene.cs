using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_GameScene : UI_Scene
{
    #region UI Enum
    enum eButton
    {
        Button_Puase
    }

    enum eImage
    {
        Image_Exp,
        Image_Hp
    }

    enum eText
    {
        //Text_Wave,
        Text_Exp,
        Text_Level,
        Text_Time,
        Text_KillCount
    }
    #endregion

    // Button
    private Button _btnPause;
    // Image
    private Image _imgExp;
    private Image _imgHp;
    // Text Mesh Pro
    //private TMP_Text _txtWave;
    private TMP_Text _txtExp;
    private TMP_Text _txtLevel;
    private TMP_Text _txtTime;
    private TMP_Text _txtKillCount;

    private int _minutes = 0;
    private int _seconds = 0;

    private int _playerLevel = 0;
    private int _curExp = 0;
    private int _requiredExp = 0;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        _sceneType = Define.eSceneType.Game;
        Managers.Instance.UIMananger.SetSceneInfo(this);

        // Bind
        BindButton(typeof(eButton));
        BindText(typeof(eText));    
        BindImage(typeof(eImage));

        // Get
        _btnPause = GetButton((int)eButton.Button_Puase);

        _imgExp = GetImage((int)eImage.Image_Exp);
        _imgHp = GetImage((int)eImage.Image_Hp);
        //_txtWave = GetText((int)eText.Text_Wave);
        _txtExp = GetText((int)eText.Text_Exp);
        _txtLevel = GetText((int)eText.Text_Level);
        _txtTime = GetText((int)eText.Text_Time);
        _txtKillCount = GetText((int)(eText.Text_KillCount));

        // Event
        _btnPause.onClick.AddListener(OnClick_Pause);
       
        return true;
    }

    private void SetTime()
    {
        _minutes = (int)(GameManager.Instance.GameData.curTime / 60f);
        _seconds = (int)(GameManager.Instance.GameData.curTime % 60f);

        _txtTime.text = $"{_minutes:D2}:{_seconds:D2}";
    }

    private void SetKillCount()
    {
        _txtKillCount.text = $"{GameManager.Instance.GameData.killCount}";
    }

    private void SetLevel()
    {
        if (GameManager.Instance.Player == null)
            return;

        // Level
        _playerLevel = GameManager.Instance.Player.CurLevel;
        _txtLevel.text = $"{_playerLevel}";
     
        // Exp
        _curExp = GameManager.Instance.Player.CurExp;
        _requiredExp = Managers.Instance.DataTableManager.DataTableInGameLevel.GetNextLevelRequiredExp(_playerLevel);

        _txtExp.text = $"{_curExp} / {_requiredExp}";
        _imgExp.fillAmount = (float)_curExp / _requiredExp;
    }

    private void SetHp()
    {
        if (GameManager.Instance.Player == null)
            return;

        float curHp = GameManager.Instance.Player.CurHp;
        float maxHp = GameManager.Instance.Player.CreatureData.maxHp;

        _imgHp.fillAmount = curHp / maxHp;
    }

    private void OnClick_Pause()
    {

    }

    private void Update()
    {
        if(GameManager.Instance != null)
        {
            SetTime();
            SetKillCount();
            SetLevel();
            SetHp();
        }
    }
}
