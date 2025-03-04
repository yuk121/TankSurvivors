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
    private Coroutine _corOnSkillDamage;
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

    public void OnSkillDamage()
    {
        if (_corOnSkillDamage != null)
            StopCoroutine(_corOnSkillDamage);

        _corOnSkillDamage = StartCoroutine(CorOnSkillDamage());
    }

    public void OnUpdateRadius(float radius)
    {
        _radius = radius;
    }

    private IEnumerator CorOnSkillDamage()
    {
        float particleDelay = 0.2f;
        while (true)
        {
            switch (_skillType)
            {
                case Define.eSkillType.ElectircField:
                    // 초당 데미지 
                    if (_checkTime < Time.time)
                    {
                        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);

                        yield return new WaitForSeconds(particleDelay);

                        foreach (Collider collider in colliders)
                        {
                            MonsterController mon = collider.GetComponent<MonsterController>();

                            if (mon != null && mon.IsAlive == true)
                            {
                                // 넉백
                                KnockBack(mon);

                                float damage = _skillData.damage;

                                if (_owner is PlayerController)
                                {
                                    PlayerController player = (PlayerController)_owner;
                                    damage = damage * (1 + player.PlayerBonusStat._bonusAtkRate);
                                }

                                mon.OnDamaged(_owner, damage);
                            }
                        }

                        _checkTime = Time.time + COLLISION_CHECK_INTERVAL;
                    }
                    break;
            }

            yield return null;
        }
    }

    private void CreateHitEffect()
    { 
        string effectPrefabPath = $"SkillPrefab/{_skillData.hitEffectPrefab}.prefab";
       
        GameObject effect = Managers.Instance.ResourceManager.Instantiate(effectPrefabPath, pooling : true);

        effect.transform.position = gameObject.transform.position;
    }

    public void KnockBack(MonsterController mon)
    {
        // 몬스터가 향하는 방향의 반대 방향
        Vector3 knockDir = -(_owner.transform.position - mon.transform.position).normalized;
        knockDir.y = 0;

        Rigidbody rigidbody = mon.GetComponent<Rigidbody>();

        rigidbody.AddForce(knockDir * Define.KNOCKBACK_FORCE, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null)
            return;

        MonsterController mon = other.GetComponent<MonsterController>();
        if (mon == null || mon.IsAlive == false)
            return;
       
        float damage = _skillData.damage;
        
        if (_owner is PlayerController)
        {
            PlayerController player = (PlayerController)_owner;
            damage = damage * (1 + player.PlayerBonusStat._bonusAtkRate);
        }

        switch (_skillType)
        {
            case Define.eSkillType.SubTank:
                // 넉백
                KnockBack(mon);
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
