using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform _target;
    private Transform _cameraTrans;
    private Vector3 _groundCenterPos;

    private void Start()
    {
        _cameraTrans = Camera.main.transform;
    }

    // Start is called before the first frame update
    // Update is called once per frame
    void LateUpdate()
    {
        if (_target == null)
        {
            if(GameManager.Instance.Player != null) 
            {
                _target = GameManager.Instance.Player.GetComponent<Transform>();
            }
        }
        else
        {
            transform.position = _target.position;
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Camera cam = Camera.main;
    //    if (cam == null) return;

    //    float camNear = cam.nearClipPlane; // 카메라의 Near Clip 거리

    //    // 카메라 꼭짓점 좌표 구하기 (Near Clip과 Far Clip)
    //    Vector3 nearBottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, camNear)); // 좌하단 (Near)
    //    Vector3 nearBottomRight = cam.ViewportToWorldPoint(new Vector3(1, 0, camNear)); // 우하단 (Near)
    //    Vector3 nearTopLeft = cam.ViewportToWorldPoint(new Vector3(0, 1, camNear)); // 좌상단 (Near)
    //    Vector3 nearTopRight = cam.ViewportToWorldPoint(new Vector3(1, 1, camNear)); // 우상단 (Near)

    //    Vector3 groundBottomLeft = Utils.GetCamRayToGroundPos(cam, nearBottomLeft);
    //    Vector3 groundBottomRight = Utils.GetCamRayToGroundPos(cam, nearBottomRight);
    //    Vector3 groundTopLeft = Utils.GetCamRayToGroundPos(cam, nearTopLeft);
    //    Vector3 groundTopRight = Utils.GetCamRayToGroundPos(cam, nearTopRight);

    //    // Gizmos로 시각화
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawSphere(groundBottomLeft, 0.3f);
    //    Gizmos.DrawSphere(groundBottomRight, 0.3f);
    //    Gizmos.DrawSphere(groundTopLeft, 0.3f);
    //    Gizmos.DrawSphere(groundTopRight, 0.3f);
    //}
}
