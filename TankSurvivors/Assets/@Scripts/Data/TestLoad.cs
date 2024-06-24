using System.Collections;
using System.Collections.Generic;
using UGS;
using UnityEngine;

public class TestLoad : MonoBehaviour
{
    void Awake()
    {
        UnityGoogleSheet.LoadAllData();
        // UnityGoogleSheet.Load<DefaultTable.Data.Load>(); it's same!
        // or call DefaultTable.Data.Load(); it's same!
    }

    void Start()
    {
        foreach (var value in StageDataTable.StageInfo.StageInfoList)
        {
            Debug.Log(value.Stageindex + "," + value.StageLevel + "," + value.StageLocalizeName);
        }
    }
}
