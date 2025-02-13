using System.Collections;
using System.Collections.Generic;

public class InGameLevelData
{
    public int level;
    public int cumulativeExp;
}
public class DataTableInGameLevel
{
    private List<InGameLevelData> _dataList = new List<InGameLevelData>();
    public List<InGameLevelData> DataList { get => _dataList; }
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

        if (dataText == null || dataText.Length < 1)
            return;

        TableLoader loader = TableLoader.Instance;
        loader.LoadTable(dataText);

        InGameLevelData inGameLevelData;

        for (int i = 0; i < loader.GetLength(); i++)
        {
            inGameLevelData = new InGameLevelData();

            inGameLevelData.level = loader.GetInt("Level", i);
            inGameLevelData.cumulativeExp = loader.GetInt("CumulativeExp", i);

            _dataList.Add(inGameLevelData);
        }

        loader.Clear();
    }

    public int GetNextLevelRequiredExp(int curLevel)
    {
        int requiredExp = 0;

        if (_dataList[_dataList.Count-1].level == curLevel)
        {
           return requiredExp =  99999999;
        }

        foreach(var data in _dataList) 
        {
            if(data.level == curLevel+1)
            {
                requiredExp = data.cumulativeExp;
                break;
            }
        }

        return requiredExp;
    }
}