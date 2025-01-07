using System.Collections;
using System.Collections.Generic;
using static Define;

public class SkillData
{
    public int skillId;
    public string skillLocalName;
    public eSkillType skillType;
    public string prefabName;
    public float coolTime;
    public float damage;
    public float damageIncRate;
    public float duration;
    public float castSpeed;
    public float castSpeedIncRate;
    public float scaleIncRate;
    public int startCreateCount;
    public string castSound;
}

public class DataTableSkill
{
    private List<SkillData> _datas = new List<SkillData>();
    public List<SkillData> Datas { get => _datas; }
    private void Unload()
    {
        if (_datas != null && _datas.Count > 0)
        {
            _datas.Clear();
        }
    }
    public void DataLoad(byte[] dataText)
    {
        Unload();
        
        TableLoader loader = TableLoader.Instance;
        loader.LoadTable(dataText);
        
        SkillData skillData;
        for (int i =0; i < loader.GetLength(); i++)
        {
            skillData = new SkillData();

            skillData.skillId = loader.GetInt("SkillId",i);
            skillData.skillLocalName = loader.GetString("SkillLocalName", i);
            skillData.skillType = loader.GetEnum<eSkillType>("SkillType", i);
            skillData.prefabName = loader.GetString("PrefabName", i);
            skillData.coolTime = loader.GetFloat("CoolTime", i);
            skillData.damage = loader.GetFloat("Damage", i);
            skillData.damageIncRate = loader.GetFloat("DamgaIncRate", i);
            skillData.duration = loader.GetFloat("Duration", i);
            skillData.castSpeed = loader.GetFloat("CastSpeed", i);
            skillData.castSpeedIncRate = loader.GetFloat("CastSpeedIncRate", i);
            skillData.scaleIncRate = loader.GetFloat("ScaleIncRate", i);
            skillData.startCreateCount = loader.GetInt("StartCreateCount", i);
            skillData.castSound = loader.GetString("CastSound", i);

            _datas.Add(skillData);  
        }

        loader.Clear();
    }

    public SkillData GetSkillData(int skillId)
    {
        SkillData data = new SkillData();

        foreach(var skillData in _datas)
        {
            if(skillData.skillId == skillId)
            {
                data = skillData;
                break;
            }
        }

        return data;
    }
}
