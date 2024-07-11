using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    public PlayerController Player { get; private set; }

    public T Spawn<T>(Vector3 position, int implementID = 0) where T :BaseController
    {
        System.Type type = typeof(T);

        if(type == typeof(PlayerController))
        {
            string charPrefabPath = string.Empty;

            foreach(var Data in CreatureDataTable.Data.DataList)
            {
                if(implementID == Data.CreatureId)
                {
                    charPrefabPath = $"PlayerPrefab/{Data.PrefabName}.prefab";
                    break;
                }
            }

            GameObject go = Managers.Instance.ResourceManager.Instantiate(charPrefabPath);
            go.name = "Player";
            go.transform.position = position;

            PlayerController pc = Utils.GetOrAddComponent<PlayerController>(go);
            Player = pc;
            pc.Init();

            return pc as T;
        }

        if(type == typeof(MonsterController))
        {
            string monsterPrefabPath = string.Empty;
            string monsterName = string.Empty;

            foreach (var Data in CreatureDataTable.Data.DataList)
            {
                if (implementID == Data.CreatureId)
                {
                    monsterPrefabPath = $"EnemyPrefab/{Data.PrefabName}.prefab";
                    monsterName = Data.PrefabName;
                    break;
                }
            }
            GameObject go = Managers.Instance.ResourceManager.Instantiate(monsterPrefabPath, pooling : true);
            go.name = monsterName;
            go.transform.position = position;

            MonsterController mon = Utils.GetOrAddComponent<MonsterController>(go);
            mon.Init();

            return mon as T;
        }

        return null;
    }
}
