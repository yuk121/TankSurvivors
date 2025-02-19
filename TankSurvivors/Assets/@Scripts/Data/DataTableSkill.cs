using System.Collections;
using System.Collections.Generic;
using static Define;

public class SkillData
{
    public int skillId;
    public string skillLocalName;        // 로컬 이름
    public string prefabName;           // 프리팹 이름 
    public float coolTime;                  // 스킬 쿨타임
    public float damage;                    // 데미지  
    public float damageIncRate;          // 데미지 증가량
    public float duration;                   // 지속 시간
    public float projectileSpeed;           // 발사체 속도
    public float scaleIncRate;              // 크기 증가량
    public int startCreateCount;            // 초기 생성 수
    public float value;                           // 공용 값
    public string castSound;                // 사용 사운드
    public string skillImage;                 // 스킬 이미지
}

public class DataTableSkill
{
    private List<SkillData> _dataList = new List<SkillData>();
    public List<SkillData> DataList { get => _dataList; }
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
        
        SkillData skillData;
        for (int i =0; i < loader.GetLength(); i++)
        {
            skillData = new SkillData();

            skillData.skillId = loader.GetInt("SkillId",i);
            skillData.skillLocalName = loader.GetString("SkillLocalName", i);
            skillData.prefabName = loader.GetString("PrefabName", i);
            skillData.coolTime = loader.GetFloat("CoolTime", i);
            skillData.damage = loader.GetFloat("Damage", i);
            skillData.damageIncRate = loader.GetFloat("DamgaIncRate", i);
            skillData.duration = loader.GetFloat("Duration", i);
            skillData.projectileSpeed = loader.GetFloat("ProjectileSpeed", i);
            skillData.scaleIncRate = loader.GetFloat("ScaleIncRate", i);
            skillData.startCreateCount = loader.GetInt("StartCreateCount", i);
            skillData.value = loader.GetFloat("Value", i);
            skillData.castSound = loader.GetString("CastSound", i);
            skillData.skillImage = loader.GetString("SkillImage", i);

            _dataList.Add(skillData);  
        }

        loader.Clear();
    }

    public SkillData GetSkillData(int skillId)
    {
        SkillData data = new SkillData();

        foreach(var skillData in _dataList)
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
