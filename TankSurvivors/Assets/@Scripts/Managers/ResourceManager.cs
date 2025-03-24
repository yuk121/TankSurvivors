using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

public class ResourceManager 
{
    Dictionary<string ,Object> _resources = new Dictionary<string, Object>();

    public T Load<T>(string key) where T : Object
    {
        if(_resources.TryGetValue(key, out Object resource))
        {
            if(/*key.Contains(".sprite") && */resource is Texture2D texture)
            {
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                return sprite as T;
            }

            return resource as T;
        }

        return null;
    }

    public GameObject Instantiate(string key, Transform parent = null, bool pooling = false)
    {
        GameObject prefab = Load<GameObject>($"{key}");

        if (prefab == null)
        {
            Debug.Log($"Failed to load prefab {key}");
            return null;
        }

        // 풀링 확인
        if(pooling == true)
        {
            return Managers.Instance.PoolManager.Pop(prefab);
        }

        GameObject go = Object.Instantiate(prefab, parent);
        go.name = prefab.name;

        return go;
    }

    public void Destroy(GameObject go)
    {
        if(go == null)
        {
            return;
        }

        if(Managers.Instance.PoolManager.Push(go)) 
        {
            return;
        }

        Object.Destroy(go);
    }

    // 어드레서블 
    public void LoadAsync<T>(string key, Action<T> callback = null) where T : Object
    {
        // 현재 있는지 확인
        if(_resources.TryGetValue(key, out Object resource)) 
        {
            callback?.Invoke(resource as T);
            return;
        }

        string loadKey = key;

        if(key.Contains(".sprite"))
        {
            loadKey = $"{key}[{key.Replace("sprite", " ")}]";
        }

        var asyncOperation = Addressables.LoadAssetAsync<T>(loadKey);
        asyncOperation.Completed += (op) =>
        {
            if (op.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                if (!_resources.ContainsKey(key)) // 중복 추가 방지
                {
                    _resources.Add(key, op.Result);
                }
                callback?.Invoke(op.Result);
            }
            else
            {
                Debug.LogError($"Failed to load asset: {key}");
            }
        };
    }

    public void LoadAllAsyncWithLabel<T>(string lable, Action<string, int, int> callback) where T : Object
    {
        var opHandle = Addressables.LoadResourceLocationsAsync(lable, typeof(T));

        opHandle.Completed += (op) =>
        {
            int loadCount = 0;
            int totalCount = op.Result.Count;

            foreach(var result in op.Result) 
            {
                LoadAsync<T>(result.PrimaryKey, (obj) =>
                {
                    loadCount++;
                    callback?.Invoke(result.PrimaryKey, loadCount, totalCount);
                });
            }
        };

    }


}
