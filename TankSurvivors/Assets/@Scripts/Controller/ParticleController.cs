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
                    // Ǯ���� �� ���¶�� 
                    Managers.Instance.PoolManager.Push(gameObject);
                    break;
                }
                else
                {
                    // Ǯ���� �ȵǴ� ��ƼŬ�� ���
                    Destroy(gameObject);
                    break;
                }
            }
            yield return null;
        }
    }


}
