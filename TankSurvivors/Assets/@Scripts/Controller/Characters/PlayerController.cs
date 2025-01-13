using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CreatureController
{
    [SerializeField]
    private Transform _tankBody = null;
    //[SerializeField]
    //private Transform _tankTurret = null;

    private Vector2 _moveDir = Vector2.zero;
    public Vector2 MoveDir { get { return _moveDir; } set { _moveDir = value; } }

    private Vector3 _prevBodyDir = Vector3.zero;
    private Vector3 _turretDir = Vector3.zero;
    //  private CharacterController _characterController;
    private Rigidbody _tankRigid = null;

    // Damaged Color
    private MeshRenderer[] _meshRenderers;
    private MaterialPropertyBlock _materialProperty;

    public override bool Init()
    {
        if(base.Init() == false)
            return false;

        //_characterController = GetComponent<CharacterController>(); 
        _tankRigid = GetComponent<Rigidbody>();

        // renderer
        _meshRenderers = GetComponentsInChildren<MeshRenderer>();
        _materialProperty = new MaterialPropertyBlock();

        return true;
    }

    public override void FixedUpdateController()
    {
        base.FixedUpdateController();
        PlayerMove();
    }

    public override void UpdateController()
    {
        base.UpdateController();  
        CollectEnv();
    }

    private void PlayerMove()
    {
        Vector3 moveDir = Managers.Instance.ObjectManager.Player.MoveDir;
        Vector3 dir = moveDir * Time.deltaTime * _creatureData.moveSpeed;

        Vector3 newPos = new Vector3(transform.position.x + dir.x, transform.position.y, transform.position.z + dir.y);
       // transform.position = newPos;

        _tankRigid.MovePosition(newPos);

        //Vector3 dir3D = new Vector3(dir.x, 0f, dir.y);
       // _characterController.Move(dir3D);
        
        if(moveDir != Vector3.zero)
            _tankBody.rotation = Quaternion.Euler(0, Mathf.Atan2(moveDir.x, moveDir.y) * Mathf.Rad2Deg, 0);
        
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public override void OnDead()
    {
        _isAlive = false;
        gameObject.SetActive(false);
    }

    private void CollectEnv()
    {

    }

    public override void OnDamaged(BaseController attacker, float damage)
    {
        base.OnDamaged(attacker, damage);

        SetDamagedColor();
        Invoke("SetDefaultColor", 0.2f);
    }

    void SetDamagedColor()
    {
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
