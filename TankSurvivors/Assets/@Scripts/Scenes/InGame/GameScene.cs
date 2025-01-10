using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : MonoBehaviour
{
    SpawningPools _spawnPools;

    // Start is called before the first frame update
    void Start()
    {
        Managers.Instance.ResourceManager.LoadAllAsyncWithLabel<Object>("preload", (key, count, totlaCount) =>
        {
            if (count == totlaCount)
            {
                Managers.Instance.DataTableManager.LoadData(() =>
                {
                    StartLoaded();
                });
            }
        });
    }
    private void StartLoaded()
    {
        int userCharId = 10001;
        var player = Managers.Instance.ObjectManager.Spawn<PlayerController>(new Vector3(0f,0.8f,0f), userCharId);
        Camera.main.GetComponent<CameraController>().Init(player.transform);

        // юс╫ц
        StageData stageInfo = Managers.Instance.DataTableManager.DataTableStage.Datas[0];
        Managers.Instance.GameManager.GameData.stageInfo = stageInfo;
        int stageIndex = Managers.Instance.GameManager.GameData.stageInfo.stageIndex;
     
        WaveData waveInfo = Managers.Instance.DataTableManager.DataTableWave.GetWaveData(stageIndex);
        Managers.Instance.GameManager.GameData.waveInfo = waveInfo;

        _spawnPools = Utils.GetOrAddComponent<SpawningPools>(gameObject);
        _spawnPools.StartSpawn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
