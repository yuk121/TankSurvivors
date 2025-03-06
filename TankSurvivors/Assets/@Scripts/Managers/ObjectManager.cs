using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectManager
{
    public PlayerController Player { get; private set; }
    public HashSet<MonsterController> Monsters { get; } = new HashSet<MonsterController>();
    public HashSet<Projectile> Projectiles { get; } = new HashSet<Projectile>();

    public HashSet<DropItemGem> Gems { get; } = new HashSet<DropItemGem>();

    public T Spawn<T>(Vector3 spawnPos, int implementID = 0, Vector3 spawnDir = default(Vector3), bool bPooling = true) where T : BaseController
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

            GameObject go = Managers.Instance.ResourceManager.Instantiate(monsterPrefabPath, pooling: bPooling);
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

            GameObject go = Managers.Instance.ResourceManager.Instantiate(skillPrefabPath, pooling: bPooling);
            go.name = skillData.prefabName;
            go.transform.position = spawnPos;
            go.transform.forward = spawnDir;

            Projectile projectile = Utils.GetOrAddComponent<Projectile>(go);

            // 발사체 HashSet에서 관리
            Projectiles.Add(projectile);

            return projectile as T;
        }
        else if(type == typeof(HitDetection))
        {
            SkillData skillData = Managers.Instance.DataTableManager.DataTableSkill.GetSkillData(implementID); ;

            string skillPrefabPath = $"SkillPrefab/{skillData.prefabName}.prefab";

            GameObject go = Managers.Instance.ResourceManager.Instantiate(skillPrefabPath, pooling: bPooling);
            go.name = skillData.prefabName;
            go.transform.position = spawnPos;
            go.transform.forward = spawnDir;

            HitDetection hitDetection = Utils.GetOrAddComponent<HitDetection>(go);

            return hitDetection as T;
        }
        else if(type == typeof(DropItemGem)) 
        {
            DropItemData dropItemData = Managers.Instance.DataTableManager.DataTableDropItem.GetDropItemData(implementID); 

            string dropItemPrefabPath = $"DropItemPrefab/{dropItemData.prefabName}.prefab";

            GameObject go = Managers.Instance.ResourceManager.Instantiate(dropItemPrefabPath, pooling: bPooling);

            go.transform.position = spawnPos+Vector3.up;

            DropItemGem gem = Utils.GetOrAddComponent<DropItemGem>(go);

            gem.Init();
            gem.SetData(dropItemData);

            Gems.Add(gem);
           
            // GridManager에 추가
            GridManager.Instance.Add(gem);

            return gem as T;
        }
        else if(type == typeof(DropItemBomb))
        {
            DropItemData dropItemData = Managers.Instance.DataTableManager.DataTableDropItem.GetDropItemData(implementID);

            string dropItemPrefabPath = $"DropItemPrefab/{dropItemData.prefabName}.prefab";

            GameObject go = Managers.Instance.ResourceManager.Instantiate(dropItemPrefabPath, pooling: bPooling);

            go.transform.position = spawnPos + Vector3.up;

            DropItemBomb bomb = Utils.GetOrAddComponent<DropItemBomb>(go);
            
            bomb.Init();
            bomb.SetData(dropItemData);
            
            GridManager.Instance.Add(bomb);

            return bomb as T;
        }
        else if(type == typeof(DropItemMagnet))
        {
            DropItemData dropItemData = Managers.Instance.DataTableManager.DataTableDropItem.GetDropItemData(implementID);

            string dropItemPrefabPath = $"DropItemPrefab/{dropItemData.prefabName}.prefab";

            GameObject go = Managers.Instance.ResourceManager.Instantiate(dropItemPrefabPath, pooling: bPooling);

            go.transform.position = spawnPos + Vector3.up;

            DropItemMagnet magnet = Utils.GetOrAddComponent<DropItemMagnet>(go);

            magnet.Init();
            magnet.SetData(dropItemData);

            GridManager.Instance.Add(magnet);
        }
        else if(type == typeof(DropItemHeart)) 
        {
            DropItemData dropItemData = Managers.Instance.DataTableManager.DataTableDropItem.GetDropItemData(implementID);

            string dropItemPrefabPath = $"DropItemPrefab/{dropItemData.prefabName}.prefab";

            GameObject go = Managers.Instance.ResourceManager.Instantiate(dropItemPrefabPath, pooling: bPooling);

            go.transform.position = spawnPos + Vector3.up;

            DropItemHeart heart = Utils.GetOrAddComponent<DropItemHeart>(go);

            heart.Init();
            heart.SetData(dropItemData);
            
            GridManager.Instance.Add(heart);
        }
        else if(type == typeof(DropItemBox))
        {
            DropItemData dropItemData = Managers.Instance.DataTableManager.DataTableDropItem.GetDropItemData(implementID);

            string dropItemPrefabPath = $"DropItemPrefab/{dropItemData.prefabName}.prefab";

            GameObject go = Managers.Instance.ResourceManager.Instantiate(dropItemPrefabPath, pooling: bPooling);

            go.transform.position = spawnPos + Vector3.up;

            DropItemBox box = Utils.GetOrAddComponent<DropItemBox>(go);
            box.Init();
            box.SetData(dropItemData);
            GridManager.Instance.Add(box);
        }

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

            Managers.Instance.ResourceManager.Destroy(mon.gameObject);
            return;
        }
        else if (type == typeof(Projectile))
        {
            Projectile projectile = gameobject as Projectile;
            Projectiles.Remove(projectile);

            Managers.Instance.ResourceManager.Destroy(projectile.gameObject);
            return;
        }
        else if(type == typeof(HitDetection))
        {
            HitDetection hitDetection = gameobject as HitDetection;

            Managers.Instance.ResourceManager.Destroy(hitDetection.gameObject);
            return;
        }
        else if (type == typeof(DropItemGem))
        {
            DropItemGem gem = gameobject as DropItemGem;
            Gems.Remove(gem);

            // GridManager에서 제거
            GridManager.Instance.Remove(gem);

            Managers.Instance.ResourceManager.Destroy(gem.gameObject);
            return;
        }
        else if (type == typeof(DropItemBomb))
        {
            DropItemBomb bomb = gameobject as DropItemBomb;
            
            GridManager.Instance.Remove(bomb);
            Managers.Instance.ResourceManager.Destroy(bomb.gameObject);
            return;
        }
        else if (type == typeof(DropItemHeart))
        {
            DropItemHeart heart = gameobject as DropItemHeart;

            GridManager.Instance.Remove(heart);
            Managers.Instance.ResourceManager.Destroy(heart.gameObject);
            return;
        }
        else if (type == typeof(DropItemMagnet))
        {
            DropItemMagnet magent = gameobject as DropItemMagnet;

            GridManager.Instance.Remove(magent);
            Managers.Instance.ResourceManager.Destroy(magent.gameObject);
            return;
        }
        else if(type == typeof(DropItemBox))
        {
            DropItemBox box = gameobject as DropItemBox;

            GridManager.Instance.Remove(box);
            Managers.Instance.ResourceManager.Destroy(box.gameObject);
            return;
        }
    }

    public void AllKillMonsters()
    {
        foreach (MonsterController monster in Monsters.ToList())
        {
            // 엘리트나 보스는 폭탄에서 제외
            if (monster.Grade == Define.eMonsterGrade.Normal)
            { 
                monster.OnDead();
            }
        }
    }
#if UNITY_EDITOR
    public void CheatAllKillMonsters()
    {
        foreach (MonsterController monster in Monsters.ToList())
        {
            monster.OnDead();
        }
    }
#endif
}
