using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class DropItemData
{
    public int dropItemId;
    public string dropItemLocalName;
    public string dropItemLocalDesc;
    public eObjectType dropItemType;
    public string prefabName;
}

public class DataTableDropItem
{
    private List<DropItemData> _dataList = new List<DropItemData>();
    public List<DropItemData> DataList { get => _dataList; }
    private void Unload()
    {
        if (_dataList != null && _dataList.Count > 0)
        {
            _dataList.Clear();
        }
    }
    public void DataLoad(byte[] dataText)
    {
        Unload();

        TableLoader loader = TableLoader.Instance;
        loader.LoadTable(dataText);

        DropItemData dropItemData;
        for (int i = 0; i < loader.GetLength(); i++)
        {
            dropItemData = new DropItemData();

            dropItemData.dropItemId = loader.GetInt("DropItemId", i);
            dropItemData.dropItemLocalName = loader.GetString("DropItemLocalName", i);
            dropItemData.dropItemLocalDesc = loader.GetString("DropItemLocalDesc", i);
            dropItemData.dropItemType = loader.GetEnum<eObjectType>("DropItemType",i);
            dropItemData.prefabName = loader.GetString("PrefabName", i);

            _dataList.Add(dropItemData);
        }

        loader.Clear();
    }

    public DropItemData GetDropItemData(int dropItemId)
    {
        DropItemData data = new DropItemData();

        foreach (var dropItemData in _dataList)
        {
            if (dropItemData.dropItemId == dropItemId)
            {
                data = dropItemData;
                break;
            }
        }

        return data;
    }
}
