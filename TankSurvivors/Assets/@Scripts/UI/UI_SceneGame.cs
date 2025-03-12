using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_SceneGame : UI_Scene
{
    #region UI Enum
    enum eGameObject
    {
        UI_HpBoss
    }

    enum eButton
    {
        Button_Puase
    }

    enum eImage
    {
        Image_Exp,
        Image_Hp,
        Image_HpBoss
    }

    enum eText
    {
        Text_Hp,
        Text_Exp,
        Text_Level,
        Text_Time,
        Text_KillCount
    }
    #endregion

    // GameObject
    private GameObject _uiHpBoss;
    // Button
    private Button _btnPause;
    // Image
    private Image _imgExp;
    private Image _imgHp;
    private Image _imgHpBoss;
    // Text Mesh Pro
    private TMP_Text _txtHp;
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

        _sceneState = GameManager.Instance.GetSceneState();
        Managers.Instance.UIMananger.SetSceneInfo(this);

        // Bind
        BindObject(typeof(eGameObject));
        BindButton(typeof(eButton));
        BindText(typeof(eText));    
        BindImage(typeof(eImage));

        // Get
        _uiHpBoss = GetObject((int)eGameObject.UI_HpBoss);

        _btnPause = GetButton((int)eButton.Button_Puase);

        _imgExp = GetImage((int)eImage.Image_Exp);
        _imgHp = GetImage((int)eImage.Image_Hp);
        _imgHpBoss = GetImage((int)eImage.Image_HpBoss);

        _txtHp = GetText((int)eText.Text_Hp);
        _txtExp = GetText((int)eText.Text_Exp);
        _txtLevel = GetText((int)eText.Text_Level);
        _txtTime = GetText((int)eText.Text_Time);
        _txtKillCount = GetText((int)(eText.Text_KillCount));

        // Event
        _btnPause.onClick.AddListener(OnClick_Pause);

        //
        _uiHpBoss.SetActive(false);
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
        float maxHp = GameManager.Instance.Player.CurMaxHp;

        _imgHp.fillAmount = curHp / maxHp;

        _txtHp.text = $"{(int)curHp} / {(int)maxHp}";
    }

    private void OnClick_Pause()
    {
       UIPopup_Pause popup = Managers.Instance.UIMananger.OpenPopupWithTween<UIPopup_Pause>(true);
    }

    private void SetHpBossUI()
    {
        MonsterController boss = Managers.Instance.ObjectManager.GetBoss();

        if (boss == null)
            return;

        _uiHpBoss.SetActive(true);
        _imgHpBoss.fillAmount = (float)boss.CurHp / boss.CreatureData.maxHp;
    }


    private void Update()
    {
        if(GameManager.Instance != null)
        {
            SetTime();
            SetKillCount();
            SetLevel();
            SetHp();
            SetHpBossUI();
        }
    }
}
