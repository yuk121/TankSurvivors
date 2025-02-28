using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectircField : ActionSkill
{
    private CreatureController _owner = null;
    private HitDetection _electricField = null;
    private float _value = 0f;
    private float _duration = 0f;
    private float _remainTime = 0f;
    private float _prevSkillLevel = 0f;

    public override void UseSkill(CreatureController owner)
    {
        base.UseSkill(owner);
        _owner = owner;
        
        UpdateElectricField();
    }

    public void UpdateElectricField()
    {
        // ù ����
        if (_electricField == null)
        {
            _electricField = Managers.Instance.ObjectManager.Spawn<HitDetection>(_owner.transform.position, SkillData.skillId, bPooling : false);

            _electricField.SetData(SkillData, _owner, Define.ELECTRIC_FIELD_DETECT_RADIUS, Define.eSkillType.ElectircField);

            _electricField.transform.SetParent(_owner.transform);
            _electricField.transform.position += Vector3.up * -0.8f;

            _electricField.transform.localScale = Vector3.one;
        }
        else
        {
            // �������� �� ��� �ݰ� ��ġ ������Ʈ
            if(_prevSkillLevel != CurSkillLevel)
            {
                 _value += SkillData.scaleIncRate;

                _electricField.transform.localScale = new Vector3(1f + _value, 0f, 1f + _value);

                float incRadius = Define.ELECTRIC_FIELD_DETECT_RADIUS + (Define.ELECTRIC_FIELD_RADIUS_INC_RATE * (CurSkillLevel-1));
                
                _electricField.OnUpdateRadius(incRadius);
            }
        }

        // ��Ÿ�� ������ �ٽ� ���� ��� Ȱ��ȭ
        _electricField.gameObject.SetActive(true);
       
        // ���ӽð� ����
        _duration = SkillData.duration;
        
        // ���� ���� ����
        _prevSkillLevel = CurSkillLevel;
    }

    public override void OnUpdatedSkill()
    {
        base.OnUpdatedSkill();
        UpdateElectricField();
    }

    public void Update()
    {
        if (_electricField != null)
        {
            // ���� �ð������� ��� �����ִ´�.
            if (_duration > 0)
            {
                _duration -= Time.deltaTime;
            }
            else
            {
                // ���ӽð��� ���� ��� ���� ��Ÿ�� ���
                if(_electricField.gameObject.activeSelf == true)
                    _electricField.gameObject.SetActive(false);

                RemainCoolTime -= Time.deltaTime;
            }
        }
    }
}
