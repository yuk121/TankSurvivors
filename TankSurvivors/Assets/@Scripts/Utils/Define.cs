using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class Define
{
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
    public enum eObjectType
    {
        Player,
        Enemy
    }

    public enum eDropItemType
    {
        Gem,
        Item
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

