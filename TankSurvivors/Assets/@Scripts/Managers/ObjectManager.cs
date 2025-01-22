using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    public PlayerController Player { get; private set; }
    public HashSet<MonsterController> Monsters { get; } = new HashSet<MonsterController>();
    public HashSet<Projectile> Projectiles { get; } = new HashSet<Projectile>();

    public T Spawn<T>(Vector3 spawnPos, int implementID = 0, Vector3 spawnDir = default(Vector3)) where T : BaseController
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
            go.transform.position = spawnPos;

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
            go.transform.position = spawnPos;
            go.transform.rotation = Quaternion.identity;

            MonsterController mon = Utils.GetOrAddComponent<MonsterController>(go);
            // 크리쳐 정보
            mon.CreatureData = creatrueData;

            mon.Init();

            // 몬스터 HashSet에 추가하여 관리
            Monsters.Add(mon);

            return mon as T;
        }

        if(type == typeof(Projectile))
        {
            string skillPrefabPath = string.Empty;
            SkillData skillData = null;

            foreach (var Data in Managers.Instance.DataTableManager.DataTableSkill.DataList)
            {
                if (implementID == Data.skillId)
                {
                    skillPrefabPath = $"EnemyPrefab/{Data.prefabName}.prefab";
                    skillData = Data;

                    break;
                }
            }

            GameObject go = Managers.Instance.ResourceManager.Instantiate(skillPrefabPath, pooling: true);
            go.name = skillData.prefabName;
            go.transform.position = spawnPos;
            go.transform.forward = spawnDir;

            Projectile projectile = Utils.GetOrAddComponent<Projectile>(go);

            // 발사체 HashSet에서 관리
            Projectiles.Add(projectile);

            return projectile as T;
        }

        return null;
    }
}
