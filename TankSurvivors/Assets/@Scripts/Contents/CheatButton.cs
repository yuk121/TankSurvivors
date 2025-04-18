using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheatButton : MonoBehaviour
{
    [SerializeField]
    private Button _btnCheat = null;

    private void Start()
    {
        _btnCheat.onClick.AddListener(OnClick_Cheat);
    }

    private void OnClick_Cheat()
    {
        UIPopup_Cheat popup =  Managers.Instance.UIMananger.OpenPopup<UIPopup_Cheat>();
        popup.Set();
    }
}
