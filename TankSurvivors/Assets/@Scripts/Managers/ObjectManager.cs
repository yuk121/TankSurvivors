using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectManager
{
    public PlayerController Player { get; private set; }
    public HashSet<MonsterController> Monsters { get; } = new HashSet<MonsterController>();
    public HashSet<Projectile> Projectiles { get; } = new HashSet<Projectile>();

    public HashSet<DropItemController> Gems { get; } = new HashSet<DropItemController>();

    public T Spawn<T>(Vector3 spawnPos, int implementID = 0, Vector3 spawnDir = default(Vector3)) where T : BaseController
    {
        System.Type type = typeof(T);

        if (type == typeof(PlayerController))
        {
            CreatureData creatrueData = Managers.Instance.DataTableManager.DataTableCreature.GetCreatureData(implementID);

            string charPrefabPath = $"PlayerPrefab/{creatrueData.prefabName}.prefab";

            GameObject go = Managers.Instance.ResourceManager.Instantiate(charPrefabPath);
            go.name = "Player";
            go.transform.position = spawnPos;

            PlayerController pc = Utils.GetOrAddComponent<PlayerController>(go);
            Player = pc;
            pc.CreatureData = creatrueData;
            
            pc.Init();

            // Indicator 
            Transform indicator = Managers.Instance.ResourceManager.Instantiate(Define.DUMMY_INDICATOR_PREFAB_PATH).GetComponent<Transform>();

            indicator.SetParent(pc.DummyIndicatorPos);
            indicator.localPosition = new Vector3(0, 0.01f, 0);
           
            return pc as T;
        }
        else if (type == typeof(MonsterController))
        {
            CreatureData creatrueData = Managers.Instance.DataTableManager.DataTableCreature.GetCreatureData(implementID);

            string monsterPrefabPath = $"EnemyPrefab/{creatrueData.prefabName}.prefab";

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
        else if(type == typeof(Projectile))
        {
            SkillData skillData = Managers.Instance.DataTableManager.DataTableSkill.GetSkillData(implementID); ;
          
            string skillPrefabPath = $"SkillPrefab/{skillData.prefabName}.prefab";

            GameObject go = Managers.Instance.ResourceManager.Instantiate(skillPrefabPath, pooling: true);
            go.name = skillData.prefabName;
            go.transform.position = spawnPos;
            go.transform.forward = spawnDir;

            Projectile projectile = Utils.GetOrAddComponent<Projectile>(go);

            // 발사체 HashSet에서 관리
            Projectiles.Add(projectile);

            return projectile as T;
        }
        else if(type == typeof(DropItemGem)) 
        {
            DropItemData dropItemData = Managers.Instance.DataTableManager.DataTableDropItem.GetDropItemData(implementID); 

            string dropItemPrefabPath = $"DropItemPrefab/{dropItemData.prefabName}.prefab";

            GameObject go = Managers.Instance.ResourceManager.Instantiate(dropItemPrefabPath, pooling: true);

            go.transform.position = spawnPos+Vector3.up;

            DropItemGem gem = Utils.GetOrAddComponent<DropItemGem>(go);

            gem.SetData(dropItemData);
            gem.Init();

            Gems.Add(gem);
           
            // GridManager에 추가
            GridManager.Instance.Add(gem);

            return gem as T;
        }

        //if(type == typeof(FogController))
        //{
        //    string fogPrefabPath = $"MapPrefab/Fog.prefab";

        //    GameObject go = Managers.Instance.ResourceManager.Instantiate(fogPrefabPath);

        //    go.transform.position = spawnPos;

        //    FogController fog = Utils.GetOrAddComponent<FogController>(go);
        //    fog.Init();

        //    return fog as T;
        //}

        return null;
    }

    public void Despawn <T>( T gameobject) where T : BaseController
    {
        if (gameobject == null)
            return;

        System.Type type = typeof(T);

        if (type == typeof(MonsterController))
        {
            MonsterController mon = gameobject as MonsterController;
            Monsters.Remove(mon);

            Managers.Instance.ResourceManager.Destory(mon.gameObject);
            return;
        }
        else if (type == typeof(Projectile))
        {
            Projectile projectile = gameobject as Projectile;
            Projectiles.Remove(projectile);

            Managers.Instance.ResourceManager.Destory(projectile.gameObject);
            return;
        }
        else if (type == typeof(DropItemGem))
        {
            DropItemGem gem = gameobject as DropItemGem;
            Gems.Remove(gem);

            // GridManager에서 제거
            GridManager.Instance.Remove(gem);

            Managers.Instance.ResourceManager.Destory(gem.gameObject);
        }
        else if (type == typeof(DropItemBomb))
        {
            DropItemBomb bomb = gameobject as DropItemBomb;
            
            GridManager.Instance.Remove(bomb);
            Managers.Instance.ResourceManager.Destory(bomb.gameObject);
        }
        else if (type == typeof(DropItemHeart))
        {
            DropItemHeart heart = gameobject as DropItemHeart;

            GridManager.Instance.Remove(heart);
            Managers.Instance.ResourceManager.Destory(heart.gameObject);
        }
        else if (type == typeof(DropItemMagent))
        {
            DropItemMagent magent = gameobject as DropItemMagent;

            GridManager.Instance.Remove(magent);
            Managers.Instance.ResourceManager.Destory(magent.gameObject);
        }
    }
}
