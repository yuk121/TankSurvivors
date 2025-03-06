using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class Define
{
    // Drop Item
    public const float ITEM_MOVE_SPEED = 15f;
    public const float HEART_RECOVERY_RATE = 0.15f;

    // Gem
    public const int GEM_RED_EXP_AMOUNT = 5;
    public const int GEM_GREEN_EXP_AMOUNT = 15;
    public const int GEM_BLUE_EXP_AMOUNT = 45;
    public const int GEM_PURPLE_EXP_AMOUNT = 100;

    // Prefab
    public const string DUMMY_INDICATOR_PREFAB_PATH = "EtcPrefab/Indicator.prefab";
    public const string DUMMY_FIREPOS_MUZZLE_PREFAB_PATH = "SkillPrefab/Tracer_GoldFire_Small_MuzzleFlare.prefab";

    public const string UI_PREFAB_PATH = "UIPrefab";

    // Skill
    public const int MAX_SKILL_LEVEL = 5;
    public const int MAX_SKILL_SELECT_COUNT = 3;
    public const int MAX_ACTIVE_SKILL_COUNT = 5;
    public const int MAX_SUPPORT_SKILL_COUNT = 5;

    public const float SUBTANK_SPAWN_RADIUS = 3f;
    public const float SUBTANK_DETECT_RADIUS = 1f;

    public const float ELECTRIC_FIELD_DETECT_RADIUS = 3f;
    public const float ELECTRIC_FIELD_RADIUS_INC_RATE = 0.6f;

    public const float MINE_DETECT_RADIUS = 1f;

    public const float KNOCKBACK_FORCE = 3f;

    // Enemy
    public const float ENEMY_DETECT_RANGE = 1.5f;

    public enum eSceneType
    {
        Cs,
        Title,
        Lobby,
        Game,
    }
    public enum eUIEvent
    {
        Click,
        Pressed,
        PointerDown,
        PointerUp,
        Drag,
        BeginDrag,
        EndDrag
    }

    public enum eCreatureAnimState
    {
        None = -1,
        Chase,
        Skill1,
        Skill2,
        AttackIdle,
        Dead,
        Max
    }

    public enum eMonsterFSMState
    {
        None,
        Chase,
        Skill,
        AttackIdle,
        Pause,
    }
    
    public enum eMonsterGrade
    {
        Normal,
        Elite,
        Boss
    }

    public enum eObjectType
    {
        Player,
        Enemy,
        Gem,
        HpRecorvery,
        Bomb,
        Magnet,
        Box
    }

    public enum eGemType
    {
        RedGem = 50001,
        GreenGem = 50002,
        BlueGem = 50003,
        PurpleGem = 50004,
    }

    public enum eSkillType
    {
        None,
        // Player
        TankShell = 1001,
        SubTank = 1011,
        ElectircField = 1021,
        Mine = 1031,

        // Enemy
        ZombieAttack01 = 2001,
        ZombieAttack02 = 2002,
        EliteZombieAttack01 = 3001,
        BossZombieAttack01 = 4001,
        BossZombieSkill01 = 4002,
    }

    public enum eSupportSkillType
    {
        None,

        Magnet = 10001,
        AttackUp = 10011,
        ExpGetRateUp = 10021,
        MaxHpUp = 10031,
        SpeedUp = 10041
    }
}

