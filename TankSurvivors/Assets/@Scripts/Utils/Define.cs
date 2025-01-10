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
        Active,
        Passive
    }
}

