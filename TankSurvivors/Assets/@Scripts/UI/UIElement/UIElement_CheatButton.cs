using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIElement_CheatButton : MonoBehaviour
{
    private Button _btnCheat = null;
    private Action _callback = null;

    public void Init(Action pCallback)
    {
        _btnCheat = GetComponent<Button>();
        _btnCheat.onClick.AddListener(UseCheat);
    }

    private void UseCheat()
    {
        if (_callback != null)
            _callback.Invoke();
    }
}
