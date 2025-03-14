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
    private float _swipeTime = 0.2f;             // swipe 시간
    [SerializeField]
    private float _swipeMinDis = 100f;         // swipe 하는데 필요한 최소 거리

    private float[] _scrollPageValue;             // page의 위치값
    private float _valuePageDis;                  // 각 page 사이의 거리
    private int _currentPageIndex = 0;       // 현재 page index
    private int _maxPageIndex = 0;           // 마지막 page index
    private float _startPosX = 0;                  // 터치 시작 x 위치
    private float _endPosX = 0;                  // 터치 끝 x 위치
    private bool _isSwiping = false;             // swipe 진행 체크 bool 값

    private Action _swipeEndCallback = null;            // swipe 끝나고 실행 될 callback

    public void Set(int contentCount, Action pSwipeEndCallback = null)
    {
        _isSwiping = false;

        _scrollPageValue = new float[contentCount];

        _valuePageDis = 1f / (contentCount - 1);

        // 각 페이지별 거리 저장
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
        // 최소 움직이는 거리가 아닌 경우 원래 페이지로 돌아간다.
        if (Mathf.Abs(_endPosX - _startPosX) < _swipeMinDis)
        {
            DoMovePage(_currentPageIndex);
            return;
        }

        // 마지막 위치 - 시작 위치 > 0 인 경우에는 왼쪽으로 swipe , 0 보다 작으면 오른쪽으로 swipe
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
        // 첫번째 페이지인 경우
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
