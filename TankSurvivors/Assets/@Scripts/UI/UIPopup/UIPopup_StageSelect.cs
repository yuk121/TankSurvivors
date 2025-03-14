using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup_StageSelect : UI_Base
{
    #region Enum UI

    private enum eGameObject
    {
        Content_StageList,
        UIElement_Stage
    }

    private enum eButton
    {
        Button_Right,
        Button_Left,
        Button_StageSelect,
        Button_Back
    }
    #endregion

    private UI_ContentSwipe _uiContentSwipe = null;
    private Button _btnRight;
    private Button _btnLeft;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        // Bind
        BindObject(typeof(eGameObject));
        BindButton(typeof(eButton));

        //Get
        _uiContentSwipe = GetObject((int)eGameObject.Content_StageList).GetComponent<UI_ContentSwipe>();

        GetObject((int)eGameObject.UIElement_Stage).SetActive(false);

        _btnRight = GetButton((int)eButton.Button_Right);
        _btnLeft = GetButton((int)eButton.Button_Left);
        GetButton((int)eButton.Button_StageSelect).onClick.AddListener(OnClick_StageSelect);
        GetButton((int)eButton.Button_Back).onClick.AddListener(OnClick_Back);

        //
        _btnRight.onClick.AddListener(OnClick_StageRight);
        _btnLeft.onClick.AddListener(OnClick_StageLeft);
        return true;
    }

    public void Set()
    {
        if (_init == false)
            Init();

        List<StageData> stageDataList = Managers.Instance.DataTableManager.DataTableStage.DataList;

        // �������� ������ �� ��ŭ ����
        for(int i =0; i < stageDataList.Count; i++)
        {
            UIElement_Stage stage = Managers.Instance.UIMananger.InstantiateUI<UIElement_Stage>(GetObject((int)eGameObject.Content_StageList).transform);
            stage.Set(stageDataList[i]);
        }

        // ���� ���� ������ ���������� ��������
        _uiContentSwipe.Set(stageDataList.Count, CheckButtonActive);
        _uiContentSwipe.SetScrollPage(0);

        CheckButtonActive();
    }

    private void OnClick_StageRight()
    {
        _uiContentSwipe.SwipeRight();
        CheckButtonActive();
    }
    private void OnClick_StageLeft()
    {
        _uiContentSwipe.SwipeLeft();
        CheckButtonActive();
    }

    private void OnClick_StageSelect()
    {
        Managers.Instance.UIMananger.ClosePopup();
    }

    private void CheckButtonActive()
    {
        if (_uiContentSwipe == null)
            return;

        if(_uiContentSwipe.CheckFirstPage() == true)
        {
            // ù��° ���������� ��� ���� ��ư ��Ȱ��ȭ
            _btnRight.gameObject.SetActive(true);
            _btnLeft.gameObject.SetActive(false);
        }
        else if(_uiContentSwipe.CheckLastPage() == true)
        {
            // ������ ���������� ��� ������ ��ư ��Ȱ��ȭ
            _btnRight.gameObject.SetActive(false);
            _btnLeft.gameObject.SetActive(true);
        }
        else
        {
            _btnRight.gameObject.SetActive(true);
            _btnLeft.gameObject.SetActive(true);
        }
    }

    private void OnClick_Back()
    {
        Managers.Instance.UIMananger.ClosePopup();
    }
}
