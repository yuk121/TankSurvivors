using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankShell : MonoBehaviour
{
    SkillData _skillData;
    Transform _targetPos;

    public void Init(SkillData data, Transform targetPos)
    {
        _skillData = data;
        _targetPos = targetPos;
    }

    private IEnumerator CorSkill()
    {
        // ���󰡼� �浹 üũ
        yield return null;
    }

}
