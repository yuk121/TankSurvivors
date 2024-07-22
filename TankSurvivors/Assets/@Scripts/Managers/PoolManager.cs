using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Pool
{
    private GameObject _prefab;
    private IObjectPool<GameObject> _pool;

    private Transform _root;

    public Transform Root
    {
        get
        {
            if(_root == null)
            {
                GameObject go = new GameObject();
                go.name = $"{_prefab.name} Pooling Root";

                _root = go.transform;
            }

            return _root;
        }
    }

    public Pool(GameObject prefab)
    {
        _prefab = prefab;
        _pool = new ObjectPool<GameObject>(OnCreate, OnGet, OnRelease, OnDestroy);
    }

    public GameObject Pop()
    {
        return _pool.Get();
    }

    public void Push(GameObject prefab)
    {
        _pool.Release(prefab);
    }

    private GameObject OnCreate()
    {
        GameObject go = GameObject.Instantiate(_prefab);

        go.transform.parent = Root;
        go.name = _prefab.name;

        return go;
    }

    private void OnGet(GameObject prefab)
    {
        prefab.SetActive(true);
    }

    private void OnRelease(GameObject prefab) 
    {
        prefab.SetActive(false);
    }

    private void OnDestroy(GameObject prefab)
    {
        GameObject.Destroy(prefab);
    }
}



public class PoolManager
{
    Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();

    public GameObject Pop(GameObject prefab)
    {
        if(_pools.ContainsKey(prefab.name) == false)
        {
            CreatePool(prefab);
        }

        return _pools[prefab.name].Pop();
    }

    public bool Push(GameObject prefab)
    {
        if (_pools.ContainsKey(prefab.name) == false)
            return false;

        _pools[prefab.name].Push(prefab);

        return true;
    }

    private void CreatePool(GameObject prefab)
    {
        Pool pool = new Pool(prefab);
        _pools.Add(prefab.name, pool);
    }
}
