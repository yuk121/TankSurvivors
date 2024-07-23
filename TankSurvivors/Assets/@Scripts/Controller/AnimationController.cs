using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator _animator;
    private Define.eCreatureAnimState _prevAnimState = Define.eCreatureAnimState.None;

    public void Pause()
    {
        _animator.enabled = false;
    }

    public void Resume()
    {
        _animator.enabled = true;
    }

    public void Play(Define.eCreatureAnimState parameter, bool isFade)
    {
        if (_prevAnimState != Define.eCreatureAnimState.None)
        {
            _animator.ResetTrigger(_prevAnimState.ToString());
            _prevAnimState = Define.eCreatureAnimState.None;
        }

        // 블렌드 트리를 사용하는 경우
        if (isFade == true)
        {
            _animator.SetTrigger(parameter.ToString());
        }
        else
        {
            _animator.Play(parameter.ToString(), 0, 0.0f);
        }

        _prevAnimState = parameter;
    }
}
