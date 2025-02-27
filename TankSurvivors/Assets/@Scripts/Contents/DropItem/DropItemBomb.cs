using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DropItemBomb : DropItemController
{
    public override bool Init()
    {
        if (_init == false)
            base.Init();
        return true;
    }

    public override void GetItemCompleted()
    {
        Managers.Instance.ObjectManager.AllKillMonsters();
        Managers.Instance.ObjectManager.Despawn(this);
    }
}
