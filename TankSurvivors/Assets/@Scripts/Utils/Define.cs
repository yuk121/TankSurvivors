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

    public enum eObjectType
    {
        Player,
        Enemy
    }
}
