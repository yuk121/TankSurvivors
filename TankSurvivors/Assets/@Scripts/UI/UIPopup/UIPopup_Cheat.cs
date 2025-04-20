using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eCheat
{
    PlayerLevelUp,
    MonsterAllKill,
    Timeslip,
    RefreshSkill,
    Max
}

public class UIPopup_Cheat : UI_Popup
{
    #region Enum UI
    private enum eGameObject
    {
        Content,
        UIElement_CheatButton
    }

    private enum eButton
    {
        Button_Close
    }
 
    #endregion

    private GameObject _cheatPrefab;
    private Transform _contentTrans;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        // Bind 
        BindObject(typeof(eGameObject));
        BindButton(typeof(eButton));

        //Get 
        _cheatPrefab = GetObject((int)eGameObject.UIElement_CheatButton);
        _cheatPrefab.SetActive(false);

        _contentTrans = GetObject((int)eGameObject.Content).transform;

        GetButton((int)eButton.Button_Close).onClick.AddListener(OnClick_Close);

        return false;
    }

    public void Set()
    {
        if (_init == false)
            Init();

        for(int i =0; i < (int)eCheat.Max; i++)
        {
            GameObject go = Instantiate(_cheatPrefab, _contentTrans);
            go.SetActive(true);

            UIElement_CheatButton cheatBtn = go.GetComponent<UIElement_CheatButton>();
            cheatBtn.Init((eCheat)i);
        }
    }

    private void OnClick_Close()
    {
        ClosePopup();
    }
}
