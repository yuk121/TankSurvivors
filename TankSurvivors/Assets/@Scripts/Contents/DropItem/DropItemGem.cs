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
    private Coroutine _corMoveToPlayer;
    private Transform _trans;

    public override bool Init()
    {
        base.Init();
        
        SetGemExp(type);
        _trans = transform;
        return true;
    }

    public override void GetItem()
    {
        base.GetItem();

        if (_corMoveToPlayer != null)
        {
            StopCoroutine(_corMoveToPlayer);
            _corMoveToPlayer = null;
        }


        PlayerController player = GameManager.Instance.Player;

        Vector3 dir = (_trans.position - player.transform.position).normalized;
        Vector3 target = _trans.position + dir * 1.5f;
        _trans.DOMove(target, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
        {
            _corMoveToPlayer = StartCoroutine(CorMoveToPlayer());
        });
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

    IEnumerator CorMoveToPlayer()
    {
        PlayerController player = GameManager.Instance.Player;

        while(true)
        {
            float distance = Vector3.Distance(_trans.position, player.transform.position);

            if(distance < 0.4f)
            {
                player.Exp += expAmount;
                Managers.Instance.ObjectManager.Despawn(this);
                yield break; 
            }

            _trans.position = Vector3.MoveTowards(_trans.position, player.transform.position, Time.deltaTime * Define.ITEM_MOVE_SPEED);
           
            yield return null;
        }
    }
}
