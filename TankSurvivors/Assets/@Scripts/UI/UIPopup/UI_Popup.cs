using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Popup : UI_Base
{
    protected bool _bAllowEscapeKey = true;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        // 자동 해상도 대응
        ApplyAutoResolution();

        return true;
    }

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

    private void ApplyAutoResolution()
    {
        GameObject canvasObj = GameObject.FindGameObjectWithTag("UICanvas");

        if (canvasObj == null)
            return;

        Transform rootTrans = transform.Find("Root");

        CanvasScaler canvasScaler = canvasObj.GetComponent<CanvasScaler>();

        float scaleFactor = canvasScaler.referenceResolution.x / Screen.width;

        rootTrans.localScale = Vector3.one * scaleFactor;
    }

}
