using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton <T> where T : class , new()
{
    // 정확히는 싱글톤이 아니다.
    static private T _instance = null;
    // 인스턴스의 프로퍼티
    static public T Instance
    {
        get
        {
            if (_instance == null)
                _instance = new T();

            return _instance;
        }
    }

    protected Singleton()
    {

    }
}
