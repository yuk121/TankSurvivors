using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetection : BaseController
{
    // 초당 충돌 체크
    const float COLLISION_CHECK_INTERVAL = 1f;

    private SkillData _skillData;
    private CreatureController _owner;
    
    Define.eSkillType _skillType;
    private float _duration;
    private float _radius;
    private float _checkTime = 0;

    public void SetData(SkillData skillData, CreatureController owner, float radius, Define.eSkillType skillType)
    {
        _skillData = skillData;
        _owner = owner;
        _radius = radius;
        _skillType = skillType;
        _duration = skillData.duration + Time.time;     
    }

    public void OnUpdateRadius(float radius)
    {
        _radius = radius;
    }

    private void Update()
    {

        switch (_skillType)
        {
            case Define.eSkillType.ElectircField:
                // 초당 데미지 
                if (_checkTime < Time.time)
                {
                    Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);

                    foreach (Collider collider in colliders)
                    {
                        MonsterController mon = collider.GetComponent<MonsterController>();

                        if (mon != null)
                        {
                            float damage = _skillData.damage;
                            mon.OnDamaged(_owner, damage);     
                        }
                    }

                    _checkTime = Time.time + COLLISION_CHECK_INTERVAL;
                }
                break;


            case Define.eSkillType.Mine:
                if (_duration < Time.time)
                {
                    // duration 동안 몬스터 감지 못한 경우 자동폭발
                    CreateHitEffect();
                    Managers.Instance.ObjectManager.Despawn(this);
                }
                return;
        }
    }

    private void CreateHitEffect()
    { 
        string effectPrefabPath = $"SkillPrefab/{_skillData.hitEffectPrefab}.prefab";
       
        GameObject effect = Managers.Instance.ResourceManager.Instantiate(effectPrefabPath, pooling : true);

        effect.transform.position = gameObject.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null)
            return;

        MonsterController mon = other.GetComponent<MonsterController>();
        if (mon == null)
            return;
        float damage = _skillData.damage;
        switch (_skillType)
        {
            case Define.eSkillType.SubTank:
                mon.OnDamaged(_owner, damage);
                break;
            case Define.eSkillType.Mine:
                mon.OnDamaged(_owner, damage);
                CreateHitEffect();
                Managers.Instance.ObjectManager.Despawn(this);
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
