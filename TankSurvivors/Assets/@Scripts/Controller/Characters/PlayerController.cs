using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CreatureController
{
    [SerializeField]
    private Transform _tankBody = null;
    [SerializeField]
    private Transform _tankTurret = null;

    private Vector2 _moveDir = Vector2.zero;
    public Vector2 MoveDir { get { return _moveDir; } set { _moveDir = value; } }

    private Vector3 _prevBodyDir = Vector3.zero;
    private Vector3 _turretDir = Vector3.zero;
    //  private CharacterController _characterController;
    private Rigidbody _tankRigid = null;

    // Start is called before the first frame update

    public override bool Init()
    {
        if(base.Init() == false)
            return false;

        _speed = 5f;
        //_characterController = GetComponent<CharacterController>(); 
        _tankRigid = GetComponent<Rigidbody>();
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
        Vector3 dir = moveDir * _speed * Time.deltaTime;

        Vector3 newPos = new Vector3(transform.position.x + dir.x, transform.position.y, transform.position.z + dir.y);
       // transform.position = newPos;

        _tankRigid.MovePosition(newPos);

        //Vector3 dir3D = new Vector3(dir.x, 0f, dir.y);
       // _characterController.Move(dir3D);
        
        if(moveDir != Vector3.zero)
            _tankBody.rotation = Quaternion.Euler(0, Mathf.Atan2(moveDir.x, moveDir.y) * Mathf.Rad2Deg, 0);
        
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    private void CollectEnv()
    {

    }
}
