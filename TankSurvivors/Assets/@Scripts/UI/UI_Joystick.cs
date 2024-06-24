using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Joystick : UI_Base
{
    enum eGameOjbect
    {
        JoystickBG,
        Handler
    }

    private GameObject _handler;
    private GameObject _joystickBG;
    private Vector2 _touchPos;
    private Vector2 _moveDir;
    private float _joystickRadius;

    public override bool init()
    {
        if(base.init() == false)
        {
            return false;
        }

        BindOjbect(typeof(eGameOjbect));
        _handler = GetObject((int)eGameOjbect.Handler);
        _joystickBG = GetObject((int)eGameOjbect.JoystickBG);

        BindEvent(gameObject, OnPointerDown, null, Define.eUIEvent.PointerDown);
        BindEvent(gameObject, OnPointerUp, null, Define.eUIEvent.PointerUp);
        BindEvent(gameObject, null, OnDrag, Define.eUIEvent.Drag);

        _handler.SetActive(false);
        _joystickBG.SetActive(false);

        _joystickRadius = _joystickBG.GetComponent<RectTransform>().sizeDelta.y / 2;
        return true;
    }

    #region Event
    public void OnPointerDown()
    {
        _handler.SetActive(true);
        _joystickBG.SetActive(true);

        _touchPos = Input.mousePosition;

        _handler.transform.position = _touchPos;
        _joystickBG.transform.position = _touchPos;
    }

    public void OnPointerUp() 
    {
        _handler.transform.position = _touchPos;
        _moveDir = Vector2.zero;

        Managers.Instance.ObjectManager.Player.MoveDir = _moveDir;

        _handler.SetActive(false);
        _joystickBG.SetActive(false);
    }

    public void OnDrag(BaseEventData eventData)
    {
        PointerEventData pointerEventData = eventData as PointerEventData;

        Vector2 touchDir = (pointerEventData.position - _touchPos);
        float moveDist = Mathf.Min(touchDir.magnitude, _joystickRadius);
        _moveDir = touchDir.normalized;

        Vector2 newPos = _touchPos + (_moveDir * moveDist);
        _handler.transform.position = newPos; // 조이스틱 위치 갱신

        Managers.Instance.ObjectManager.Player.MoveDir = _moveDir;
    }

    #endregion
}
