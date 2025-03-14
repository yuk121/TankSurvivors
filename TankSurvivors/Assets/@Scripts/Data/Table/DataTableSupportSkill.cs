using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportSkillData
{
    public int skillId;
    public string skillLocalName;           // 로컬 이름
    public string skillLocalDesc;             // 로컬 설명
    public float value;                         // 공용 값
    public string skillImage;                 // 스킬 이미지
}

public class DataTableSupportSkill
{
    private List<SupportSkillData> _dataList = new List<SupportSkillData>();
    public List<SupportSkillData> DataList { get => _dataList; }

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

        SupportSkillData skillData;
        for (int i = 0; i < loader.GetLength(); i++)
        {
            skillData = new SupportSkillData();

            skillData.skillId = loader.GetInt("SkillId", i);
            skillData.skillLocalName = loader.GetString("SkillLocalName", i);
            skillData.skillLocalDesc = loader.GetString("SkillLocalDesc", i);
            skillData.value = loader.GetFloat("Value", i);
            skillData.skillImage = loader.GetString("SkillImage", i);

            _dataList.Add(skillData);
        }

        loader.Clear();
    }

    public SupportSkillData GetSupportSkillData(int skillId)
    {
        SupportSkillData data = new SupportSkillData();

        foreach (var skillData in _dataList)
        {
            if (skillData.skillId == skillId)
            {
                data = skillData;
                break;
            }
        }

        return data;
    }
}
