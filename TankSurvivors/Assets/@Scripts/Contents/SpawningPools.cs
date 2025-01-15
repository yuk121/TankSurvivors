using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningPools : MonoBehaviour
{
    private Coroutine _corSpawn = null;

    public bool SpawnStop { get; set; } = false;

    public void StartSpawn()
    {
        if (_corSpawn != null) 
        {
            StopCoroutine(_corSpawn);
        }

        _corSpawn = StartCoroutine(SpwanCreatureCor());
    }


    IEnumerator SpwanCreatureCor()
    {     
        yield return null;

        // 임시
        int curStageId = GameManager.Instance.GameData.stageInfo.stageIndex;
        WaveData waveInfo = GameManager.Instance.GameData.waveInfo;
       
        while(true)
        {
            // 플레이어가 사망시 더이상 작동하지 않는다.
            if (GameManager.Instance.CheckPlayerAlive() == false)
                break;

            int spawnMonsterId = -1;
            int onceSpawnCount = waveInfo.onceSpawnCount;

            // 테스트용 (한마리만 소환)
            //*
            {
                onceSpawnCount = 1;

                if (Managers.Instance.ObjectManager.Monsters.Count > 0)
                    break;
            }
            /**/

            // 몬스터를 주기적으로 소환
            // 현재 스테이지에서 몬스터 종류가 하나 인 경우
            if (waveInfo.monsterSpawnRate.Count == 1)
            {
                for (int i = 0; i < onceSpawnCount; i++)
                {
                    Vector3 spawnPos = Utils.GetCamOutPos3D(Camera.main, 0.5f, 2f);

                    spawnMonsterId = waveInfo.spawnMonsterId[0];
                    Managers.Instance.ObjectManager.Spawn<MonsterController>(spawnPos, spawnMonsterId);
                }
                
                yield return new WaitForSeconds(waveInfo.spawnInterval);
            }
            else // 몬스터 종류가 여러 마리인 경우
            {
                float totalRate = -1;

                for (int i = 0; i < waveInfo.monsterSpawnRate.Count; i++)
                {
                    totalRate += waveInfo.monsterSpawnRate[i];
                }
              
                for (int i = 0; i < waveInfo.onceSpawnCount; i++)
                {
                    Vector3 spawnPos = Utils.GetCamOutPos3D(Camera.main, 0.5f, 3f);
                   
                    // 여러 몬스터 중 하나 선택
                    float rand = Random.value * totalRate;

                    for (int n= 0; n < waveInfo.spawnMonsterId.Count; n++)
                    {
                        rand -= waveInfo.monsterSpawnRate[n];

                        if (rand <= 0)
                        {
                            spawnMonsterId = waveInfo.spawnMonsterId[n];
                            break;
                        }
                    }

                    Managers.Instance.ObjectManager.Spawn<MonsterController>(spawnPos, spawnMonsterId);
                }
                
                yield return new WaitForSeconds(waveInfo.spawnInterval);
            }
        }
     /**/
    }
}
