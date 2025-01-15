using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    public PlayerController Player { get; private set; }
    public HashSet<MonsterController> Monsters { get; } = new HashSet<MonsterController>();

    public T Spawn<T>(Vector3 position, int implementID = 0) where T : BaseController
    {
        System.Type type = typeof(T);

        if (type == typeof(PlayerController))
        {
            string charPrefabPath = string.Empty;
            CreatureData creatrueData = null;

            foreach (var Data in Managers.Instance.DataTableManager.DataTableCreature.DataList)
            {
                if (implementID == Data.creatureId)
                {
                    charPrefabPath = $"PlayerPrefab/{Data.prefabName}.prefab";
                    creatrueData = Data;
                    break;
                }
            }

            GameObject go = Managers.Instance.ResourceManager.Instantiate(charPrefabPath);
            go.name = "Player";
            go.transform.position = position;

            PlayerController pc = Utils.GetOrAddComponent<PlayerController>(go);
            Player = pc;
            pc.CreatureData = creatrueData;
            
            pc.Init();

            return pc as T;
        }

        if (type == typeof(MonsterController))
        {
            string monsterPrefabPath = string.Empty;
            CreatureData creatrueData = null;

            foreach (var Data in Managers.Instance.DataTableManager.DataTableCreature.DataList)
            {
                if (implementID == Data.creatureId)
                {
                    monsterPrefabPath = $"EnemyPrefab/{Data.prefabName}.prefab";
                    creatrueData = Data;

                    break;
                }
            }

            GameObject go = Managers.Instance.ResourceManager.Instantiate(monsterPrefabPath, pooling: true);
            go.name = creatrueData.creatureLocalName;
            go.transform.position = position;
            go.transform.rotation = Quaternion.identity;

            MonsterController mon = Utils.GetOrAddComponent<MonsterController>(go);
            // 크리쳐 정보
            mon.CreatureData = creatrueData;

            mon.Init();

            // 몬스터 HashSet에 추가하여 관리
            Monsters.Add(mon);

            return mon as T;
        }

        return null;
    }
}
