using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using static Define;

public class CreatureData
{
    public int creatureId;
    public string creatureLocalName;
    public eObjectType objectType;
    public float maxHp;
    public float atk;
    public float def;
    public float moveSpeed;
    public string prefabName;
    public List<int> skillList;
}

public class DataTableCreature 
{
    private List<CreatureData> _dataList = new List<CreatureData>();
    public List<CreatureData> DataList { get => _dataList; }

    private void Unload()
    {
        if(_dataList != null && _dataList.Count > 0) 
        {
            _dataList.Clear();
        }
    }

    public void DataLoad(byte[] dataText)
    {
        Unload();

        if (dataText == null || dataText.Length < 1)
            return;
      
        TableLoader loader = TableLoader.Instance;
        loader.LoadTable(dataText);

        CreatureData creatureData;

        for (int i = 0; i < loader.GetLength(); i++)
        {
            creatureData = new CreatureData();

            creatureData.creatureId = loader.GetInt("CreatureId", i);
            creatureData.creatureLocalName = loader.GetString("CreatureLocalName", i);
            creatureData.objectType = loader.GetEnum<eObjectType>("ObjectType", i);
            creatureData.maxHp = loader.GetFloat("MaxHp", i);
            creatureData.atk = loader.GetFloat("Atk", i);
            creatureData.def = loader.GetFloat("Def", i);
            creatureData.moveSpeed = loader.GetFloat("MoveSpeed",i);
            creatureData.prefabName = loader.GetString("PrefabName", i);
            creatureData.skillList = loader.GetList_Int("SkillList", i);

            _dataList.Add(creatureData);
        }

        loader.Clear();
    }

    public CreatureData GetCreatureData(int creatureId)
    {
        CreatureData data = new CreatureData();

        foreach (var creatureData in _dataList)
        {
            if (creatureData.creatureId == creatureId)
            {
                data = creatureData;
                break;
            }
        }

        return data;
    }
}
