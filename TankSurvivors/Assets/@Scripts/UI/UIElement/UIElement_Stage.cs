using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIElement_Stage : UI_Base
{
    #region Enum UI


    private enum eImage
    {
        Image_Stage
    }

    private enum eText
    {
        Text_Stage
    }
    #endregion

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        // Bind
        BindImage(typeof(eImage));
        BindText(typeof(eText));
      
        return true;
    }

    public void Set(StageData stageData)
    {
        if (_init == false)
            Init();

        GetImage((int)eImage.Image_Stage).sprite = Managers.Instance.ResourceManager.Load<Sprite>(stageData.stageIcon);
        GetText((int)eText.Text_Stage).text = Managers.Instance.DataTableManager.DataTableLocalization.GetLocalString(stageData.stageLocalizeName);
    }
}
