using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BaseController : MonoBehaviour
{
    public eObjectType _objectType { get; protected set; }
    protected bool _init = false;

    public virtual bool Init()
    {
        if (_init == false)
        {
            return false;
        }

        _init = true;
        return true;
    }

    private void Awake()
    {
        //Init();
    }
    
    private void FixedUpdate()
    {
        FixedUpdateController();
    }

    private void Update()
    {
        UpdateController();
    }

    public virtual void FixedUpdateController() { }
    public virtual void UpdateController() { }
}
