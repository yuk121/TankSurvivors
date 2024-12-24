using System.Collections;
using System.Collections.Generic;

public class StageData
{
    public int stageIndex;
    public int stageLevel;
    public string stagePrefab;
    public string stageLocalizeName;
    public int firstClearRewardGold;
    public int firstClearRewardExp;
    public int clearRewardGold;
    public int clearRewardExp;
    public List<string> spawnMonster;
    public List<string> spawnElite;
    public string spawnBoss;
}
public class DataTableStage
{
    private List<StageData> _datas = new List<StageData>();
    public List<StageData> Datas { get => _datas; }
    private void Unload()
    {
        if (_datas != null && _datas.Count > 0)
        {
            _datas.Clear();
        }
    }
    public void DataLoad(byte[] dataText)
    {
        Unload();

        if (dataText == null || dataText.Length < 1)
            return;

        TableLoader loader = TableLoader.Instance;
        loader.LoadTable(dataText);
       
        StageData stageData;

        for (int i = 0; i < loader.GetLength(); i++)
        {
            stageData = new StageData();

            stageData.stageIndex = loader.GetInt("Stageindex", i);
            stageData.stageLevel = loader.GetInt("StageLevel", i);
            stageData.stagePrefab = loader.GetString("StagePrefab", i);
            stageData.stageLocalizeName = loader.GetString("StageLocalizeName", i);
            stageData.firstClearRewardGold = loader.GetInt("FirstClearRewardGold", i);
            stageData.firstClearRewardExp = loader.GetInt("FirstClearRewardExp", i);
            stageData.clearRewardGold = loader.GetInt("ClearRewardGold", i);
            stageData.clearRewardExp = loader.GetInt("ClearRewardExp", i);
            stageData.spawnMonster = loader.GetList_String("SpawnMonster", i);
            stageData.spawnElite = loader.GetList_String("SpawnElite", i);
            stageData.spawnBoss = loader.GetString("SpawnBoss", i);

            _datas.Add(stageData);
        }

        loader.Clear();
    }
}
