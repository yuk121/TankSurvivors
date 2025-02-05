using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    #region SimpleSingleTon
    public static GridManager Instance;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    #endregion
}
