using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup_StageSelect : UI_Base
{
    #region Enum UI
    private enum eGameObject
    {

    }
    private enum eButton
    {

    }
    private enum eImage
    {

    }

    private enum eText
    {

    }
    #endregion
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }
        return true;
    }

    public void Set()
    {
        if (_init == false)
            Init();


    }
}
