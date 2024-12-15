using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform _target = null;
    private Transform _trans;

    // Start is called before the first frame update
    public void Init(Transform target)
    {
        _target = target;
        _trans = transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (_target == null)
            return;

        _trans.position = new Vector3(_target.transform.position.x, transform.position.y, _target.transform.position.z);
    }
}
