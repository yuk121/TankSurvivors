using GoogleSheet.Core.Type;
using GoogleSheet.Type;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[UGS(typeof(eObjectType))]
public enum eObjectType
{
    Player,
    Enemy
}

[UGS(typeof(eDropItemType))]
public enum eDropItemType
{
    Gem,
    Item
}

[UGS(typeof(eSkillType))]
public enum eSkillType
{
    Active,
    Passive
}


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
}
