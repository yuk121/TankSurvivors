using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class Define
{
    public const float ITEM_MOVE_SPEED = 15f;

    public const int GEM_RED_EXP_AMOUNT = 5;
    public const int GEM_GREEN_EXP_AMOUNT = 15;
    public const int GEM_BLUE_EXP_AMOUNT = 45;
    public const int GEM_PURPLE_EXP_AMOUNT = 100;

    public const string DUMMY_INDICATOR_PREFAB_PATH = "EtcPrefab/Indicator.prefab";
    public const string DUMMY_FIREPOS_MUZZLE_PREFAB_PATH = "SkillPrefab/Tracer_GoldFire_Small_MuzzleFlare.prefab";
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
        RedGem,
        GreenGem,
        BlueGem,
        PurpleGem,
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
}

