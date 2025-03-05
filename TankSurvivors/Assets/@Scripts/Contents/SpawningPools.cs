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

        // �ӽ�
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

            // �÷��̾ ����� ���̻� �۵����� �ʴ´�.
            if (GameManager.Instance.CheckPlayerAlive() == false)
                break;


            // �׽�Ʈ�� (�Ѹ����� ��ȯ)
            /*
            {
                onceSpawnCount = 1;

                if (Managers.Instance.ObjectManager.Monsters.Count > 0)
                    break;
            }
            /**/

            // ���� ��ȯ
            if (_gameTimeMin >= waveInfo.spawnBossTime)
            {
                Vector3 spawnPos = Utils.GetRotatedCamOutRandPos3D(Camera.main);

                spawnMonsterId = waveInfo.spawnBossId;
                Managers.Instance.ObjectManager.Spawn<MonsterController>(spawnPos, spawnMonsterId);
                yield break;
            }
            else  // ���� ��ȯ�� �� �̻� ��ȯ���� �ʴ´�.
            {
                // ����Ʈ ��ȯ
                // Ư�� �ð��� ������ ����Ʈ ���� ��ȯ

                if (EliteSpawnIndex < waveInfo.spawnEliteId.Count && _gameTimeMin >= waveInfo.spawnEliteTime[EliteSpawnIndex])
                {
                    Vector3 spawnPos = Utils.GetRotatedCamOutRandPos3D(Camera.main);

                    spawnMonsterId = waveInfo.spawnEliteId[EliteSpawnIndex];
                    Managers.Instance.ObjectManager.Spawn<MonsterController>(spawnPos, spawnMonsterId);

                    EliteSpawnIndex++;
                }

                // �Ϲ� ���͸� �ֱ������� ��ȯ
                // ���� ������������ ���� ������ �ϳ� �� ���
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
                else // ���� ������ ���� ������ ���
                {
                    float totalRate = -1;

                    for (int i = 0; i < waveInfo.monsterSpawnRate.Count; i++)
                    {
                        totalRate += waveInfo.monsterSpawnRate[i];
                    }

                    for (int i = 0; i < waveInfo.onceSpawnCount; i++)
                    {
                        Vector3 spawnPos = Utils.GetRotatedCamOutRandPos3D(Camera.main);

                        // ���� ���� �� �ϳ� ����
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
