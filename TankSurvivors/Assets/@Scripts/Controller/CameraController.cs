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
            Ray ray = new Ray(_cameraTrans.position, _cameraTrans.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f)) // �ִ� 100m �Ÿ����� Ž��
            {
                _groundCenterPos = hit.point;
            }

            transform.position = _target.position;
        }
    }

    private void OnDrawGizmos()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        float camNear = cam.nearClipPlane; // ī�޶��� Near Clip �Ÿ�

        // ī�޶� ������ ��ǥ ���ϱ� (Near Clip�� Far Clip)
        Vector3 nearBottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, camNear)); // ���ϴ� (Near)
        Vector3 nearBottomRight = cam.ViewportToWorldPoint(new Vector3(1, 0, camNear)); // ���ϴ� (Near)
        Vector3 nearTopLeft = cam.ViewportToWorldPoint(new Vector3(0, 1, camNear)); // �»�� (Near)
        Vector3 nearTopRight = cam.ViewportToWorldPoint(new Vector3(1, 1, camNear)); // ���� (Near)

        //Vector3 nearBottomMid = (nearBottomLeft + nearBottomRight) / 2f; // �Ʒ��� �� ���� (Near)
        //Vector3 nearTopMid = (nearTopLeft + nearTopRight) / 2f; // ���� �� ���� (Near)
        //Vector3 nearLeftMid = (nearBottomLeft + nearTopLeft) / 2f; // ���� �� ���� (Near)
        //Vector3 nearRightMid = (nearBottomRight + nearTopRight) / 2f; // ������ �� ���� (Near)

        //Vector3 groundBottomMid = Utils.GetCamRayToGroundPos(cam, nearBottomMid);
        //Vector3 groundTopMid = Utils.GetCamRayToGroundPos(cam, nearTopMid);
        //Vector3 groundLeftMid = Utils.GetCamRayToGroundPos(cam, nearLeftMid);
        //Vector3 groundRightMid = Utils.GetCamRayToGroundPos(cam, nearRightMid);

        Vector3 groundBottomLeft = Utils.GetCamRayToGroundPos(cam, nearBottomLeft);
        Vector3 groundBottomRight = Utils.GetCamRayToGroundPos(cam, nearBottomRight);
        Vector3 groundTopLeft = Utils.GetCamRayToGroundPos(cam, nearTopLeft);
        Vector3 groundTopRight = Utils.GetCamRayToGroundPos(cam, nearTopRight);

        // Gizmos�� �ð�ȭ
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundBottomLeft * 2f, 0.3f);
        Gizmos.DrawSphere(groundBottomRight * 2f, 0.3f);
        Gizmos.DrawSphere(groundTopLeft * 2f, 0.3f);
        Gizmos.DrawSphere(groundTopRight * 2f, 0.3f);

        //// Gizmos�� �ð�ȭ
        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(groundBottomMid, 0.3f);
        //Gizmos.DrawSphere(groundTopMid, 0.3f);
        //Gizmos.DrawSphere(groundLeftMid, 0.3f);
        //Gizmos.DrawSphere(groundRightMid, 0.3f);
    }
}
