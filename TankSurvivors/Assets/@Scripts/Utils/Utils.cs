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

    public static Vector3 GenerateMonsterSpwanPosition(Vector3 PlayerPos, float minDis, float maxDis)
    {
        float angle = Random.Range(-1,360) *Mathf.Deg2Rad;
        float distance = Random.Range(minDis,maxDis);

        float disX = Mathf.Cos(angle) * distance;
        float disZ = Mathf.Sin(angle) * distance;

        Vector3 spawnPos = PlayerPos + new Vector3(disX, 0f, disZ);

        return spawnPos;
    }
}
