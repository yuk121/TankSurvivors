using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBase : MonoBehaviour
{
    private const int MAX_SKILLLEVEL = 5;

    private SkillData _skillData = new SkillData();
    public SkillData SkillData { get => _skillData; set => _skillData = value; }

    private Define.eSkillType _skillType = Define.eSkillType.None;
    public Define.eSkillType SkillType  { get => _skillType; set => _skillType = value; } 

    private int _curSkillLevel = 0;
    public int CurSkillLevel { get => _curSkillLevel; }

    private float _remainCoolTime = 0;        // 쿨타임 남은 시간
    public float RemainCoolTime { get => _remainCoolTime; set => _remainCoolTime = value; }

    private int _Index;    // 스킬 순서
    public int Index { get => _Index; set => _Index = value; }

    public void StartCoolTime()
    {
        _remainCoolTime = SkillData.coolTime;
    }

    public void RemoveCoolTime()
    {
        _remainCoolTime = 0f;
    }

    public void SkillLevelup()
    {
        if (_curSkillLevel <= MAX_SKILLLEVEL)
            _curSkillLevel++;
    }
    
    public virtual void UseSkill()
    {
        // 스킬 사용했으니 쿨타임 적용
        StartCoolTime();
    }

    protected virtual void GenerateSkillPrefab(Transform spawnPos)
    {
        string prefabName = _skillData.prefabName;
        string skillPrefabPath = $"SkillPrefab/{prefabName}.prefab";

        GameObject go = Managers.Instance.ResourceManager.Instantiate(skillPrefabPath, pooling: true);
        go.name = _skillData.prefabName;
        go.transform.position = spawnPos.position;
        go.transform.forward = spawnPos.forward;
    }

    private void Update()
    {
        if(RemainCoolTime > 0)
        {
            RemainCoolTime -= Time.deltaTime;
        }
    }
}
