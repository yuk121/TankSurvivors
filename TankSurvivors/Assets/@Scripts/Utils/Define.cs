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
        Skill,
        AttackIdle,
        Dead,
        Max
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

