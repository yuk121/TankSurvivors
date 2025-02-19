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

    public T OpenPopup<T>(string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(T).Name;
            name = $"UIPrefab/UIPopup/{name}.prefab";
        }

        Transform parent = null;
        
        if(_curScene == null)
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

    public T OpenPopupWithTween<T>(string name = null, Ease ease = Ease.OutBack) where T : UI_Base
    {
        T popup = OpenPopup<T>();

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

    public void ClosePopup(UI_Base popup)
    {
        if (_uiPopupStack.Count < 1)
            return;

        if(_uiPopupStack.Peek() != popup)
        {
            Debug.LogError($"{this} : Close Popup Failed !!!");
        }

        ClosePopup();
    }

    public void ClosePopup()
    {
        if (_uiPopupStack.Count == 0)
            return;

        UI_Base popup = _uiPopupStack.Pop();
        Managers.Instance.ResourceManager.Destroy(popup.gameObject);
        popup = null;
    }

    public void CloseAllPopup()
    {
        while (_uiPopupStack.Count > 0)
        {
            ClosePopup();
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

        T popup = Utils.GetOrAddComponent<T>(go);

        _uiPopupStack.Push(popup);

        return popup;
    }
}
