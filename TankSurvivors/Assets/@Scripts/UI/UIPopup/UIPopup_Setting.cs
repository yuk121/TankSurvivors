using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIPopup_Setting : UI_Popup
{
    #region Enum UI

    private enum eButton
    {
        Button_OK,
    }

    private enum eSlider
    {
        Slider_MasterSound,
        Slider_BackgroundSound,
        Slider_EffectSound
    }
    #endregion

    private Slider _sliderMasterSound = null;
    private Slider _sliderBGM = null;
    private Slider _sliderSFX = null;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        //Bind
        BindButton(typeof(eButton));
        BindSlider(typeof(eSlider));

        // Get
        GetButton((int)eButton.Button_OK).onClick.AddListener(OnClick_OK);

        _sliderMasterSound = GetSlider((int)eSlider.Slider_MasterSound);
        _sliderBGM = GetSlider((int)eSlider.Slider_BackgroundSound);
        _sliderSFX = GetSlider((int)eSlider.Slider_EffectSound);

        // Event
        BindEvent(_sliderMasterSound.gameObject, SaveSetting, type : Define.eUIEvent.PointerUp);
        BindEvent(_sliderBGM.gameObject, SaveSetting, type: Define.eUIEvent.PointerUp);
        BindEvent(_sliderSFX.gameObject, SaveSetting, type: Define.eUIEvent.PointerUp);

        // OnClick
        _sliderMasterSound.onValueChanged.AddListener(SetVolume_MasterSound);
        _sliderBGM.onValueChanged.AddListener(SetVolume_BGM);
        _sliderSFX.onValueChanged.AddListener(SetVolume_SFX);

        return true;
    }

    public void Set()
    {
        if (_init == false)
            Init();

        // 유저 사운드 정보 불러오기
        _sliderMasterSound.value = Managers.Instance.OptionManager.LocalData._userOption.soundMaster;
        _sliderBGM.value = Managers.Instance.OptionManager.LocalData._userOption.soundBackground;
        _sliderSFX.value = Managers.Instance.OptionManager.LocalData._userOption.soundEffect;
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

    private void OnClick_OK()
    {
        SoundManager.Instance.PlayButtonSound();
        Managers.Instance.UIMananger.ClosePopup();
    }
}

