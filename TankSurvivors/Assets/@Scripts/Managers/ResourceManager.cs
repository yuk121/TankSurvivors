using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

public class ResourceManager 
{
    Dictionary<string ,Object> _resources = new Dictionary<string, Object>();
    private bool _isDownloadStart = false;
    public bool IsDownloadStart { get => _isDownloadStart; set => _isDownloadStart = value; }
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

        // Ǯ�� Ȯ��
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

    // ��巹���� 
    public void LoadAsync<T>(string key, Action<T> callback = null) where T : Object
    {
        // ���� �ִ��� Ȯ��
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
                if (!_resources.ContainsKey(key)) // �ߺ� �߰� ����
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

    public void LoadWithCatalogCheckAndDownload<T>(string label, Action<string, int, int> callback) where T : Object
    {
        Debug.Log("[ResourceManager] īŻ�α� ������Ʈ Ȯ�� ��");

        var opCheckHandle = Addressables.CheckForCatalogUpdates();
        opCheckHandle.Completed += (opCheck) =>
        {
            if (opCheck.Result.Count > 0)
            {
                var opUpdateHandle = Addressables.UpdateCatalogs(opCheck.Result);
                opUpdateHandle.Completed += (opUpdate) =>
                {
                    Debug.Log("[ResourceManager] īŻ�α� ������Ʈ �Ϸ�");
                    ProceedToLoad<T>(label, callback);
                };
            }
            else
            {
                Debug.Log("[ResourceManager] īŻ�α� ������Ʈ ���ʿ�");
                ProceedToLoad<T>(label, callback);
            }
        };
    }

    private void ProceedToLoad<T>(string label, Action<string, int, int> callback) where T : Object
    {
        _isDownloadStart = false;

        var opLocationsHandle = Addressables.LoadResourceLocationsAsync(label, typeof(T));
        opLocationsHandle.Completed += (opLoc) =>
        {
            var opDownloadHandle = Addressables.GetDownloadSizeAsync(label);
            opDownloadHandle.Completed += (opSize) =>
            {
                long byteSize = opSize.Result;
                float mbSize = byteSize / (1024f * 1024f);

                Debug.Log($"[ResourceManager] �ٿ�ε� Ȯ�� �Ϸ� - �뷮: {mbSize:F2} MB");

                if (byteSize > 0)
                {
                    // �ٿ�ε� �˾� ǥ��
                    UIPopup_FileDownload popup = Managers.Instance.UIMananger.OpenPopupWithResources<UIPopup_FileDownload>();
                    popup.Set(mbSize, () =>
                    {
                        var downloadDependenciesHandle = Addressables.DownloadDependenciesAsync(label);
                        downloadDependenciesHandle.Completed += (downloadOp) =>
                        {
                            if (downloadOp.Status == AsyncOperationStatus.Succeeded)
                            {
                                _isDownloadStart = true;
                                Debug.Log("[ResourceManager] �ٿ�ε� �Ϸ�");
                                
                                LoadAllAsyncWithLabel<T>(label, callback);
                            }
                            else
                            {
                                Debug.LogError("[ResourceManager] �ٿ�ε� ����");
                            }
                        };
                    });
                }
                else
                {
                    LoadAllAsyncWithLabel<T>(label, callback);
                }
            };
        };
    }

    public void LoadAllAsyncWithLabel<T>(string lable, Action<string, int, int> callback) where T : Object
    {
        var opHandle = Addressables.LoadResourceLocationsAsync(lable, typeof(T));

        Debug.Log($"[ResourceManager] ���ν� �ε� ��");

        opHandle.Completed += (op) =>
        {
            int loadCount = 0;
            int totalCount = op.Result.Count;

            foreach (var result in op.Result)
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
