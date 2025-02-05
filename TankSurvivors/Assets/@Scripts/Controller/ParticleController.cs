using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : BaseController
{
    [SerializeField]
    private float _duration = 1.5f;

    private Coroutine _corDestroyParticle;
    private float _destroyTime;
    Transform _trans;

    public void Init(Vector3 spawnPos = default(Vector3), Vector3 spawnFoward = default(Vector3))
    {
        base.Init();

        _trans = transform;
        _trans.position = spawnPos;
        _trans.forward = spawnFoward;

        _destroyTime = Time.time + _duration;

        if (_corDestroyParticle != null)
            StopCoroutine(_corDestroyParticle);

        _corDestroyParticle = StartCoroutine(CorDestroyParticle());
    }


    private IEnumerator CorDestroyParticle()
    {
        while (true)
        {
            if (Time.time > _destroyTime)
            {
                Managers.Instance.ResourceManager.Destory(gameObject);
                break;
            }
            yield return null;
        }
    }


}
