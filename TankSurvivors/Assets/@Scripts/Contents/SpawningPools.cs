using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningPools : MonoBehaviour
{
    private Coroutine _corSpawn = null;

    public bool _spawnStop = false;
    private float _gameTimeMin = 0f;

    public void StartSpawn()
    {
        if (_corSpawn != null) 
        {
            StopCoroutine(_corSpawn);
        }

        _corSpawn = StartCoroutine(SpawnCreatureCor());
    }

#if UNITY_EDITOR
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            _spawnStop = !_spawnStop;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            Vector3 spawnPos = Utils.GetRotatedCamOutRandPos3D(Camera.main);
            Managers.Instance.ObjectManager.Spawn<MonsterController>(spawnPos, 20001);
        }
#endif
    }
#endif

    IEnumerator SpawnCreatureCor()
    {     
        yield return null;

        // 임시
        int curStageId = GameManager.Instance.GameData.stageInfo.stageIndex;
        WaveData waveInfo = GameManager.Instance.GameData.waveInfo;
        
        int spawnMonsterId = -1;       
        int onceSpawnCount = waveInfo.onceSpawnCount;
        int EliteSpawnIndex = 0;

        while (true && !_spawnStop)
        {
            _gameTimeMin = GameManager.Instance.GameData.curTime / 60f;

            if (GameManager.Instance.Pause == true)
            {
                yield return new WaitForSeconds(waveInfo.spawnInterval);
                continue;
            }

            // 플레이어가 사망시 더이상 작동하지 않는다.
            if (GameManager.Instance.CheckPlayerAlive() == false)
                break;


            // 테스트용 (한마리만 소환)
            /*
            {
                onceSpawnCount = 1;

                if (Managers.Instance.ObjectManager.Monsters.Count > 0)
                    break;
            }
            /**/

            // 보스 소환
            if (_gameTimeMin >= waveInfo.spawnBossTime)
            {
                Vector3 spawnPos = Utils.GetRotatedCamOutRandPos3D(Camera.main);

                spawnMonsterId = waveInfo.spawnBossId;
                Managers.Instance.ObjectManager.Spawn<MonsterController>(spawnPos, spawnMonsterId);
                yield break;
            }
            else  // 보스 소환시 더 이상 소환하지 않는다.
            {
                // 엘리트 소환
                // 특정 시간을 넘으면 엘리트 몬스터 소환

                if (EliteSpawnIndex < waveInfo.spawnEliteId.Count && _gameTimeMin >= waveInfo.spawnEliteTime[EliteSpawnIndex])
                {
                    Vector3 spawnPos = Utils.GetRotatedCamOutRandPos3D(Camera.main);

                    spawnMonsterId = waveInfo.spawnEliteId[EliteSpawnIndex];
                    Managers.Instance.ObjectManager.Spawn<MonsterController>(spawnPos, spawnMonsterId);

                    EliteSpawnIndex++;
                }

                // 일반 몬스터를 주기적으로 소환
                // 현재 스테이지에서 몬스터 종류가 하나 인 경우
                if (waveInfo.monsterSpawnRate.Count == 1)
                {
                    for (int i = 0; i < onceSpawnCount; i++)
                    {
                        Vector3 spawnPos = Utils.GetRotatedCamOutRandPos3D(Camera.main);

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
                        Vector3 spawnPos = Utils.GetRotatedCamOutRandPos3D(Camera.main);

                        // 여러 몬스터 중 하나 선택
                        float rand = Random.value * totalRate;

                        for (int n = 0; n < waveInfo.spawnMonsterId.Count; n++)
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
            yield return null;
        }
     /**/
    }
}
