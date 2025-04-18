using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;

[FirestoreData]
public class UserData
{
    #region UserData Class List
    [FirestoreData]
    public class UserCurrency
    {
        [FirestoreProperty] public int gold { get; set; }
        [FirestoreProperty] public int cashFree { get; set; }
        [FirestoreProperty] public int cashPaid { get; set; }

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

    #endregion

    [FirestoreProperty] public string uid { get; set; }
    [FirestoreProperty] public int userChaterId { get; set; }
    [FirestoreProperty] public int userExp { get; set; }
    [FirestoreProperty] public int userStaminaMax { get; set; }
    [FirestoreProperty] public int userStaminaCurrent { get; set; }
    [FirestoreProperty] public UserCurrency userCurrency { get; set; }
    [FirestoreProperty] public Define.eLoginPlatform lastPlatform { get; set; }
    [FirestoreProperty] public long lastAccessTimestamp { get; set; }
    [FirestoreProperty] public long lastStaminaChangeTimestamp { get; set; }
    [FirestoreProperty] public List<bool> stageClearList { get; set; }       // 클리어 할 때마다 true값이 추가됨 (가변형)
    [FirestoreProperty] public int lastSelectStageLevel { get; set; }

    public void ClearUserData()
    {
        uid = "-1";
        userChaterId = 10001;
        userExp = 0;
        userStaminaMax = 100;
        userStaminaCurrent = 100;
        userCurrency = new UserCurrency();
        userCurrency.Clear();
        lastPlatform = Define.eLoginPlatform.None;
        lastAccessTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        lastStaminaChangeTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        stageClearList = new List<bool>();
        stageClearList.Clear();
        lastSelectStageLevel = 1;
    }
}
