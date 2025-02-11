using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemController : BaseController
{
    DropItemData _dropItemData;

    [SerializeField]
    private Define.eObjectType _itemType;
    public Define.eObjectType ItemType { get => _itemType; set => _itemType = value; }

    public void SetData(DropItemData data)
    {
        _dropItemData = data;
        _itemType = data.dropItemType;
    }

    public virtual void GetItem() 
    {
        GridManager.Instance.Remove(this);
    }
}
