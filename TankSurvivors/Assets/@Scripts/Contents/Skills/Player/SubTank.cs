using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class SubTank : ActionSkill
{
    List<HitDetection> _subTankList = new List<HitDetection>();
    CreatureController _owner = null;
    [SerializeField]
    float _rotationSpeed = 200f;
    float _angle = 0f;
    float _fixedY = 0.4f;
    public override void UseSkill(CreatureController owner)
    {
        base.UseSkill(owner);
        _owner = owner;

        UpdateSubTank();
    }

    private void UpdateSubTank()
    {
        int createCount = SkillData.startCreateCount;

        // 1. 탱크숫자가 적은 경우 null 로 초기 리스트에 추가
        while (_subTankList.Count < createCount)
        {
            _subTankList.Add(null);
        }

        // 2. 스킬 정보에 맞게 서브탱크 생성
        for (int i = 0; i < createCount; i++)
        {
            float angle = (360f / SkillData.startCreateCount) * i;
            float radian = angle * Mathf.Deg2Rad;

            float cos = Mathf.Cos(radian);
            float sin = Mathf.Sin(radian);

            Vector3 spawnPoint = _owner.transform.position + (new Vector3(
                sin, 0f, cos) * Define.SUBTANK_SPAWN_RADIUS);

            Quaternion rotation = Quaternion.AngleAxis(angle, _owner.transform.up);
            Vector3 spawnDir = rotation * Vector3.forward;

            // 존재하지 않은 탱크라면 생성
            if (_subTankList[i] == null)
            {
                HitDetection subTank = Managers.Instance.ObjectManager.Spawn<HitDetection>(spawnPoint, SkillData.skillId, spawnDir);

                subTank.transform.SetParent(_owner.transform);

                subTank.SetData(SkillData, _owner, Define.SUBTANK_DETECT_RADIUS);

                _subTankList[i] = subTank;
            }
            else
            {
                // 존재하는 경우 위치만 변경
                _subTankList[i].transform.position = new Vector3(spawnPoint.x, _subTankList[i].transform.position.y, spawnPoint.z);
                _subTankList[i].transform.forward = spawnDir;
            }
        }
    }

    public override void OnUpdatedSkill()
    {
        base.OnUpdatedSkill();

        UpdateSubTank();
    }


    private void Update()
    {
        if(_subTankList.Count > 0) 
        {
            _angle += +(Time.deltaTime * _rotationSpeed);
            for (int i = 0; i < _subTankList.Count; i++)
            {
                float angleOffset = (360f / _subTankList.Count) * i;
                float radian = (_angle + angleOffset) * Mathf.Deg2Rad;
                float cos = Mathf.Cos(radian);
                float sin = Mathf.Sin(radian);

                _subTankList[i].transform.position = _owner.transform.position + new Vector3(
                    sin, 0, cos) * Define.SUBTANK_SPAWN_RADIUS;

                _subTankList[i].transform.position = new Vector3(_subTankList[i].transform.position.x, _fixedY, _subTankList[i].transform.position.z);

                _subTankList[i].transform.rotation = Quaternion.Euler(0f, radian * Mathf.Rad2Deg, 0f);
            }
        }
    }
}
