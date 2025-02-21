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
    /// �˾�â�� ���� �޼ҵ�
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="bPause"> </param> �˾�â ���½� �Ͻ����� ���� Ȯ��
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
    /// �˾�â�� Ʈ�� ȿ���� �ָ鼭 ���� �޼ҵ�
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ease"> Ʈ�� ȿ��</param>
    /// <param name="bPause">�˾�â ���½� �Ͻ����� ���� Ȯ��</param>
    /// <returns></returns>
    public T OpenPopupWithTween<T>(Ease ease = Ease.OutBack, bool bPause = false) where T : UI_Base
    {
        T popup = OpenPopup<T>(bPause);

        Transform popupTransform = popup.GetComponent<Transform>();

        popupTransform.DOScale(1.2f, 0.2f) // 1.2�� Ŀ�� (0.2�� ����)
            .SetEase(ease) // ȿ��
            .OnComplete(() =>
            {
                popupTransform.DOScale(1f, 0.1f) // ���� ũ��(1��)�� ���ƿ� (0.1�� ����)
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
