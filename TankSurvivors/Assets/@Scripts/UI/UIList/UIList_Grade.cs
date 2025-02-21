using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIList_Grade : UI_Base
{
    #region Enum_UI
    enum eGameObject
    {
        Image_Grade1,
        Image_Grade2,
        Image_Grade3,
        Image_Grade4,
        Image_Grade5
    }
    #endregion

    List<GameObject> _objectGradeList = new List<GameObject>();
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        // Bind
        BindOjbect(typeof(eGameObject));

        // Get
        for(int i =0; i < Define.MAX_SKILL_LEVEL; i++)
        {
            eGameObject gradeEnum = (eGameObject)(i);
            _objectGradeList.Add(GetObject((int)gradeEnum));
        }

        for(int i =0; i < _objectGradeList.Count; i++)
        {
            _objectGradeList[i].SetActive(false);
        }

        return true;
    }

    public void SetGrade(int skillLevel)
    {
        if(_init == false)
        {
            Init();
        }

        for(int i = 0; i < skillLevel; i++)
        {
            _objectGradeList[i].SetActive(true);
        }
    }
}
