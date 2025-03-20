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

        // UID ����
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
        else // ������ ���������� ���ų� ���ο� ���������� �� ��� ������ ���������� �����ش�.
        {
            int last = 0;
            // �������� 1�� �� �� ���
            if (_userData._stageClearList.Count == 0)
            {
                last = 1;
            }

            // _stageClearList�� �� �������� �� ��ŭ ����Ʈ�� ���� �� ����
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

        int requiredStamina = Define.STAGE_ENTER_STAMINA; // * ( �������� ���̵�?) 

        _userData._userStaminaCurrent -= requiredStamina;
    }

    public void GetReward(StageData stageData)
    {
        bool isFirstClear = CheckFirstStageClear(stageData);

        // ù��° Ŭ�������� Ȯ��
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

    // ���� �� ���������� �������� Ŭ���� �̷��� �ִ��� Ȯ���ϴ� �޼ҵ�
    public void StageClear(StageData stage)
    {
        // Ŭ���� �� ���
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
        // _stageClearList.Count�� �� �������� ���� ���� ������ ������ stageIndex�� 1���� ������
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
        // ���� Ȯ��

        // ���� Ȯ��
        string path = Application.persistentDataPath + "/userdata.json";

        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            userData = JsonConvert.DeserializeObject<UserData>(json);

            // �ҷ��� ���� ����
            _userData = userData;

            return userData;
        }

        return userData;
    }

    public void SaveUserData()
    {
        // ���� ����

        // ���� ����
        string path = Application.persistentDataPath + "/userdata.json";
        string json = JsonConvert.SerializeObject(_userData, Formatting.Indented);
        System.IO.File.WriteAllText(path, json);
    }
}
