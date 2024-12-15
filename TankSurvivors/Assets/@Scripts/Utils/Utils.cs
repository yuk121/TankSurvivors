using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Utils
{
    /// <summary>
    /// ������Ʈ�� �����ų� ���� ��� �߰�
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
    /// ���̶�Ű�� �ش� ������Ʈ�� �ڽ� ������Ʈ ��ȯ
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
    /// float���� ���� ���� �� ��
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
    /// �÷��̾� �ֺ��� ��ǥ�� ������������ �������� �޼ҵ�
    /// </summary>
    /// <param name="PlayerPos"></param>
    /// <param name="minDis"></param>
    /// <param name="maxDis"></param>
    /// <returns></returns>
    public static Vector3 GenerateMonsterSpwanPosition(Vector3 playerPos, float minDis, float maxDis)
    {
        float angle = Random.Range(-1,360) *Mathf.Deg2Rad;
        float distance = Random.Range(minDis,maxDis);

        float disX = Mathf.Cos(angle) * distance;
        float disZ = Mathf.Sin(angle) * distance;

        Vector3 spawnPos = playerPos + new Vector3(disX, 0f, disZ);

        return spawnPos;
    }

 
    /// <summary>
    /// 3D ȯ�濡���� ī�޶� �ٱ� ��ǥ �����ִ� �޼ҵ�
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

        // ���ο� ���� �� ī�޶� ���� �� ��ǥ
        int randMax = Random.Range(0, 2);

        if (randMax == 0)  // ���ΰ� ī�޶� �ּ�~�ִ� ����
        {
            // ī�޶� �� ���� ��ǥ
            float widthMin = camSizeAspect + minDis;
            float widthMax = camSizeAspect + maxDis;
            width = Random.Range(widthMin, widthMax);

            // ī�޶� ���� ~ ī�޶� �ִ� ���� ����
            height = Random.Range(0f, camSize + maxDis);
        }
        else if (randMax == 1)  // ���ΰ� ī�޶� �ּ�~�ִ� ����
        {
            width = Random.Range(0f, camSizeAspect + maxDis);

            float heightMin = camSize + minDis;
            float heightMax = camSize + maxDis;
            height = Random.Range(heightMin, heightMax);
        }

        // ��� ���� ����
        width = Random.Range(0, 2) > 0 ? width : -width;
        height = Random.Range(0, 2) > 0 ? height : -height;

        // ī�޶��� �������� ����
        width += camPos.x;
        height += camPos.z;

        // ���ο� ���� �� �ϳ��� 
        outPos = new Vector3(width, 0, height);

        return outPos;
    }

    public static T ToEnum<T>(string str)
    {
        // ���� Enum�� �迭�� ��ȯ
        System.Array A = System.Enum.GetValues(typeof(T));
        foreach (T t in A)
        {
            if (t.ToString().Equals(str))
                return t;
        }
        return default(T);
    }

}
