using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager
{
    private Stack<UI_Base> _uiPopupStack = new Stack<UI_Base>();
    private UI_Scene _curScene = null;

    public Transform UICanvas
    {
        get 
        {
            GameObject uiCanvas = GameObject.FindWithTag("UICanvas");
            if(uiCanvas == null)
            {
                uiCanvas = new GameObject("UICanvas");
                uiCanvas.tag = "UICanvas";
            }

            return uiCanvas.transform;
        }
    }

    public void SetSceneInfo<T>(T uiScene) where T : UI_Scene
    {
        _curScene = uiScene;
    }
    /// <summary>
    /// 팝업창을 여는 메소드
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="bPause"> </param> 팝업창 오픈시 일시정지 여부 확인
    /// <returns></returns>
    public T OpenPopup<T>(bool bPause = false) where T : UI_Base
    {
        if(bPause == true)
        {
            GameManager.Instance.SetPause(true);
        }

        string name = typeof(T).Name;
        name = $"UIPrefab/UIPopup/{name}.prefab";

        Transform parent = null;

        if (_curScene == null)
        {
            parent = UICanvas;
        }
        else
        {
            parent = _curScene.GetComponent<Transform>();
        }

        GameObject go = Managers.Instance.ResourceManager.Instantiate(name, parent);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;

        T popup = Utils.GetOrAddComponent<T>(go);

        _uiPopupStack.Push(popup);

        return popup;
    }
    /// <summary>
    /// 팝업창을 트윈 효과를 주면서 여는 메소드
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ease"> 트윈 효과</param>
    /// <param name="bPause">팝업창 오픈시 일시정지 여부 확인</param>
    /// <returns></returns>
    public T OpenPopupWithTween<T>(Ease ease = Ease.OutBack, bool bPause = false) where T : UI_Base
    {
        T popup = OpenPopup<T>(bPause);

        Transform popupTransform = popup.GetComponent<Transform>();

        popupTransform.DOScale(1.2f, 0.2f) // 1.2배 커짐 (0.2초 동안)
            .SetEase(ease) // 효과
            .OnComplete(() =>
            {
                popupTransform.DOScale(1f, 0.1f) // 원래 크기(1배)로 돌아옴 (0.1초 동안)
                    .SetEase(Ease.InOutQuad);
            });

        return popup;
    }

    public void ClosePopup(UI_Base popup, bool bUnpause = true)
    {
        if (_uiPopupStack.Count < 1)
            return;

        if(_uiPopupStack.Peek() != popup)
        {
            Debug.LogError($"{this} : Close Popup Failed !!!");
        }

        ClosePopup(bUnpause);
    }

    public void ClosePopup(bool bUnpause = true)
    {
        if (_uiPopupStack.Count == 0)
            return;

        if (bUnpause == true && GameManager.Instance.Pause == true)
        {
            GameManager.Instance.SetPause(false);
        }

        UI_Base popup = _uiPopupStack.Pop();
        Managers.Instance.ResourceManager.Destroy(popup.gameObject);
        popup = null;
    }

    public void CloseAllPopup(bool bUnpause = true)
    {
        while (_uiPopupStack.Count > 0)
        {
            ClosePopup(bUnpause);
        }
    }

    public T InstantiateUI <T>(Transform parent) where T : UI_Base
    {
        string name = typeof(T).Name;

        if(name.Contains("UIList"))
        {
            name = $"UIPrefab/UIList/{name}.prefab";
        }
        else if(name.Contains("UIElement"))
        {
            name = $"UIPrefab/UIElement/{name}.prefab";
        }
        
        GameObject go = Managers.Instance.ResourceManager.Instantiate(name);
        go.transform.SetParent(parent);

        T ui = Utils.GetOrAddComponent<T>(go);

        return ui;
    }
}
