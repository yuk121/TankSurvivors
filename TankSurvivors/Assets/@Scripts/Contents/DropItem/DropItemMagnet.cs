using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DropItemMagnet : DropItemController
{
    public override bool Init()
    {
        if (_init == false)
            base.Init();
        return true;
    }


    public override void GetItemCompleted()
    {
        HashSet<DropItemGem> gems = Managers.Instance.ObjectManager.Gems;
        
        foreach (DropItemGem gem in gems.ToList())
        {
            gem.GetItem(); 
        }

        Managers.Instance.ObjectManager.Despawn(this);
    }

}
