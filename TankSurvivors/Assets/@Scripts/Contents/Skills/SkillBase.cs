using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBase
{
    private const int MAX_SKILLLEVEL = 5;

    private SkillData _skillData = new SkillData();
    public SkillData SkillData { get => _skillData; set => _skillData = value; }

    private int _curSkillLevel = 0;
    public int CurSkillLevel { get => _curSkillLevel; }

    private float _remainCoolTime = 0;        // 쿨타임 남은 시간
    public float RemainCoolTime { get => _remainCoolTime; set => _remainCoolTime = value; }

    private int _Index;    // 스킬 순서
    public int Index { get => _Index; set => _Index = value; }

    public void SkillUpgrade()
    {
        _curSkillLevel++;

        // TODO : 스킬 레벨업에 따라 다음 스킬 데이터 가져오기 및 맥스 상태인 경우 확인하기
    }

    public void SetCoolTime()
    {
        _remainCoolTime = SkillData.coolTime;
    }

    public void RemoveCoolTime()
    {
        _remainCoolTime = 0f;
    }
}
