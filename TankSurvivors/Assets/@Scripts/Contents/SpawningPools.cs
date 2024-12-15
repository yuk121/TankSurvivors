using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningPools : MonoBehaviour
{
    private Coroutine _spawnCor = null;

    public bool SpawnStop { get; set; } = false;

    public void StartSpawn()
    {
        if (_spawnCor != null) 
        {
            StopCoroutine(_spawnCor);
        }

        _spawnCor = StartCoroutine(SpwanCreatureCor());
    }


    IEnumerator SpwanCreatureCor()
    {     
        yield return null;
        
        // �ӽ�
        int curStageId = 1;
        WaveData waveInfo = null;
       
        // ���̺� ���� �ʿ�
        foreach (var Data in Managers.Instance.DataTableManager.DataTableWave.Datas)
        {
            if (Data.stageIndex == curStageId)
            {
                waveInfo = Data;
                break;
            }
        }

        while(true)
        {
            int spawnMonsterId = -1;
            int onceSpawnCount = waveInfo.onceSpawnCount;

            // �׽�Ʈ�� (�Ѹ����� ��ȯ)
            //onceSpawnCount = 1;

            //if (Managers.Instance.ObjectManager.Monsters.Count > 0)
            //    break;

            // ���͸� �ֱ������� ��ȯ
            // ���� ������������ ���� ������ �ϳ� �� ���
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
            else // ���� ������ ���� ������ ���
            {
                float totalRate = -1;

                for (int i = 0; i < waveInfo.monsterSpawnRate.Count; i++)
                {
                    totalRate += waveInfo.monsterSpawnRate[i];
                }
              
                for (int i = 0; i < waveInfo.onceSpawnCount; i++)
                {
                    Vector3 spawnPos = Utils.GetCamOutPos3D(Camera.main, 0.5f, 3f);
                   
                    // ���� ���� �� �ϳ� ����
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
