using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Scene : UI_Base
{
    protected eGameManagerState _sceneState;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        return true;
    }
}
