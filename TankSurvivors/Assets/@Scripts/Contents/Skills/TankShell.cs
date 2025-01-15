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
        // 날라가서 충돌 체크
        yield return null;
    }

}
