using System.IO;
using System;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Firebase.Extensions;
using Firebase.Auth;
using Firebase.Firestore;
using Newtonsoft.Json;
using UnityEngine.SocialPlatforms;
using System.Security.Cryptography;
using Google;

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
    //FirebaseStorage _storage = null;
    #endregion

    public string webClientId = "<your client id here>";

    private GoogleSignInConfiguration configuration;
    private Action _callback = null;

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

    public void Init()
    {
        Init_Google();
    }

    #region Google Sign in

    private void Init_Google()
    {
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestIdToken = true
        };
   }

    public void OnGamesSignIn(Action pCallback)
    {
        _callback = pCallback;

        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = true;
        GoogleSignIn.Configuration.RequestIdToken = false;

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
          OnAuthenticationFinished);
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using (IEnumerator<System.Exception> enumerator =
                    task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error =
                            (GoogleSignIn.SignInException)enumerator.Current;
                }
                else
                {
                    Debug.Log("Got Unexpected Exception?!?" + task.Exception);
                }
            }
        }
        else if (task.IsCanceled)
        {
            Debug.Log("Canceled");
        }
        else
        {
            Debug.Log("Welcome: " + task.Result.DisplayName + "!");

            // Firebase Auth 시작
            FirebaseAuth(task.Result.IdToken);
        }
    }
    #endregion

    #region Firebase Auth

    public bool CheckFirebaseAuth()
    {
        FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        if(auth.CurrentUser != null)
        {
            Debug.Log($"[APIManager] Firebase Auth Current User UID : {auth.CurrentUser.UserId}");
            return true;
        }

        return false;
    }

    public void FirebaseAuth(string googleIdToken)
    {
        FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        Credential credential = GoogleAuthProvider.GetCredential(googleIdToken, null);

        auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAndRetrieveDataWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAndRetrieveDataWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);

            // LocalData
            Managers.Instance.OptionManager.LocalData._lastPlatform = Define.eLoginPlatform.Google;
            Managers.Instance.OptionManager.SaveLocalData();

            if (_callback != null)
                _callback.Invoke();
        });
    }
    #endregion

    #region Firebase Firestore
    #endregion

    #region Firebase Storage

    /*
    private void Init_FirebaseStorage()
    {
       _storage = FirebaseStorage.DefaultInstance;
    }

    // 파일 유효성 확인
    public async Task CheckCatalog(Action pCallback)
    {
        Debug.Log("카탈로그 비교 중");

        string localPath = Path.Combine(Application.persistentDataPath, "catalog.json");
        string tempPath = Path.Combine(Application.persistentDataPath, "catalog_temp.json");

        // firebase 카탈로그 다운로드
        Debug.Log("Firebase 카탈로그 다운로드 시작");
        //bool catalogDownloadSuccess = await GetFirebaseCatalog(tempPath);
  
        // 다운로드 파일이 없다면 오류창 띄우고 종료
        if (catalogDownloadSuccess == false)
        {
            // 파일 다운로드
            UIPopup_Notification popup = Managers.Instance.UIMananger.OpenPopupWithTween<UIPopup_Notification>();

            popup.SetMessageOK("501 Error", () =>
            {
                Application.Quit();
            });

            return;
        }

        Debug.Log("Hash 비교");
        string localHash = GetFileHash(localPath);
        Debug.Log($"Local Hash Bytes : {localHash}");

        string firebaseHash = GetFileHash(tempPath);
        Debug.Log($"Firebase Hash Bytes : {firebaseHash}");

        // 카탈로그가 로컬에 없거나 서로 다른 경우 파일 다운로드 진행
        if (string.IsNullOrEmpty(localHash) || localHash.Equals(firebaseHash) == false)
        {
            Debug.Log("APIManager: 새 catalog.json으로 교체");

            if (File.Exists(localPath) == true)
            {
                File.Delete(localPath);
            }
            File.Move(tempPath, localPath);

            UIPopup_Notification popup = Managers.Instance.UIMananger.OpenPopupWithTween<UIPopup_Notification>();

            popup.SetMessageOKCancel("추가 파일 다운로드가 존재합니다.", async () =>
            {
                 await DownloadAllAddressableFiles(localPath, pCallback);
            }, () =>
            {
                // 안받는 경우 종료
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
        // firebase에서 catalog.json 을 가져오고 파일 이름 변경 후 저장
        try
        {
            StorageReference _gsReference = _storage.GetReference("catalog.json");

            await _gsReference.GetFileAsync(savePath).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError($"APIManager: Firebase 'catalog.json' 다운로드 실패 - {task.Exception}");
                    return;
                }
            });

            return File.Exists(savePath); 
        }
        catch (Exception e)
        {
            Debug.LogError($"APIManager : Firebase 'catalog.json' 다운로드 실패 {e.Message}");
            return false;
        }
    }

    //catalog.json과 함께 필요한 AssetBundle 파일까지 다운로드
    private async Task DownloadAllAddressableFiles(string localPath, Action pCallback)
    {
        try
        {
            string catalog = GetLocalCatalog(localPath);
            // catalog.json을 파싱하여 필요한 AssetBundle 목록 가져오기
            List<string> assetBundleFiles = ParseCatalogForBundles(catalog);

            // AssetBundle 파일들 다운로드
            foreach (string bundleFile in assetBundleFiles)
            {
                await DownloadBundle(bundleFile);
            }

            Debug.Log("모든 Addressables 파일 다운로드 완료!");

            if (pCallback != null)
                pCallback.Invoke();
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
            string bundlePath = $"Android/{bundleFile}";
            StorageReference bundleRef = _storage.GetReference(bundlePath);

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
    
    // MD5 해시 값 계산
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
