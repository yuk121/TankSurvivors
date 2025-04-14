using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopup_Cheat : UI_Popup
{
    private enum eCheat
    {
        PlayerLevelUp,
        MonsterAllKill,
    }

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        return false;
    }
}
