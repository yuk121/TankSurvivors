using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup_Pause : UI_Popup
{
    #region Enum UI
    private enum eGameObject
    {
        Image_ActiveSkills,
        Image_SupportSkills,
        UIElement_SkillSlot,
    }

    private enum eButton
    {
        Button_Continue,
        Button_Lobby
    }

    private  enum eSlider
    {
        Slider_MasterSound,
        Slider_BackgroundSound,
        Slider_EffectSound
    }
    #endregion

    private GameObject _skillSlotPrefab = null;

    private Slider _sliderMasterSound = null;
    private Slider _sliderBGM = null;
    private Slider _sliderSFX = null;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        // Bind
        BindObject(typeof(eGameObject));
        BindButton(typeof(eButton));    
        BindSlider(typeof(eSlider));

        // Get
        //_btnContinue.onClick.RemoveAllListeners();
        GetButton((int)eButton.Button_Continue).onClick.AddListener(OnClick_Continue);
        //_btnLobby.onClick.RemoveAllListeners();
        GetButton((int)eButton.Button_Lobby).onClick.AddListener(OnClick_Lobby);

        if (_skillSlotPrefab == null)
           _skillSlotPrefab = GetObject((int)eGameObject.UIElement_SkillSlot);
        
        _skillSlotPrefab.SetActive(false);

        _sliderMasterSound = GetSlider((int)eSlider.Slider_MasterSound);
        _sliderBGM = GetSlider((int)eSlider.Slider_BackgroundSound);
        _sliderSFX = GetSlider((int)eSlider.Slider_EffectSound);

        // Event
        BindEvent(_sliderMasterSound.gameObject, SaveSetting, type: Define.eUIEvent.PointerUp);
        BindEvent(_sliderBGM.gameObject, SaveSetting, type: Define.eUIEvent.PointerUp);
        BindEvent(_sliderSFX.gameObject, SaveSetting, type: Define.eUIEvent.PointerUp);

        // OnClick
        _sliderMasterSound.onValueChanged.AddListener(SetVolume_MasterSound);
        _sliderBGM.onValueChanged.AddListener(SetVolume_BGM);
        _sliderSFX.onValueChanged.AddListener(SetVolume_SFX);

        // 
        SetSound();
        SetOwnedSkillList();

        return true;
    }

    private void SetSound()
    {
        // 유저 사운드 정보 불러오기
        _sliderMasterSound.value = Managers.Instance.UserDataManager.UserData._userOption.soundMaster;
        _sliderBGM.value = Managers.Instance.UserDataManager.UserData._userOption.soundBackground;
        _sliderSFX.value = Managers.Instance.UserDataManager.UserData._userOption.soundEffect;
    }

    private void SetOwnedSkillList()
    {
        if (GameManager.Instance == null || GameManager.Instance.Player == null)
            return;

        PlayerController player = GameManager.Instance.Player;

        List<ActionSkill> actionSkillList = player.GetActionSkillList().Where(skill => skill.CurSkillLevel > 0).ToList();
        List<SupportSkill>  supportSkillList = player.GetSupportSkillList().Where(skill => skill.CurSkillLevel > 0).ToList();

        for(int i = 0; i < actionSkillList.Count; i++)
        {
            UIElement_SkillSlot slot = Managers.Instance.UIMananger.InstantiateUI<UIElement_SkillSlot>(GetObject((int)eGameObject.Image_ActiveSkills).transform);

            slot.SetSkillSlot(actionSkillList[i]);
        }

        for (int i = 0; i < supportSkillList.Count; i++)
        {
            UIElement_SkillSlot slot = Managers.Instance.UIMananger.InstantiateUI<UIElement_SkillSlot>(GetObject((int)eGameObject.Image_SupportSkills).transform);

            slot.SetSkillSlot(supportSkillList[i]);
        }
    }

    private void SetVolume_MasterSound(float value)
    {
        SoundManager.Instance.SetVolume_MasterSound(value);
    }

    private void SetVolume_BGM(float value)
    {
        SoundManager.Instance.SetVolume_BGM(value);
    }

    private void SetVolume_SFX(float value)
    {
        SoundManager.Instance.SetVolume_SFX(value);
    }

    private void SaveSetting()
    {
        SoundManager.Instance.SaveSoundSetting();
    }

    private void OnClick_Lobby()
    {
        SoundManager.Instance.PlayButtonSound();
        GameManager.Instance.GoLobby();
        Managers.Instance.UIMananger.ClosePopup();
    }

    private void OnClick_Continue()
    {
        SoundManager.Instance.PlayButtonSound();
        Managers.Instance.UIMananger.ClosePopup();
    }
}
