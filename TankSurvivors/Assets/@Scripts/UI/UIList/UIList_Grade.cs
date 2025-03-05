using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; 

public class UIList_Grade : UI_Base
{
    #region Enum_UI
    enum eImage
    {
        Image_Grade1,
        Image_Grade2,
        Image_Grade3,
        Image_Grade4,
        Image_Grade5
    }
    #endregion

    private int _skillLevel = 0;

    List<Image> _objectGradeList = new List<Image>();
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        // Bind
        BindImage(typeof(eImage));

        // Get
        for(int i =0; i < Define.MAX_SKILL_LEVEL; i++)
        {
            eImage gradeEnum = (eImage)(i);
            _objectGradeList.Add(GetImage((int)gradeEnum));
        }

        for(int i =0; i < _objectGradeList.Count; i++)
        {
            _objectGradeList[i].gameObject.SetActive(false);
        }

        return true;
    }

    public void SetGrade(int skillLevel)
    {
        if(_init == false)
        {
            Init();
        }

        _skillLevel = skillLevel;

        for (int i = 0; i < _skillLevel; i++)
        {
            _objectGradeList[i].gameObject.SetActive(true);
        }

        // 다음 레벨 보여줌
        if (skillLevel < Define.MAX_SKILL_LEVEL)
        {
            _objectGradeList[_skillLevel].gameObject.SetActive(true);
            _objectGradeList[_skillLevel].GetComponent<Image>().DOFade(0f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        }
    }

    private void OnDisable()
    {
        if (_objectGradeList.Count > 0)
            _objectGradeList[_skillLevel].DOKill();
    }
}
