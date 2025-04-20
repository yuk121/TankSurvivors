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
        // UID ����
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

        // ���������� �ϳ��� ���� ���������� ������ ���������� ���������� Ŭ������ �������� �������� �������� ������ �켱
        if (userSelectStage > 0 && userSelectStage <= userLastClearStage)
        {
            stageLevel = _userData.lastSelectStageLevel;
            return stageLevel;
        }
        else // ������ ���������� ���ų� ���ο� ���������� �� ��� ������ ���������� �����ش�.
        {
            int last = 0;
            // �������� 1�� �� �� ���
            if (userLastClearStage == 0)
            {
                last = 1;
            }

            // _stageClearList�� �� �������� �� ��ŭ ����Ʈ�� ���� �� ����
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

        int requiredStamina = Define.STAGE_ENTER_STAMINA; // * ( �������� ���̵�?) 

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

        // ù��° Ŭ�������� Ȯ��
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

    // ���� �� ���������� �������� Ŭ���� �̷��� �ִ��� Ȯ���ϴ� �޼ҵ�
    public void StageClear(StageData stage)
    {
        // Ŭ���� �� ���
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
        // _stageClearList.Count�� �� �������� ���� ���� ������ ������ stageIndex�� 1���� ������
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
        // ���� Ȯ��
        string uid = APIManager.Instance.GetUserUid();
        APIManager.Instance.GetUserData(uid, pCallback);
       
#elif UNITY_EDITOR
        UserData userData = null;
        // ���� Ȯ��
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
        // ���� ����
        APIManager.Instance.SetUserData(_userData, pCallback);

#elif UNITY_EDITOR
        // ���� ����
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
        // ���� ������ ó��
        UseStamina(stageData);

        // ���� ��ȭ ȹ��
        GetReward(stageData);

        // �������� Ŭ���� ó��
        StageClear(stageData);

#if !UNITY_EDITOR
        APIManager.Instance.SetTransaction_StageClear(_userData);
#else
        // �����Ϳ����� ���� ����
        Managers.Instance.UserDataManager.SaveUserData();
#endif
    }

    public void Request_StageSelect()
    {
#if !UNITY_EDITOR
        APIManager.Instance.SetTransaction_StageSelect(_userData);
#else
        // �����Ϳ����� ���� ����
        Managers.Instance.UserDataManager.SaveUserData();
#endif
    }
}
