using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemHeart : DropItemController
{
    public override bool Init()
    {
        if (_init == false)
            base.Init();
        return true;
    }

    public override void GetItemCompleted()
    {
        Managers.Instance.ObjectManager.Despawn(this);
    }
}
