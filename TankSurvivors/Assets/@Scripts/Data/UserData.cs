using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserData
{
    #region UserData Class List
    [System.Serializable]
    public class UserCurrency
    {
        public int gold;
        public int cashFree;
        public int cashPaid;

        public int GetGold()
        {
            return gold;
        }

        public void AddGold(int gold)
        {
            this.gold += gold;
        }

        public int GetCash()
        {
            return cashFree + cashPaid;
        }

        public void Clear()
        {
            gold = 0;
            cashFree = 0;
            cashPaid = 0;
        }
    }

    [System.Serializable]
    public class UserOption
    {
        public int soundMaster;
        public int soundBackground;
        public int soundEffect;

        public void Clear()
        {
            soundMaster = 100;
            soundBackground = 100;
            soundEffect = 100;
        }
    }
    #endregion

    public string _uid;
    public int _userChaterId = 0;
    public int _userExp;
    public int _userStaminaMax;
    public int _userStaminaCurrent;
    public UserCurrency _userCurrency = new UserCurrency();
    public Define.eLoginPlatform _lastPlatform;
    public long _lastAccessTimestamp;
    public long _lastStaminaChageTimestamp;
    public UserOption _userOption = new UserOption();
    public List<bool> _stageClearList = new List<bool>();       // Ŭ���� �� ������ true���� �߰��� (������)
    public int _lastSelectStageLevel;

    public void ClearUserData()
    {
        _uid = "-1";
        _userChaterId = 10001;
        _userExp = 0;
        _userStaminaMax = 100;
        _userStaminaCurrent = 100;
        _userCurrency.Clear();
        _lastPlatform = Define.eLoginPlatform.None;
        _lastAccessTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        _lastStaminaChageTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        _userOption.Clear();
        _stageClearList.Clear();
        _lastSelectStageLevel = 1;
    }
}
