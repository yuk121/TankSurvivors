using StageDataTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningPools : MonoBehaviour
{
    Coroutine spawnCor = null;

    public bool SpawnStop { get; set; } = false;

    public void StartSpawn()
    {
        if (spawnCor != null) 
        {
            StopCoroutine(spawnCor);
        }

        spawnCor = StartCoroutine(SpwanCreatureCor());
    }


    IEnumerator SpwanCreatureCor()
    {     
        // 임시
        int curStageId = 1;
        WaveDataTable.Data waveInfo = null;
       
        // 웨이브 정보 필요
        foreach (var Data in WaveDataTable.Data.DataList)
        {
            if (Data.StageIndex == curStageId)
            {
                waveInfo = Data;
                break;
            }
        }

        while(true)
        {
            int spawnMonsterId = -1;

            if (waveInfo.MonsterSpawnRate.Count == 1)
            {
                for (int i = 0; i < waveInfo.OnceSpawnCount; i++)
                {
                    Vector3 playerPos = Managers.Instance.ObjectManager.Player.transform.position;
                    Vector3 spawnPos = Utils.GenerateMonsterSpwanPosition(playerPos, 20f, 25f);

                    spawnMonsterId = waveInfo.SpawnMonsterId[0];
                    Managers.Instance.ObjectManager.Spawn<MonsterController>(spawnPos, spawnMonsterId);
                }
                
                yield return new WaitForSeconds(waveInfo.SpawnInterval);
            }
            else
            {
                float totalRate = -1;

                for (int i = 0; i < waveInfo.MonsterSpawnRate.Count; i++)
                {
                    totalRate += waveInfo.MonsterSpawnRate[i];
                }
              
                for (int i = 0; i < waveInfo.OnceSpawnCount; i++)
                {
                    Vector3 playerPos = Managers.Instance.ObjectManager.Player.transform.position;
                    Vector3 spawnPos = Utils.GenerateMonsterSpwanPosition(playerPos, 20f, 25f);

                    // 여러 몬스터 중 하나 선택
                    float rand = Random.value * totalRate;

                    for (int n= 0; n < waveInfo.SpawnMonsterId.Count; n++)
                    {
                        rand -= waveInfo.MonsterSpawnRate[n];

                        if (rand <= 0)
                        {
                            spawnMonsterId = waveInfo.SpawnMonsterId[n];
                            break;
                        }
                    }

                    Managers.Instance.ObjectManager.Spawn<MonsterController>(spawnPos, spawnMonsterId);
                }
                
                yield return new WaitForSeconds(waveInfo.SpawnInterval);
            }
        }
    }
}
