using System.Collections;
using System.Collections.Generic;
using static Define;

public class SkillData
{
    public int skillId;
    public string skillLocalName;        // ���� �̸�
    public string prefabName;           // ������ �̸� 
    public float coolTime;                  // ��ų ��Ÿ��
    public float damage;                    // ������  
    public float damageIncRate;          // ������ ������
    public float duration;                   // ���� �ð�
    public float projectileSpeed;           // �߻�ü �ӵ�
    public float scaleIncRate;              // ũ�� ������
    public int startCreateCount;            // �ʱ� ���� ��
    public float value;                           // ���� ��
    public string castSound;                // ��� ����
    public string skillImage;                 // ��ų �̹���
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
