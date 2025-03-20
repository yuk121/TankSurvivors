using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Popup : UI_Base
{
    protected bool _bAllowEscapeKey = true;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(Managers.Instance.UIMananger != null && _bAllowEscapeKey == true)
            {
                ClosePopup();
            }
        }
    }

    protected void ClosePopup()
    {
        Managers.Instance.UIMananger.ClosePopup();
    }

}
