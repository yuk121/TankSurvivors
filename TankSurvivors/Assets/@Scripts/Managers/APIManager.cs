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
        public List<string> m_InternalIds; // 다운로드할 번들 파일 리스트
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

    // 파일 유효성 확인
    public async Task CheckCatalog()
    {
        _gsReference = _storage.GetReferenceFromUrl(Define.FIREBASE_STORAGE_URL);

        // 로컬 카탈로그 확인 
        string localCatalog = GetLocalCatalog();
        // firebase 카탈로그 확인
        string firebastCatalog = await GetFirebaseCatalog();

        if (string.IsNullOrEmpty(firebastCatalog))
        {
            // 파일 다운로드
            UIPopup_Notification popup = Managers.Instance.UIMananger.OpenPopupWithTween<UIPopup_Notification>();

            // 다운로드 파일이 없다면 오류창 띄우고 종료
            popup.SetMessageOK("501 Error", () =>
            {
                Application.Quit();
            });

            return;
        }

        string localHash = localCatalog.GetHashCode().ToString();
        string firebaseHash = firebastCatalog.GetHashCode().ToString();

        // 카탈로그가 로컬에 없거나 서로 다른 경우 파일 다운로드 진행
        if (string.IsNullOrEmpty(localHash) || string.Equals(localHash, firebaseHash) == false)
        {
            UIPopup_Notification popup = Managers.Instance.UIMananger.OpenPopupWithTween<UIPopup_Notification>();

            popup.SetMessageOKCancel("추가 파일 다운로드가 존재합니다.", async () =>
            {
                await DownloadAllAddressableFiles(firebastCatalog);
            }, () =>
            {
                // 안받는 경우 종료
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
                    Debug.LogError($"APIManager: Firebase 'catalog.json' 다운로드 실패 - {task.Exception}");
                    return;
                }
            });

            return File.ReadAllText(path);
        }
        catch (Exception e)
        {
            Debug.LogError($"APIManager : Firebase 'catalog.json' 다운로드 실패 {e.Message}");
            return null;
        }
    }

    //catalog.json과 함께 필요한 AssetBundle 파일까지 다운로드
    private async Task DownloadAllAddressableFiles(string firebaseCatalog)
    {
        try
        {
            // catalog.json을 파싱하여 필요한 AssetBundle 목록 가져오기
            List<string> assetBundleFiles = ParseCatalogForBundles(firebaseCatalog);

            // AssetBundle 파일들 다운로드
            foreach (string bundleFile in assetBundleFiles)
            {
                await DownloadBundle(bundleFile);
            }

            Debug.Log("모든 Addressables 파일 다운로드 완료!");
        }
        catch (Exception e)
        {
            Debug.LogError($"APIManager: Addressables 다운로드 실패 - {e.Message}");
        }
    }

    //catalog.json을 파싱해서 필요한 AssetBundle 파일 목록 가져오기
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
            Debug.LogError($"APIManager: catalog.json 파싱 오류 - {e.Message}");
        }

        return bundleFiles;
    }

    //Firebase Storage에서 개별 AssetBundle 파일 다운로드
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
                    Debug.LogError($"APIManager: Firebase 다운로드 실패 - {task.Exception}");
                    return;
                }
            });

            Debug.Log($"{bundleFile} 다운로드 완료!");
        }
        catch (Exception e)
        {
            Debug.LogError($"APIManager: {bundleFile} 다운로드 실패 - {e.Message}");
        }
    }

    #endregion
}
