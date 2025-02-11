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
                
                if(mon != null)
                {
                    float damage = _skillData.damage;
                    mon.OnDamaged(_owner, damage);
                    //  해당 프로젝타일 풀링
                    Managers.Instance.PoolManager.Push(gameObject);
                }

                break;
            }

            _trans.Translate(Vector3.forward * moveDis);

            yield return null;
        }
    }

    
}
