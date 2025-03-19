using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class DropItemGem : DropItemController
{
    [SerializeField]
    private Define.eGemType type;
    private int expAmount = 0;

    public override bool Init()
    {
        if(_init == false)
            base.Init();
        
        SetGemExp(type);
        return true;
    }
    
    public int GetGemExp()
    {
        return expAmount;   
    }

    public void SetGemExp(Define.eGemType gemType)
    {
        switch (gemType)
        {
            case Define.eGemType.RedGem:
                expAmount = Define.GEM_RED_EXP_AMOUNT;
                break;

            case Define.eGemType.GreenGem:
                expAmount = Define.GEM_GREEN_EXP_AMOUNT;
                break;

            case Define.eGemType.BlueGem:
                expAmount = Define.GEM_BLUE_EXP_AMOUNT;
                break;

            case Define.eGemType.PurpleGem:
                expAmount = Define.GEM_PURPLE_EXP_AMOUNT;
                break;
        }
    }

    public override void GetItemCompleted()
    {
        // »ç¿îµå
        SoundManager.Instance.Play("SFX_CollectGem", Define.eSoundType.SFX);

        _player.GetExp(expAmount);
        Managers.Instance.ObjectManager.Despawn(this);
    }
}
