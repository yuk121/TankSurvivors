using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemController : BaseController
{
    [SerializeField]
    private Define.eObjectType _itemType;
    public Define.eObjectType ItemType { get => _itemType; set => _itemType = value; }
}
