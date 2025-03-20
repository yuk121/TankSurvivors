using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class UserDataManager
{
    private UserData _userData;
    public UserData UserData { get => _userData; }

    public void NewStartUser()
    {
        _userData = new UserData();
        _userData.ClearUserData();

        // UID 생성
        _userData._uid = Managers.Instance.OptionManager.CreateUID();
    }

    public int GetUserLevel()
    {
        int level = Managers.Instance.DataTableManager.UserTable.GetLevelByExp(_userData._userExp);

        return level;
    }

    public int GetLastSelectStage()
    {
        int stageLevel = 0;

        if(_userData._lastSelectStageLevel > 0)
        {
            stageLevel = _userData._lastSelectStageLevel;
            return stageLevel;
        }
        else // 선택한 스테이지가 없거나 새로운 스테이지를 깬 경우 깨야할 스테이지를 보여준다.
        {
            int last = 0;
            // 스테이지 1도 안 깬 경우
            if (_userData._stageClearList.Count == 0)
            {
                last = 1;
            }

            // _stageClearList는 깬 스테이지 수 만큼 리스트에 값이 들어가 있음
            last = _userData._stageClearList.Count + 1;
            stageLevel = last;
        }

        return stageLevel;
    }

    public bool CheckStamina(int requiredStamina)
    {
        if(_userData._userStaminaCurrent - requiredStamina < 0)
        {
            return false;
        }

        return true;
    }

    public void UseStamina(StageData stageData)
    {
        if(stageData == null)
        {
            Debug.LogWarning("StageData is Null !!!");
            return;
        }

        int requiredStamina = Define.STAGE_ENTER_STAMINA; // * ( 스테이지 난이도?) 

        _userData._userStaminaCurrent -= requiredStamina;
    }

    public void GetReward(StageData stageData)
    {
        bool isFirstClear = CheckFirstStageClear(stageData);

        // 첫번째 클리어인지 확인
        if (isFirstClear == true)
        {
            _userData._userCurrency.AddGold(stageData.firstClearRewardGold);
            _userData._userExp += stageData.firstClearRewardExp;
        }
        else
        {
            _userData._userCurrency.AddGold(stageData.clearRewardGold);
            _userData._userExp += stageData.clearRewardExp;
        }
    }

    // 현재 깬 스테이지에 스테이지 클리어 이력이 있는지 확인하는 메소드
    public void StageClear(StageData stage)
    {
        // 클리어 한 경우
        if(_userData._stageClearList.Count >= stage.stageIndex && _userData._stageClearList[stage.stageIndex-1] == true)
        {
            return;
        }
        else
        {
            _userData._stageClearList.Add(true);
            SaveUserData();
        }
    }

    private bool CheckFirstStageClear(StageData stageData)
    {
        // _stageClearList.Count는 깬 스테이지 수의 값을 가지고 있으며 stageIndex는 1부터 시작함
        if(_userData._stageClearList.Count >= stageData.stageIndex)
        {
            return false;
        }

        return true;
    }

    public void LogOutUser()
    {
        _userData._lastAccessTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public UserData LoadUserData()
    {
        UserData userData = null;
        // 서버 확인

        // 로컬 확인
        string path = Application.persistentDataPath + "/userdata.json";

        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            userData = JsonConvert.DeserializeObject<UserData>(json);

            // 불러온 유저 정보
            _userData = userData;

            return userData;
        }

        return userData;
    }

    public void SaveUserData()
    {
        // 서버 저장

        // 로컬 저장
        string path = Application.persistentDataPath + "/userdata.json";
        string json = JsonConvert.SerializeObject(_userData, Formatting.Indented);
        System.IO.File.WriteAllText(path, json);
    }
}
