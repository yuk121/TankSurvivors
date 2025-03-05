using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemBox : DropItemController
{
    public override bool Init()
    {
        if (_init == false)
            base.Init();

        return true;
    }

    public override void GetItemCompleted()
    {
        UIPopup_SkillGet popup = Managers.Instance.UIMananger.OpenPopup<UIPopup_SkillGet>();
        popup.Set();

        Managers.Instance.ObjectManager.Despawn(this);
    }
}
