using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogController : BaseController
{
    [SerializeField]
    private float _clearRadius = 10f; // �÷��̾� �ֺ��� �Ȱ��� ���� �ݰ��Դϴ�.
    [SerializeField]
    private float _transitionWidth = 5f; // �Ȱ��� ������ ��ȯ�Ǵ� ���Դϴ�.
    [SerializeField]
    private Color _fogColor = Color.gray; // �⺻ �Ȱ� �����Դϴ�.
    [SerializeField]
    private float _fogDensity = 0.05f; // �⺻ �Ȱ� �е��Դϴ�.
    private PlayerController _player; // �÷��̾��� Transform�� �����մϴ�.

    private Coroutine _corFogAroundPlayer = null;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    public void SetFog(PlayerController player)
    {
        _player = player;

        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogColor = _fogColor;

        if (_corFogAroundPlayer != null)
        {
            StopCoroutine(_corFogAroundPlayer);
            _corFogAroundPlayer = null;
        }

        _corFogAroundPlayer = StartCoroutine(CorFogAroundPlayer());
    }

    IEnumerator CorFogAroundPlayer()
    {
        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

        while(true)
        {
            Vector3 playerPosition = _player.transform.position;
            foreach (Renderer renderer in FindObjectsOfType<Renderer>())
            {
                Vector3 objectPosition = renderer.transform.position;
                float distanceToPlayer = Vector3.Distance(playerPosition, objectPosition);

                if (distanceToPlayer < _clearRadius)
                {
                    renderer.material.SetFloat("_FogDensity", 0);
                }
                else if (distanceToPlayer < _clearRadius + _transitionWidth)
                {
                    float t = (distanceToPlayer - _clearRadius) / _transitionWidth;
                    renderer.material.SetFloat("_FogDensity", Mathf.Lerp(0, _fogDensity, t));
                }
                else
                {
                    renderer.material.SetFloat("_FogDensity", _fogDensity);
                }
            }

            yield return waitForEndOfFrame;
        }
    }

}
