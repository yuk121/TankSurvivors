using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemController : BaseController
{
    DropItemData _dropItemData;

    [SerializeField]
    private Define.eObjectType _itemType;
    public Define.eObjectType ItemType { get => _itemType; set => _itemType = value; }

    protected Coroutine _corMoveToPlayer;
    protected Transform _trans;
    protected PlayerController _player;

    public void SetData(DropItemData data)
    {
        _dropItemData = data;
        _itemType = data.dropItemType;
        _trans = transform;
    }

    public virtual void GetItem()
    {
        GridManager.Instance.Remove(this);

        _player = GameManager.Instance.Player;

        if (_corMoveToPlayer != null)
        {
            StopCoroutine(_corMoveToPlayer);
            _corMoveToPlayer = null;
        }

        Vector3 dir = (_trans.position - _player.transform.position).normalized;
        Vector3 target = _trans.position + dir * 2f;
        _trans.DOKill();
        _trans.DOMove(target, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
        {
            _corMoveToPlayer = StartCoroutine(CorMoveToPlayer(_player));
        });
    }

    public virtual void GetItemCompleted() { }

    private IEnumerator CorMoveToPlayer(PlayerController player)
    {
        if (player == null)
        {
            Debug.LogError($"{this} : Player is null !!!");
            yield break;
        }

        Vector3 playerPos = Vector3.zero;
        float moveToPlayerSpeed = Time.deltaTime * Define.ITEM_MOVE_SPEED;

        while (true)
        {
           if (GameManager.Instance.Pause == true)
            {
                yield return null;
                continue;
            }
            
            if (player == null)
                yield break;

            playerPos = player.transform.position;
            float distance = Vector3.Distance(_trans.position, playerPos);

            if (distance < 0.4f)
            {
                GetItemCompleted();             
                yield break;
            }

            _trans.position = Vector3.MoveTowards(_trans.position, playerPos, moveToPlayerSpeed);

            yield return null;
        }
    }
}
