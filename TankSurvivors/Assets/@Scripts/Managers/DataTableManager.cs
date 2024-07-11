using System.Collections;
using System.Collections.Generic;
using UGS;
using UnityEngine;

public class DataTableManager : MonoBehaviour
{
    // Start is called before the first frame update
    public void Init()
    {
        UnityGoogleSheet.LoadAllData();
    }
}

