using System.Collections;
using System.Collections.Generic;

public class UserLevelData
{
    public int userLevel;
    public int userCumulativeExp;
}
public class DataTableUser
{
    private List<UserLevelData> _dataList = new List<UserLevelData>();
    public List<UserLevelData> DataList { get => _dataList; }
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

        UserLevelData inGameLevelData;

        for (int i = 0; i < loader.GetLength(); i++)
        {
            inGameLevelData = new UserLevelData();

            inGameLevelData.userLevel = loader.GetInt("UserLevel", i);
            inGameLevelData.userCumulativeExp = loader.GetInt("UserCumulativeExp", i);

            _dataList.Add(inGameLevelData);
        }

        loader.Clear();
    }

    public int GetNextLevelRequiredExp(int curLevel)
    {
        int requiredExp = 0;

        if (_dataList[_dataList.Count - 1].userLevel == curLevel)
        {
            return requiredExp = 99999999;
        }

        foreach (var data in _dataList)
        {
            if (data.userLevel == curLevel + 1)
            {
                requiredExp = data.userCumulativeExp;
                break;
            }
        }

        return requiredExp;
    }

    public int GetLevelByExp(int exp)
    {
        int level = -1;

        if(exp <= 0)
        {
            return 1;
        }

        foreach(var data in _dataList)
        {
            if(exp < data.userCumulativeExp)
            {
                return data.userLevel - 1;
            }
        }

        if (_dataList[_dataList.Count - 1].userCumulativeExp >= exp)
        {
            level = _dataList[_dataList.Count - 1].userLevel;
            return level;
        }

        return level;
    }
}