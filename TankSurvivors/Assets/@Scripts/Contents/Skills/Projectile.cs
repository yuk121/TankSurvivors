using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : BaseController
{
    private SkillData _skillData;
    private Transform _trans;
    private CreatureController _owner;
    private float _destroyTime;

    public void Init(CreatureController owenr , SkillData data, Define.eSkillType skillType)
    {
        _trans = transform;
        _owner = owenr;
        _skillData = data;

        _destroyTime = Time.time + data.duration;

        switch(skillType)
        {
            case Define.eSkillType.TankShell:
                StartCoroutine(CorTankShell());
                break;
        }
    }

    private IEnumerator CorTankShell()
    {
        float speed = _skillData.projectileSpeed;
        while(true)
        {
            if (GameManager.Instance.Pause == true)
            {
                yield return null;
                continue;
            }

            // 지속시간이 지난 경우
            if(Time.time > _destroyTime)
            {
                Managers.Instance.ObjectManager.Despawn(this);
            }

            float moveDis = speed * Time.deltaTime;
            RaycastHit hit;

            //Debug.DrawRay(_trans.position, transform.forward * (moveDis + 0.4f)  , Color.red);

            if (Physics.Raycast(_trans.position,transform.forward, out hit, moveDis +0.15f))
            {
                MonsterController mon = hit.collider.gameObject.GetComponent<MonsterController>();
                
                if(mon != null && mon.IsAlive == true)
                {
                    float damage = _skillData.damage ;
                  
                    // 공격력에 추가 스탯 반영
                    if(_owner is PlayerController)
                    {
                        PlayerController player = (PlayerController)_owner;
                        damage = damage * (1 + player.PlayerBonusStat._bonusAtkRate);
                    }

                    mon.OnDamaged(_owner, damage);
                    //  해당 발사체 풀링
                    Managers.Instance.PoolManager.Push(gameObject);
                }

                break;
            }

            _trans.Translate(Vector3.forward * moveDis);

            yield return null;
        }
    }

    
}
