using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : BaseController
{
    [SerializeField]
    private float _duration = 1.5f;

    private Coroutine _corDestroyParticle;
    private GameObject _goParticle;
    private float _destroyTime;
    private bool _isPooling = false;

    public void SetPooling()
    {
        _isPooling = true;
        SetDestroy();
    }

    public void SetDestroy()
    {
        _destroyTime = Time.time + _duration;

        if( _corDestroyParticle != null )
            StopCoroutine(_corDestroyParticle );

        _corDestroyParticle = StartCoroutine(CorDestroyParticle());

    }

    private IEnumerator CorDestroyParticle()
    {
        while (true)
        {
            if (Time.time > _destroyTime)
            {
                if (_isPooling == true)
                {
                    // 풀링이 된 상태라면 
                    Managers.Instance.PoolManager.Push(gameObject);
                    break;
                }
                else
                {
                    // 풀링이 안되는 파티클인 경우
                    Destroy(gameObject);
                    break;
                }
            }
            yield return null;
        }
    }


}
