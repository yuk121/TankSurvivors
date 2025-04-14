using System.IO;
using System;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
//using Firebase.Extensions;
//using Firebase.Storage;
using Newtonsoft.Json;
using UnityEngine.SocialPlatforms;
using System.Security.Cryptography;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.Initialization;
using UnityEngine.ResourceManagement.AsyncOperations;

public class APIManager : MonoBehaviour
{
    [Serializable]
    public class AddressablesCatalog
    {
        public List<string> m_InternalIds; // �ٿ�ε��� ���� ���� ����Ʈ
    }

    #region Simple SingleTon
    public static APIManager Instance;
    #endregion

    #region Firebase Stroage
    //FirebaseStorage _storage = null;
    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        //Init_FirebaseStorage();
        //Init_Addressable();#endif
#endif
    }

    #region Firebase Storage

    /*
    private void Init_FirebaseStorage()
    {
       _storage = FirebaseStorage.DefaultInstance;
    }

    // ���� ��ȿ�� Ȯ��
    public async Task CheckCatalog(Action pCallback)
    {
        Debug.Log("īŻ�α� �� ��");

        string localPath = Path.Combine(Application.persistentDataPath, "catalog.json");
        string tempPath = Path.Combine(Application.persistentDataPath, "catalog_temp.json");

        // firebase īŻ�α� �ٿ�ε�
        Debug.Log("Firebase īŻ�α� �ٿ�ε� ����");
        //bool catalogDownloadSuccess = await GetFirebaseCatalog(tempPath);
  
        // �ٿ�ε� ������ ���ٸ� ����â ���� ����
        if (catalogDownloadSuccess == false)
        {
            // ���� �ٿ�ε�
            UIPopup_Notification popup = Managers.Instance.UIMananger.OpenPopupWithTween<UIPopup_Notification>();

            popup.SetMessageOK("501 Error", () =>
            {
                Application.Quit();
            });

            return;
        }

        Debug.Log("Hash ��");
        string localHash = GetFileHash(localPath);
        Debug.Log($"Local Hash Bytes : {localHash}");

        string firebaseHash = GetFileHash(tempPath);
        Debug.Log($"Firebase Hash Bytes : {firebaseHash}");

        // īŻ�αװ� ���ÿ� ���ų� ���� �ٸ� ��� ���� �ٿ�ε� ����
        if (string.IsNullOrEmpty(localHash) || localHash.Equals(firebaseHash) == false)
        {
            Debug.Log("APIManager: �� catalog.json���� ��ü");

            if (File.Exists(localPath) == true)
            {
                File.Delete(localPath);
            }
            File.Move(tempPath, localPath);

            UIPopup_Notification popup = Managers.Instance.UIMananger.OpenPopupWithTween<UIPopup_Notification>();

            popup.SetMessageOKCancel("�߰� ���� �ٿ�ε尡 �����մϴ�.", async () =>
            {
                 await DownloadAllAddressableFiles(localPath, pCallback);
            }, () =>
            {
                // �ȹ޴� ��� ����
                Application.Quit();
            });
            return;
        }
        else
        {
            File.Delete(tempPath);

            if (pCallback != null)
                pCallback.Invoke();
        }
    }

    private string GetLocalCatalog(string path)
    {
        if (File.Exists(path))
        {
            return File.ReadAllText(path);
        }

        return null;
    }

    
    async Task<bool> GetFirebaseCatalog(string savePath)
    {
        // firebase���� catalog.json �� �������� ���� �̸� ���� �� ����
        try
        {
            StorageReference _gsReference = _storage.GetReference("catalog.json");

            await _gsReference.GetFileAsync(savePath).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError($"APIManager: Firebase 'catalog.json' �ٿ�ε� ���� - {task.Exception}");
                    return;
                }
            });

            return File.Exists(savePath); 
        }
        catch (Exception e)
        {
            Debug.LogError($"APIManager : Firebase 'catalog.json' �ٿ�ε� ���� {e.Message}");
            return false;
        }
    }

    //catalog.json�� �Բ� �ʿ��� AssetBundle ���ϱ��� �ٿ�ε�
    private async Task DownloadAllAddressableFiles(string localPath, Action pCallback)
    {
        try
        {
            string catalog = GetLocalCatalog(localPath);
            // catalog.json�� �Ľ��Ͽ� �ʿ��� AssetBundle ��� ��������
            List<string> assetBundleFiles = ParseCatalogForBundles(catalog);

            // AssetBundle ���ϵ� �ٿ�ε�
            foreach (string bundleFile in assetBundleFiles)
            {
                await DownloadBundle(bundleFile);
            }

            Debug.Log("��� Addressables ���� �ٿ�ε� �Ϸ�!");

            if (pCallback != null)
                pCallback.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError($"APIManager: Addressables �ٿ�ε� ���� - {e.Message}");
        }
    }

    //catalog.json�� �Ľ��ؼ� �ʿ��� AssetBundle ���� ��� ��������
    private List<string> ParseCatalogForBundles(string catalogPath)
    {
        List<string> bundleFiles = new List<string>();
        try
        {
            string jsonContent = File.ReadAllText(catalogPath);
            AddressablesCatalog catalogData = JsonConvert.DeserializeObject<AddressablesCatalog>(jsonContent);

            foreach (var bundle in catalogData.m_InternalIds)
            {
                if (bundle.EndsWith(".bundle"))
                {
                    bundleFiles.Add(bundle);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"APIManager: catalog.json �Ľ� ���� - {e.Message}");
        }

        return bundleFiles;
    }

    
    //Firebase Storage���� ���� AssetBundle ���� �ٿ�ε�
    private async Task DownloadBundle(string bundleFile)
    {
        try
        {
            string bundlePath = $"Android/{bundleFile}";
            StorageReference bundleRef = _storage.GetReference(bundlePath);

            await bundleRef.GetFileAsync(bundlePath).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError($"APIManager: Firebase �ٿ�ε� ���� - {task.Exception}");
                    return;
                }
            });

            Debug.Log($"{bundleFile} �ٿ�ε� �Ϸ�!");
        }
        catch (Exception e)
        {
            Debug.LogError($"APIManager: {bundleFile} �ٿ�ε� ���� - {e.Message}");
        }
    }
    
    // MD5 �ؽ� �� ���
    private string GetFileHash(string filePath)
    {
        Debug.Log($"Hash : {filePath}");

        if (!File.Exists(filePath))
            return null;
       
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hashBytes = md5.ComputeHash(stream);            
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }

    */
    #endregion
}
