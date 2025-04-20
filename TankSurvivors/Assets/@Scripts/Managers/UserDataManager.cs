using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class UserDataManager
{
    private UserData _userData;
    public UserData UserData { get => _userData; set => _userData = value; }

    public void NewStartUser()
    {
        _userData = new UserData();
        _userData.ClearUserData();

#if UNITY_ANDROID && ! UNITY_EDITOR
        _userData.uid = APIManager.Instance.GetUserUid();
        Debug.Log($"New User ID : { _userData.uid}");
#elif UNITY_EDITOR
        // UID 생성
        _userData.uid = Managers.Instance.OptionManager.CreateUID();
#endif
    }

    public int GetUserLevel()
    {
        int level = Managers.Instance.DataTableManager.UserTable.GetLevelByExp(_userData.userExp);

        return level;
    }

    public int GetLastSelectStage()
    {
        int stageLevel = 0;
        int userSelectStage = _userData.lastSelectStageLevel;
        int userLastClearStage = _userData.stageClearList.Count;

        // 스테이지를 하나라도 깼고 마지막으로 선택한 스테이지가 마지막으로 클리어한 스테이지 레벨보다 작을때는 선택을 우선
        if (userSelectStage > 0 && userSelectStage <= userLastClearStage)
        {
            stageLevel = _userData.lastSelectStageLevel;
            return stageLevel;
        }
        else // 선택한 스테이지가 없거나 새로운 스테이지를 깬 경우 깨야할 스테이지를 보여준다.
        {
            int last = 0;
            // 스테이지 1도 안 깬 경우
            if (userLastClearStage == 0)
            {
                last = 1;
            }

            // _stageClearList는 깬 스테이지 수 만큼 리스트에 값이 들어가 있음
            last = userLastClearStage + 1;
            stageLevel = last;
        }

        return stageLevel;
    }

    public bool CheckStamina(int requiredStamina)
    {
        if(_userData.userStaminaCurrent - requiredStamina < 0)
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

        _userData.userStaminaCurrent -= requiredStamina;
        _userData.lastStaminaChangeTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public void RecoveryStamina(int value)
    {
        _userData.userStaminaCurrent += value;
        _userData.lastStaminaChangeTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public void GetReward(StageData stageData)
    {
        bool isFirstClear = CheckFirstStageClear(stageData);

        // 첫번째 클리어인지 확인
        if (isFirstClear == true)
        {
            _userData.userCurrency.AddGold(stageData.firstClearRewardGold);
            _userData.userExp += stageData.firstClearRewardExp;
        }
        else
        {
            _userData.userCurrency.AddGold(stageData.clearRewardGold);
            _userData.userExp += stageData.clearRewardExp;
        }
    }

    // 현재 깬 스테이지에 스테이지 클리어 이력이 있는지 확인하는 메소드
    public void StageClear(StageData stage)
    {
        // 클리어 한 경우
        if(_userData.stageClearList.Count >= stage.stageIndex && _userData.stageClearList[stage.stageIndex-1] == true)
        {
            return;
        }
        else
        {
            _userData.stageClearList.Add(true);
        }
    }

    private bool CheckFirstStageClear(StageData stageData)
    {
        // _stageClearList.Count는 깬 스테이지 수의 값을 가지고 있으며 stageIndex는 1부터 시작함
        if(_userData.stageClearList.Count >= stageData.stageIndex)
        {
            return false;
        }

        return true;
    }

    public void LogOutUser()
    {
        _userData.lastAccessTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public void LoadUserData(Action<UserData> pCallback = null)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // 서버 확인
        string uid = APIManager.Instance.GetUserUid();
        APIManager.Instance.GetUserData(uid, pCallback);
       
#elif UNITY_EDITOR
        UserData userData = null;
        // 로컬 확인
        string path = Application.persistentDataPath + "/userdata.json";

        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            userData = JsonConvert.DeserializeObject<UserData>(json);
        }

        if (pCallback != null)
            pCallback.Invoke(userData);
#endif
    }

    public void SaveUserData(Action pCallback = null)
    {
        if(_userData == null)
        {
            Debug.LogWarning($"[Userdata] UserData is Null");
            return;
        }

#if !UNITY_EDITOR
        // 서버 저장
        APIManager.Instance.SetUserData(_userData, pCallback);

#elif UNITY_EDITOR
        // 로컬 저장
        string path = Application.persistentDataPath + "/userdata.json";
        string json = JsonConvert.SerializeObject(_userData, Formatting.Indented);
        System.IO.File.WriteAllText(path, json);

        if (pCallback != null)
            pCallback.Invoke();
#endif
    }

    public void Request_StageClear(StageData stageData)
    {
        Debug.Log("[UserData] Request StageClear");
        // 유저 데이터 처리
        UseStamina(stageData);

        // 유저 재화 획득
        GetReward(stageData);

        // 스테이지 클리어 처리
        StageClear(stageData);

#if !UNITY_EDITOR
        APIManager.Instance.SetTransaction_StageClear(_userData);
#else
        // 에디터에서는 로컬 저장
        Managers.Instance.UserDataManager.SaveUserData();
#endif
    }

    public void Request_StageSelect()
    {
#if !UNITY_EDITOR
        APIManager.Instance.SetTransaction_StageSelect(_userData);
#else
        // 에디터에서는 로컬 저장
        Managers.Instance.UserDataManager.SaveUserData();
#endif
    }
}
