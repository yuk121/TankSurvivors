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
    public int _userExp;
    public int _userStaminaMax;
    public int _userStaminaCurrent;
    public UserCurrency _userCurrency = new UserCurrency();
    public Define.eLoginPlatform _lastPlatform;
    public string _lastAccessDate;
    public UserOption _userOption = new UserOption();
    public List<bool> _stageClearList = new List<bool>();       // 클리어 할 때마다 true값이 추가됨 (가변형)
    public int _lastSelectStageLevel;

    public void ClearUserData()
    {
        _uid = "-1";
        _userExp = 0;
        _userStaminaMax = 100;
        _userStaminaCurrent = 100;
        _userCurrency.Clear();
        _lastPlatform = Define.eLoginPlatform.None;
        _lastAccessDate = DateTime.Now.ToString();
        _userOption.Clear();
        _stageClearList.Clear();
        _lastSelectStageLevel = 1;
    }

    // 0 일 경우 선택한 스테이지없음 , 새로운 스테이지 클리어시 초기화 해줌
    public void ClearStageSelect()
    {
        _lastSelectStageLevel = 0;
    }
}
