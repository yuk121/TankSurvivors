using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LocalData
{
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

    public Define.eLoginPlatform _lastPlatform;
    public UserOption _userOption = new UserOption();

    public void ClearLocalData()
    {
        _lastPlatform = Define.eLoginPlatform.None;
        _userOption.Clear();
    }
}

