using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CreatureController
{
    private const float ENV_COLLECT_DISTANCE_BASIC = 1.5f;

    [Header("BASE")]
    [SerializeField]
    private Transform _tankBody = null;
    [SerializeField]
    private Transform _tankHead = null;
    [SerializeField]
    private Transform _dummyFirePos = null;
    public Transform DummyFirePos { get => _dummyFirePos; }

    [Header("UI")]
    [SerializeField]
    private Transform _dummyIndicatorPos = null;
    public Transform DummyIndicatorPos { get => _dummyIndicatorPos; }
    [SerializeField]
    private UI_PlayerHpBar _playerHpBar = null;

    private Vector2 _moveDir = Vector2.zero;
    public Vector2 MoveDir { get { return _moveDir; } set { _moveDir = value; } }
    private Rigidbody _tankRigid = null;

    // Damaged Color
    private MeshRenderer[] _meshRenderers;
    private MaterialPropertyBlock _materialProperty;

    // Stat
    public int CurExp { get; set; } = 0;
    public int CurLevel { get; set; } = 1;

    // Etc
    private bool _isPuase = false;
    public bool IsPause { get => _isPuase; set => _isPuase = value; }
    private bool _skillAllStop = false;

    public override bool Init()
    {
        if(base.Init() == false)
            return false;

        ObjectType = Define.eObjectType.Player;
        _tankRigid = GetComponent<Rigidbody>();

        // UI
        _playerHpBar.SetPlayer(this);

        // renderer
        _meshRenderers = GetComponentsInChildren<MeshRenderer>();
        _materialProperty = new MaterialPropertyBlock();

        // �⺻ ������ �̸� 1������ ����
        _skillBook.UpgradeSkill(_skillBook.ActionSkillList[0].SkillData.skillId);
        
        // Stat �ʱ�ȭ
        CurExp = 0;
        CurLevel = 1;

        return true;
    }

    public override void FixedUpdateController()
    {
        if (GameManager.Instance.Pause == true)
            return;

        base.FixedUpdateController();
        PlayerMove();
    }

    public override void UpdateController()
    {
        if (GameManager.Instance.Pause == true )
            return;

        base.UpdateController();
        UseAutoSkill();
        CollectEnv();

#if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            GetExp(Managers.Instance.DataTableManager.DataTableInGameLevel.GetNextLevelRequiredExp(CurLevel));
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            _skillAllStop = (!_skillAllStop);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Managers.Instance.ObjectManager.AllKillMonsters();
        }
#endif
    }

    private void PlayerMove()
    {
        Vector3 moveDir = Managers.Instance.ObjectManager.Player.MoveDir;
        Vector3 dir = moveDir * Time.deltaTime * _creatureData.moveSpeed;

        // �̵�
        Vector3 newPos = new Vector3(transform.position.x + dir.x, transform.position.y, transform.position.z + dir.y);
        _tankRigid.MovePosition(newPos);

        // ȸ��
        if(moveDir != Vector3.zero)
            _tankBody.rotation = Quaternion.Euler(0, Mathf.Atan2(moveDir.x, moveDir.y) * Mathf.Rad2Deg, 0);
    
        // TODO : �����̴� �Ҹ� �߰�
    }

    public override void OnDead()
    {
        // TODO : ������ ȿ�� �߰�
        _isAlive = false;
        gameObject.SetActive(false);
    }

    // �÷��̾��� ��ų ��Ͽ� ���缭 �ڵ����� ��ų ���
    private void UseAutoSkill()
    {
#if UNITY_EDITOR
        if (_skillAllStop == true)
            return;
#endif

        for (int i = 0; i < _skillBook.ActionSkillList.Count; i++)
        {
            // ��ų ������ 0�̶�� ���
            if (_skillBook.ActionSkillList[i].CurSkillLevel < 1)
                continue;

            // ��ų ��Ÿ�� Ȯ���ϱ�
            if (_skillBook.ActionSkillList[i].RemainCoolTime > 0)
                continue;

            if(_skillBook.ActionSkillList[i].SkillType == Define.eSkillType.TankShell)
            {
                // �⺻������ ��ź ��ų�� ��� ��ũ�� �Ӹ��� ������ ����Ѵ�.
                // ���� ��ó�� �� ��ġ Ž��
                MonsterController mon = GetNearEnemy();

                if (mon != null && mon.CheckAlive() == true)
                {
                    // ������ ���� ���� �Ӹ����� ����
                    Vector3 dir = mon.transform.position - _tankHead.position;
                    dir.y = 0;
                    _tankHead.transform.forward = dir;
                }
            }

            // ��ų ���
            _skillBook.ActionSkillList[i].UseSkill(this);
        }
    }

    private MonsterController GetNearEnemy()
    {
        MonsterController nearEnemy = null;

        float minDis = float.MaxValue;
        float curDis = 0f;

        foreach(MonsterController mc in Managers.Instance.ObjectManager.Monsters)
        {
            curDis = Vector3.Distance(_tankBody.position, mc.transform.position);
            
            if(curDis < minDis)
            {
                minDis = curDis;
                nearEnemy = mc;
            }
        }

        return nearEnemy;
    }

    // �ֺ��� �ִ� �ڿ� �ݱ�
    private void CollectEnv()
    {
        List<DropItemController> findDropItems = GridManager.Instance.GatherDropObjects(transform.position, ENV_COLLECT_DISTANCE_BASIC + 0.5f);
        float collectSqr = ENV_COLLECT_DISTANCE_BASIC * ENV_COLLECT_DISTANCE_BASIC;

        foreach(DropItemController item in findDropItems)
        {
            Vector3 dir = item.transform.position - transform.position;
       
            if(dir.sqrMagnitude <= collectSqr)
            {
                switch(item.ItemType)
                {
                    case Define.eObjectType.Bomb:
                    case Define.eObjectType.HpRecorvery:
                    case Define.eObjectType.Magnet:
                    case Define.eObjectType.Box:
                        item.GetItem();
                        break;

                    default:
                        item.GetItem();
                        break;
                }
            }
        }

    }

    public void GetExp(int exp)
    {
        CurExp += exp;

        int nextRequiredExp = Managers.Instance.DataTableManager.DataTableInGameLevel.GetNextLevelRequiredExp(CurLevel);
        
        // �������� ������ ����ġ
        if (CurExp >= nextRequiredExp)
        {
            CurExp -= nextRequiredExp;

            CurLevel++;

            UIPopup_SkillSelect popup = Managers.Instance.UIMananger.OpenPopup<UIPopup_SkillSelect>(true);

            popup.SetSkillSelect();

            if (GameManager.Instance != null && GameManager.Instance.GameData != null)
            {
                GameManager.Instance.GameData.firstLevelUp = true;
            }
        }
    }

    public override void OnDamaged(BaseController attacker, float damage)
    {
        // TODO : �´� �Ҹ� �߰�

        base.OnDamaged(attacker, damage);

        SetDamagedColor();
        Invoke("SetDefaultColor", 0.2f);
    }

    public void OnRecovery(float recoveryRate)
    {
        float recoveryValue = CreatureData.maxHp * recoveryRate;

        _curHp = Math.Min(CreatureData.maxHp, _curHp + recoveryValue);
    }



    void SetDamagedColor()
    {
        // �ǰݽ� ��� ������ �Ǵ� ȿ��
        if (_materialProperty != null)
        {
            foreach(Renderer render in _meshRenderers)
            {
                _materialProperty.SetColor("_Color", Color.red);
                render.SetPropertyBlock(_materialProperty);
            }
        }
    }

    void SetDefaultColor()
    {
        // ���� �������� ���ƿ�
        if (_materialProperty != null)
        {
            foreach (Renderer render in _meshRenderers)
            {
                _materialProperty.SetColor("_Color", Color.white);
                render.SetPropertyBlock(_materialProperty);
            }
        }
    }
}
