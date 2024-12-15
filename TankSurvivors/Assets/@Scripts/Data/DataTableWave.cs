using System.Collections;
using System.Collections.Generic;
public class WaveData
{
    public int stageIndex;
    public List<int> spawnMonsterId;
    public float spawnInterval;
    public int onceSpawnCount;
    public List<float> monsterSpawnRate;
    public List<int> spawnEliteId;
    public List<float> spawnEliteTime;
    public int spawnBossId;
    public float spawnBossTime;
    public float dropItemRate;
    public List<int> normalDropId;
    public int eliteDropId;
    public float redGemDropRate;
    public float greenGemDropRate;
    public float blueGemDropRate;
    public float purpleGemDropRate;
}
public class DataTableWave
{

    private List<WaveData> _datas = new List<WaveData>(); 
    public List<WaveData> Datas { get => _datas; }
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

        for (int i = 0; i < loader.GetLength(); i++)
        {
            WaveData waveData = new WaveData();

            waveData.stageIndex = loader.GetInt("StageIndex", i);
            waveData.spawnMonsterId = loader.GetList_Int("SpawnMonsterId", i);
            waveData.spawnInterval = loader.GetFloat("SpawnInterval", i);
            waveData.onceSpawnCount = loader.GetInt("OnceSpawnCount", i);
            waveData.monsterSpawnRate = loader.GetList_Float("MonsterSpawnRate", i);
            waveData.spawnEliteId = loader.GetList_Int("SpawnEliteId", i);
            waveData.spawnEliteTime = loader.GetList_Float("SpawnEliteTime", i);
            waveData.spawnBossId = loader.GetInt("SpawnBossId", i);
            waveData.spawnBossTime = loader.GetFloat("SpawnBossTime", i);
            waveData.dropItemRate = loader.GetFloat("DropItemRate", i);
            waveData.normalDropId = loader.GetList_Int("NormalDropId", i);
            waveData.eliteDropId = loader.GetInt("EliteDropId", i);
            waveData.redGemDropRate = loader.GetFloat("RedGemDropRate", i);
            waveData.greenGemDropRate = loader.GetFloat("GreenGemDropRate", i);
            waveData.blueGemDropRate = loader.GetFloat("BlueGemDropRate", i);
            waveData.purpleGemDropRate = loader.GetFloat("PurpleGemDropRate", i);

            _datas.Add(waveData);
        }

        loader.Clear();
    }

}
