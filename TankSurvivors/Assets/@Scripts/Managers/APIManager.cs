using System.IO;
using System;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Firebase.Extensions;
using Firebase.Storage;
using Newtonsoft.Json;

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
    FirebaseStorage _storage = null;
    StorageReference _gsReference = null;
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
        Init_FirebaseStorage();
    }

    #region Firebase Storage

    private void Init_FirebaseStorage()
    {
        _storage = FirebaseStorage.DefaultInstance;
    }

    // ���� ��ȿ�� Ȯ��
    public async Task CheckCatalog()
    {
        _gsReference = _storage.GetReferenceFromUrl(Define.FIREBASE_STORAGE_URL);

        // ���� īŻ�α� Ȯ�� 
        string localCatalog = GetLocalCatalog();
        // firebase īŻ�α� Ȯ��
        string firebastCatalog = await GetFirebaseCatalog();

        if (string.IsNullOrEmpty(firebastCatalog))
        {
            // ���� �ٿ�ε�
            UIPopup_Notification popup = Managers.Instance.UIMananger.OpenPopupWithTween<UIPopup_Notification>();

            // �ٿ�ε� ������ ���ٸ� ����â ���� ����
            popup.SetMessageOK("501 Error", () =>
            {
                Application.Quit();
            });

            return;
        }

        string localHash = localCatalog.GetHashCode().ToString();
        string firebaseHash = firebastCatalog.GetHashCode().ToString();

        // īŻ�αװ� ���ÿ� ���ų� ���� �ٸ� ��� ���� �ٿ�ε� ����
        if (string.IsNullOrEmpty(localHash) || string.Equals(localHash, firebaseHash) == false)
        {
            UIPopup_Notification popup = Managers.Instance.UIMananger.OpenPopupWithTween<UIPopup_Notification>();

            popup.SetMessageOKCancel("�߰� ���� �ٿ�ε尡 �����մϴ�.", async () =>
            {
                await DownloadAllAddressableFiles(firebastCatalog);
            }, () =>
            {
                // �ȹ޴� ��� ����
                Application.Quit();
            });
            return;
        }
    }

    private string GetLocalCatalog()
    {
        string path = Path.Combine(Application.persistentDataPath, "catalog.json");

        if (File.Exists(path))
        {
            return File.ReadAllText(path);
        }

        return null;
    }

    async Task<string> GetFirebaseCatalog()
    {
        try
        {
            string path = Path.Combine(Application.persistentDataPath, "catalog.json");
            await _gsReference.GetFileAsync(path).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError($"APIManager: Firebase 'catalog.json' �ٿ�ε� ���� - {task.Exception}");
                    return;
                }
            });

            return File.ReadAllText(path);
        }
        catch (Exception e)
        {
            Debug.LogError($"APIManager : Firebase 'catalog.json' �ٿ�ε� ���� {e.Message}");
            return null;
        }
    }

    //catalog.json�� �Բ� �ʿ��� AssetBundle ���ϱ��� �ٿ�ε�
    private async Task DownloadAllAddressableFiles(string firebaseCatalog)
    {
        try
        {
            // catalog.json�� �Ľ��Ͽ� �ʿ��� AssetBundle ��� ��������
            List<string> assetBundleFiles = ParseCatalogForBundles(firebaseCatalog);

            // AssetBundle ���ϵ� �ٿ�ε�
            foreach (string bundleFile in assetBundleFiles)
            {
                await DownloadBundle(bundleFile);
            }

            Debug.Log("��� Addressables ���� �ٿ�ε� �Ϸ�!");
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
            string bundlePath = Path.Combine(Application.persistentDataPath, bundleFile);
            StorageReference bundleRef = _storage.GetReferenceFromUrl($"{Define.FIREBASE_STORAGE_URL}/{bundleFile}");

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

    #endregion
}
