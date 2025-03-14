using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserData
{
    #region UserData Class List
    public class UserCurrency
    {
        public int gold;
        public int cashFree;
        public int cashPaid;

        public int GetGold()
        {
            return gold;
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

    private string _uid;
    private int _userExp;
    private int _userStaminaMax;
    private int _userStaminaCurrent;
    public UserCurrency _userCurrency = new UserCurrency();
    public string _lastPlatform;
    public string _lastAccessDate;
    public UserOption _userOption = new UserOption();
    public List<bool> _stageClearList = new List<bool>();

    public void ClearUserData()
    {
        _uid = "-1";
        _userExp = 0;
        _userStaminaMax = 100;
        _userStaminaCurrent = 100;
        _userCurrency.Clear();
        _lastPlatform = Define.eLoginPlatform.None.ToString();
        _lastAccessDate = DateTime.Now.ToString();
        _userOption.Clear();
        _stageClearList.Clear();
    }
}
