using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Utils
{
    /// <summary>
    /// 컴포넌트를 얻어오거나 없는 경우 추가
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="go"></param>
    /// <returns></returns>
    public static T GetOrAddComponent<T>(GameObject go) where T : Component
    {
        T component = go.GetComponent<T>();

        if (component == null)
        {
            component = go.AddComponent<T>();
        }

        return component;
    }

    /// <summary>
    /// 하이라키상 해당 오브젝트의 자식 오브젝트 반환
    /// </summary>
    /// <param name="go"></param>
    /// <param name="name"></param>
    /// <param name="recursive"></param>
    /// <returns></returns>
    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);

        if (transform == null)
        {
            return null;
        }

        return transform.gameObject;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : Object
    {
        if (go == null)
        {
            return null;
        }

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);

                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();

                    if (component != null)
                    {
                        return component;
                    }
                }
            }
        }
        else
        {
            foreach (T component in go.transform.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }

    /// <summary>
    /// float형이 같은 값인 지 비교
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool IsEqual(float a, float b)
    {
        if (a >= b - Mathf.Epsilon && a <= b + Mathf.Epsilon)
        {
            return true;
        }
        else
            return false;
    }

    public static bool IsValid(this GameObject go)
    {
        return go != null && go.activeSelf;
    }

    /// <summary>
    /// 플레이어 주변의 좌표를 무작위적으로 가져오는 메소드
    /// </summary>
    /// <param name="PlayerPos"></param>
    /// <param name="minDis"></param>
    /// <param name="maxDis"></param>
    /// <returns></returns>
    public static Vector3 GetPlayerNearCirclePos(Vector3 playerPos, float minDis, float maxDis)
    {
        float angle = Random.Range(-1,360) *Mathf.Deg2Rad;
        float distance = Random.Range(minDis,maxDis);

        float disX = Mathf.Cos(angle) * distance;
        float disZ = Mathf.Sin(angle) * distance;

        Vector3 spawnPos = playerPos + new Vector3(disX, 0f, disZ);

        return spawnPos;
    }

 
    /// <summary>
    /// 3D 환경에서의 카메라 바깥 좌표 구해주는 메소드
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="minDis"></param>
    /// <param name="maxDis"></param>
    /// <returns></returns>
    public static Vector3 GetCamOutPos3D(Camera camera ,float minDis, float maxDis)
    {
        Vector3 camPos = camera.transform.position;
        float camSize = camera.orthographicSize;
        float camSizeAspect = camSize * camera.aspect;
        Vector3 outPos = Vector3.zero;

        float width = 0f;
        float height = 0f;

        // 가로와 세로 중 카메라 범위 밖 좌표
        int randMax = Random.Range(0, 2);

        if (randMax == 0)  // 가로가 카메라 최소~최대 범위
        {
            // 카메라 밖 랜덤 좌표
            float widthMin = camSizeAspect + minDis;
            float widthMax = camSizeAspect + maxDis;
            width = Random.Range(widthMin, widthMax);

            // 카메라 안쪽 ~ 카메라 최대 오차 범위
            height = Random.Range(0f, camSize + maxDis);
        }
        else if (randMax == 1)  // 세로가 카메라 최소~최대 범위
        {
            width = Random.Range(0f, camSizeAspect + maxDis);

            float heightMin = camSize + minDis;
            float heightMax = camSize + maxDis;
            height = Random.Range(heightMin, heightMax);
        }

        // 양수 음수 랜덤
        width = Random.Range(0, 2) > 0 ? width : -width;
        height = Random.Range(0, 2) > 0 ? height : -height;

        // 카메라의 기준으로 변경
        width += camPos.x;
        height += camPos.z;

        // 가로와 세로 중 하나는 
        outPos = new Vector3(width, 0, height);

        return outPos;
    }

    public static Vector3 GetRotatedCamOutRandPos3D(Camera camera, float minDis = 0, float maxDis = 0)
    {
        float camNear = camera.nearClipPlane; // 카메라의 Near Clip 거리
        Vector3 outPos = Vector3.zero;

        // 카메라 꼭짓점 좌표 구하기
        Vector3 nearBottomLeft = camera.ViewportToWorldPoint(new Vector3(0, 0, camNear)); // 좌하e단 (Near)
        Vector3 nearBottomRight = camera.ViewportToWorldPoint(new Vector3(1, 0, camNear)); // 우하단 (Near)
        Vector3 nearTopLeft = camera.ViewportToWorldPoint(new Vector3(0, 1, camNear)); // 좌상단 (Near)
        Vector3 nearTopRight = camera.ViewportToWorldPoint(new Vector3(1, 1, camNear)); // 우상단 (Near)

        float dis = 0;

        if(minDis > 0 && maxDis > 0)
            dis = Random.Range(minDis, maxDis);

        // 상하좌우 선택 ( 0 : 상, 1 : 하 , 2 : 좌 , 3 : 우)
        int rand = Random.Range(0, 4);

        switch (rand)
        {
            case 0: // 상
                nearTopLeft = GetCamRayToGroundPos(camera, nearTopLeft);
                nearTopRight = GetCamRayToGroundPos(camera, nearTopRight);

                nearTopLeft = nearTopLeft * (1 + dis);
                nearTopRight = nearTopRight * (1 + dis);

                outPos = RandomRangeVector(nearTopLeft, nearTopRight);
                break;

            case 1: // 하
                nearBottomLeft = GetCamRayToGroundPos(camera, nearBottomLeft);
                nearBottomRight = GetCamRayToGroundPos(camera, nearBottomRight);

                nearBottomLeft = nearBottomLeft * (1 + dis);
                nearBottomRight = nearBottomRight * (1 + dis);

                outPos = RandomRangeVector(nearBottomLeft, nearBottomRight);
                break;

            case 2: // 좌
                nearTopLeft = GetCamRayToGroundPos(camera, nearTopLeft);
                nearBottomLeft = GetCamRayToGroundPos(camera, nearBottomLeft);

                nearTopLeft = nearTopLeft * (1 + dis);
                nearBottomLeft = nearBottomLeft * (1 + dis);

                outPos = RandomRangeVector(nearTopLeft, nearBottomLeft);
                break;

            case 3:  // 우
                nearTopRight = GetCamRayToGroundPos(camera, nearTopRight);
                nearBottomRight = GetCamRayToGroundPos(camera, nearBottomRight);

                nearTopRight = nearTopRight * (1 + dis);
                nearBottomRight = nearBottomRight * (1 + dis);

                outPos = RandomRangeVector(nearTopRight, nearBottomRight);
                break;
        }

        if(outPos.y < 0f ||  outPos.y >0f)
            outPos.y = 0f;

        return outPos;
    }

    public static Vector3 RandomRangeVector(Vector3 vectorA, Vector3 vectorB)
    {
        Vector3 randVector = Vector3.zero;
        float rand = Random.Range(0f, 1f);

        randVector = Vector3.Lerp(vectorA, vectorB, rand);

        return randVector;
    }

    public static Vector3 GetCamRayToGroundPos(Camera camera, Vector3 point)
    {
        Transform _cameraTrans = camera.transform;
        Ray ray = new Ray(point, _cameraTrans.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, 1 << LayerMask.NameToLayer("Ground"))) // 최대 100m 거리까지 탐색
        {
            return hit.point;
        }

        return point;
    }


    public static T ToEnum<T>(string str)
    {
        // 받은 Enum을 배열로 반환
        System.Array A = System.Enum.GetValues(typeof(T));
        foreach (T t in A)
        {
            if (t.ToString().Equals(str))
                return t;
        }
        return default(T);
    }

    public static void DebugLog(string message)
    {
        Debug.Log(message);
    }
}
