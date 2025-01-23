using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DropItemGem : DropItemController
{
    [SerializeField]
    private Define.eGemType type;
    private int expAmount;

    public override bool Init()
    {
        base.Init();
        
        SetGemExp(type);
        
        return true;
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
}
