using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossZombieSkill01 : ActionSkill
{
    private Vector3 _playerPos = Vector3.zero;
    private Coroutine _corMoveToPlayer = null;
    private float _speed = 5f;

    public override void UseSkill(CreatureController owner)
    {
        base.UseSkill(owner);

        _playerPos = GameManager.Instance.Player.transform.position;

        if (_corMoveToPlayer != null)
            StopCoroutine(_corMoveToPlayer);

        _corMoveToPlayer = StartCoroutine(CorMoveToPlayer());
    }

    private IEnumerator CorMoveToPlayer()
    {
        while (true)
        {
            Vector3 dir = _playerPos - _owner.transform.position;
            dir.y = 0;

            // ��ų ���� ������ ������ �ʴ� ���
            if (SkillData.attackRange < dir.magnitude)
            {
                // �÷��̾�� �̵�
                _owner.transform.Translate(dir.normalized * Time.deltaTime * _speed, Space.World);
            }
            else
            {
                yield break;
            }

            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (RemainCoolTime > 0)
        {
            RemainCoolTime -= Time.deltaTime;
        }
    }
}
