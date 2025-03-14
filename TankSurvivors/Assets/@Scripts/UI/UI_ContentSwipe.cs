using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_ContentSwipe : MonoBehaviour
{
    [SerializeField]
    private Scrollbar _scrollBar = null;
    [SerializeField]
    private float _swipeTime = 0.2f;             // swipe �ð�
    [SerializeField]
    private float _swipeMinDis = 100f;         // swipe �ϴµ� �ʿ��� �ּ� �Ÿ�

    private float[] _scrollPageValue;             // page�� ��ġ��
    private float _valuePageDis;                  // �� page ������ �Ÿ�
    private int _currentPageIndex = 0;       // ���� page index
    private int _maxPageIndex = 0;           // ������ page index
    private float _startPosX = 0;                  // ��ġ ���� x ��ġ
    private float _endPosX = 0;                  // ��ġ �� x ��ġ
    private bool _isSwiping = false;             // swipe ���� üũ bool ��

    private Action _swipeEndCallback = null;            // swipe ������ ���� �� callback

    public void Set(int contentCount, Action pSwipeEndCallback = null)
    {
        _isSwiping = false;

        _scrollPageValue = new float[contentCount];

        _valuePageDis = 1f / (contentCount - 1);

        // �� �������� �Ÿ� ����
        for (int i = 0; i < contentCount; i++)
        {
            _scrollPageValue[i] = _valuePageDis * i;
        }

        _maxPageIndex = contentCount - 1;

        if (pSwipeEndCallback != null)
            _swipeEndCallback = pSwipeEndCallback;
    }

    public void SetScrollPage(int index)
    {
        _currentPageIndex = index;
        _scrollBar.value = _scrollPageValue[index];
    }
    // Update is called once per frame
    void Update()
    {
        if (_isSwiping == true)
            return;

#if UNITY_EDITOR
        if(Input.GetMouseButtonDown(0))
        {
            _startPosX = Input.mousePosition.x;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            _endPosX = Input.mousePosition.x;
            UpdateSwipe();
        }
#elif UNITY_ANDROID
       
        if (Input.touchCount == 1)
        {
            _startPosX = Input.mousePosition.x;
        }
        else if (Input.touchCount == 0)
        {
            _endPosX = Input.mousePosition.x;
            UpdateSwipe();
        }

#endif
    }

    private void UpdateSwipe()
    {
        // �ּ� �����̴� �Ÿ��� �ƴ� ��� ���� �������� ���ư���.
        if (Mathf.Abs(_endPosX - _startPosX) < _swipeMinDis)
        {
            DoMovePage(_currentPageIndex);
            return;
        }

        // ������ ��ġ - ���� ��ġ > 0 �� ��쿡�� �������� swipe , 0 ���� ������ ���������� swipe
        bool isLeft = _endPosX - _startPosX > 0 ? true : false;
        
        if(isLeft == true)
        {
            SwipeLeft();
        }
        else
        {
            SwipeRight();
        }
    }

    public void SwipeRight()
    {
        if (_currentPageIndex == _maxPageIndex)
            return;

        _currentPageIndex++;

        DoMovePage(_currentPageIndex);
    }

    public void SwipeLeft()
    {
        // ù��° �������� ���
        if (_currentPageIndex == 0)
            return;

        _currentPageIndex--;

        DoMovePage(_currentPageIndex);
    }

    private void DoMovePage(int currentPageIndex)
    {
        _isSwiping = true;

        DOTween.To(() => _scrollBar.value, x => _scrollBar.value = x, _scrollPageValue[currentPageIndex], _swipeTime).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                if (_swipeEndCallback != null)
                    _swipeEndCallback.Invoke();

                _isSwiping = false;
            });
    }

    public bool CheckFirstPage()
    {
        return _currentPageIndex == 0;
    }

    public bool CheckLastPage()
    {
        return _currentPageIndex == _maxPageIndex;
    }
}
