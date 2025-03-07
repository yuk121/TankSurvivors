using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup_Pause : UI_Base
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
        
        // 
        SetOwnedSkillList();

        return true;
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

    private void OnClick_Lobby()
    {

    }

    private void OnClick_Continue()
    {
        Managers.Instance.UIMananger.ClosePopup();
    }
}
