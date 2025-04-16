using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class OptionManager 
{
    private LocalData _localData;
    public LocalData LocalData { get => _localData; }

    public void NewLocalData()
    {
        _localData = new LocalData();
        _localData.ClearLocalData();
    }

    public string CreateUID()
    {
        StringBuilder uid = new StringBuilder();
        int rand = 0;

        for(int i =0; i < 15; i++)
        {
            if(i == 0)
            {
                rand = Random.Range(1, 10);
            }
            else
            {
                rand = Random.Range(0, 10);
            }

            uid.Append(rand.ToString());
        }

        return uid.ToString();
    }

    public LocalData LoadLocalData()
    {
        LocalData localData = null;

        // 로컬 확인
        string path = Application.persistentDataPath + "/localData.json";
      
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            localData = JsonConvert.DeserializeObject<LocalData>(json);

            if (localData == null)
            {
                return null;
            }

            // 불러온 유저 정보
            _localData = localData;

            return localData;
        }
        return localData;
    }

    public void SaveLocalData()
    {
        string path = Application.persistentDataPath + "/localData.json";
        string json = JsonConvert.SerializeObject(_localData, Formatting.Indented);
        System.IO.File.WriteAllText(path, json);
    }
}
