using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationData
{
    public string localKey;
    public string Korean;
    public string English;
}

public class DataTableLocalization
{
    private List<LocalizationData> _dataList = new List<LocalizationData>();
    public List<LocalizationData> DataList { get => _dataList; }

    private void Unload()
    {
        if (_dataList != null && _dataList.Count > 0)
        {
            _dataList.Clear();
        }
    }
    public void DataLoad(byte[] dataText)
    {
        Unload();

        TableLoader loader = TableLoader.Instance;
        loader.LoadTable(dataText);

        LocalizationData localData;
        for (int i = 0; i < loader.GetLength(); i++)
        {
            localData = new LocalizationData();

            localData.localKey = loader.GetString("LocalKey", i);
            localData.Korean = loader.GetString("Korean", i);
            localData.English = loader.GetString("English", i);

            _dataList.Add(localData);
        }

        loader.Clear();
    }

    public string GetLocalString(string localKey)
    {
        string local = string.Empty;
        SystemLanguage currentLanguage = Application.systemLanguage;

        foreach(var data in _dataList)
        {
            if(data.localKey == localKey)
            {
                switch(currentLanguage)
                {
                    case SystemLanguage.English:
                        local = data.English;
                        break;

                    default:
                        local = data.Korean;
                        break;
                }

                break;
            }
        }

        return local;
    }

}
