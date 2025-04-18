using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIElement_Stage : UI_Base
{
    #region Enum UI


    private enum eImage
    {
        Image_Stage
    }

    private enum eText
    {
        Text_Stage
    }
    #endregion
    private StageData _stageData = null;
    private Image _imgStage = null;
    private bool _canAccess = false;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        // Bind
        BindImage(typeof(eImage));
        BindText(typeof(eText));

        // Get
        _imgStage = GetImage((int)eImage.Image_Stage);

        return true;
    }

    public void Set(StageData stageData)
    {
        if (_init == false)
            Init();

        _imgStage.sprite = Managers.Instance.ResourceManager.Load<Sprite>(stageData.stageIcon);
        GetText((int)eText.Text_Stage).text = Managers.Instance.DataTableManager.DataTableLocalization.GetLocalString(stageData.stageLocalizeName);

        _stageData = stageData;

        CheckAccessStage();
    }

    // 해당 스테이지에 들어갈 수 있는지 확인하는 메소드
    private void CheckAccessStage()
    {
        int stageIndex = _stageData.stageIndex;
        List<bool> stageClearList = Managers.Instance.UserDataManager.UserData.stageClearList;
        
        // 깨야할 스테이지까지 포함
        if(stageClearList.Count +1 >= stageIndex)
        {
            _canAccess = true;
            _imgStage.color = Color.white;
        }
        else
        {
            _canAccess = false;
            _imgStage.color = Color.gray;
        }
    }
    public bool CanAccess()
    {
        return _canAccess;
    }
}
