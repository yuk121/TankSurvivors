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

    private List<UIElement_Stage> _stageList = new List<UIElement_Stage>();
    private UI_ContentSwipe _uiContentSwipe = null;
    private Button _btnRight;
    private Button _btnLeft;
    private bool _bWait = false;

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
        
        _stageList.Clear();
        
        return true;
    }

    public void Set()
    {
        if (_init == false)
            Init();

        _bWait = false;

        _stageList.Clear();
        List<StageData> stageDataList = Managers.Instance.DataTableManager.DataTableStage.DataList;

        if (_stageList.Count == 0)
        {
            // �������� ������ �� ��ŭ ����
            for (int i = 0; i < stageDataList.Count; i++)
            {
                UIElement_Stage stage = Managers.Instance.UIMananger.InstantiateUI<UIElement_Stage>(GetObject((int)eGameObject.Content_StageList).transform);
                stage.Set(stageDataList[i]);

                _stageList.Add(stage);
            }
        }
        else
        {
            // �������� ��Ȳ �ݿ�
            for (int i = 0; i < _stageList.Count; i++)
            {
                _stageList[i].Set(stageDataList[i]);
            }
        }
        

        // ���� ���� ������ ���������� ��������
        int pageIndex = Managers.Instance.UserDataManager.GetLastSelectStage() -1;

        _uiContentSwipe.Set(stageDataList.Count, CheckButtonActive);
        _uiContentSwipe.SetScrollPage(pageIndex);

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
        if (_bWait == true)
            return;

        _bWait = true;

        int currentStageIndex = _uiContentSwipe.GetCurrentPageIndex();

        // �������� ���� �������� Ȯ��       
        bool canAccess = _stageList[currentStageIndex].CanAccess();

        if(canAccess == false)
        {
            // ���� �Ұ����ϴٰ� �˾�â ����ֱ�
            UIPopup_NotificationBar popup = Managers.Instance.UIMananger.OpenPopup<UIPopup_NotificationBar>();
            popup.SetMessage("���� �Ұ�", () => 
            {
                _bWait = false; 
            });
        }
        else
        {
            Managers.Instance.UserDataManager.UserData._lastSelectStageLevel = currentStageIndex + 1;
            Managers.Instance.UserDataManager.SaveUserData();

            Managers.Instance.UIMananger.ClosePopup();
        }
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
