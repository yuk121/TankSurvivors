using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Scene : UI_Base
{
    protected Define.eSceneType _sceneType;
    public Define.eSceneType SceneType { get => _sceneType; }
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        return true;
    }
}
