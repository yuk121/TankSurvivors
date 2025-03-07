using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UI_Base : MonoBehaviour
{
    protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();
    protected bool _init = false;

    public virtual bool Init()
    {
        if (_init)
            return false;

        _init = true;
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] names = Enum.GetNames(type);
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];    
        _objects.Add(typeof(T), objects);

        for(int i =0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
                objects[i] = Utils.FindChild(gameObject, names[i], true);

            else
                objects[i] = Utils.FindChild<T>(gameObject, names[i], true);

            if (objects[i] == null)
                Debug.Log($"Failed to bind {names[i]}");
        }
    }

    protected void BindObject(Type type) { Bind<GameObject>(type); }
    protected void BindImage (Type type) { Bind<Image>(type);}
    protected void BindText(Type type) { Bind<TMP_Text>(type);}
    protected void BindButton(Type type) { Bind<Button>(type);}
    protected void BindToggle(Type type) { Bind<Toggle>(type);}
    protected void BindSlider(Type type) { Bind<Slider>(type);} 

    protected T Get<T>(int index) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;

        if(_objects.TryGetValue(typeof(T), out objects) == false)
            return null;

        T get = objects[index] as T;
        return get;
    }
    
    protected GameObject GetObject(int index) { return Get<GameObject>(index); }
    protected Image GetImage(int index) { return Get<Image>(index); }
    protected TMP_Text GetText(int index) { return Get<TMP_Text>(index); }
    protected Button GetButton(int index) { return Get<Button>(index); }
    protected Toggle GetToggle(int index) { return Get<Toggle>(index); }
    protected Slider GetSlider(int index) { return Get<Slider>(index); }

    public void BindEvent(GameObject go, Action action = null, Action<BaseEventData> dragAction = null, Define.eUIEvent type = Define.eUIEvent.Click) 
    {
        UI_EventHandler eventHandler = Utils.GetOrAddComponent<UI_EventHandler>(go);

        switch(type)
        {
            case Define.eUIEvent.Click:
                eventHandler.OnClickHandler -= action;
                eventHandler.OnClickHandler += action;
                break;
            case Define.eUIEvent.Pressed:
                eventHandler.OnPressedHandler -= action;
                eventHandler.OnPressedHandler += action;
                break;
            case Define.eUIEvent.PointerDown:
                eventHandler.OnPointerDownHandler -= action;
                eventHandler.OnPointerDownHandler += action;
                break;
            case Define.eUIEvent.PointerUp:
                eventHandler.OnPointerUpHandler -= action;
                eventHandler.OnPointerUpHandler += action;
                break;
            case Define.eUIEvent.Drag:
                eventHandler.OnDragHandler -= dragAction;
                eventHandler.OnDragHandler += dragAction;
                break;
            case Define.eUIEvent.BeginDrag:
                eventHandler.OnBeginDragHandler -= dragAction;
                eventHandler.OnBeginDragHandler += dragAction;
                break; ;
            case Define.eUIEvent.EndDrag:
                eventHandler.OnEndDragHandler -= dragAction;
                eventHandler.OnEndDragHandler += dragAction;
                break;
        }
    }
}
