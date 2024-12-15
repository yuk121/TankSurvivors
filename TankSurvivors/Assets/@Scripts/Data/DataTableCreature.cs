using System.Collections;
using System.Collections.Generic;
using static DataTableWave;

public class CreatureData
{
    public int creatureId;
    public string creatureLocalName;
    public Define.eObjectType objectType;
    public float maxHp;
    public float atk;
    public float def;
    public float moveSpeed;
    public string prefabName;
    public List<int> skillList;
}

public class DataTableCreature 
{
    private List<CreatureData> _datas = new List<CreatureData>();
    public List<CreatureData> Datas { get => _datas; }

    private void Unload()
    {
        if(_datas != null && _datas.Count > 0) 
        {
            _datas.Clear();
        }
    }

    public void DataLoad(byte[] dataText)
    {
        Unload();

        if (dataText == null || dataText.Length < 1)
            return;
      
        TableLoader loader = TableLoader.Instance;
        loader.LoadTable(dataText);

        for (int i = 0; i < loader.GetLength(); i++)
        {
            CreatureData creatureData = new CreatureData();

            creatureData.creatureId = loader.GetInt("CreatureId", i);
            creatureData.creatureLocalName = loader.GetString("CreatureLocalName", i);
            creatureData.objectType = loader.GetEnum<Define.eObjectType>("ObjectType", i);
            creatureData.maxHp = loader.GetFloat("MaxHp", i);
            creatureData.atk = loader.GetFloat("Atk", i);
            creatureData.def = loader.GetFloat("Def", i);
            creatureData.moveSpeed = loader.GetFloat("MoveSpeed",i);
            creatureData.prefabName = loader.GetString("PrefabName", i);
            creatureData.skillList = loader.GetList_Int("SkillList", i);

            _datas.Add(creatureData);
        }

        loader.Clear();
    }
}
