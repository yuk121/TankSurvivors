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
        // »ç¿îµå
        SoundManager.Instance.Play("SFX_CollectHeart", Define.eSoundType.SFX);

        _player.OnRecovery(Define.HEART_RECOVERY_RATE);
        Managers.Instance.ObjectManager.Despawn(this);
    }
}
