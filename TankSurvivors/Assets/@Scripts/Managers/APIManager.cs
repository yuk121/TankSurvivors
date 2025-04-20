using System.IO;
using System;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;
using Firebase.Auth;
using Firebase.Firestore;
using Newtonsoft.Json;
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

    [SerializeField]
    private string webClientId = "<your client id here>";
    private GoogleSignInConfiguration configuration;

    private Action _callbackSuccess = null;
    private Action _callbackFail = null;

    private FirebaseAuth _auth = null;
    private FirebaseFirestore _db = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        DontDestroyOnLoad(this);
    }

    public void Init()
    {

    }

#region Google Sign in

    public void Init_Google()
    {
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestIdToken = true
        };
    }

    public void OnSignIn(Action pCallbackSuccess = null, Action pCallbackFail = null)
    {
        _callbackSuccess = pCallbackSuccess;
        _callbackFail = pCallbackFail;  

        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        Debug.Log("Calling SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished, TaskScheduler.FromCurrentSynchronizationContext());
    }

    public void OnSignOut()
    {
        Debug.Log("Calling SignOut");
        GoogleSignIn.DefaultInstance.SignOut();
    }

    public void OnDisconnect()
    {
        Debug.Log("Calling Disconnect");
      GoogleSignIn.DefaultInstance.Disconnect();
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using (IEnumerator<System.Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                    Debug.Log("Got Error: " + error.Status + " " + error.Message);
                }
                else
                {
                    Debug.Log("Got Unexpected Exception?!?" + task.Exception);
                }
            }

            if (_callbackFail != null)
                _callbackFail.Invoke();
        }
        else if (task.IsCanceled)
        {
            Debug.Log("Canceled");

            if (_callbackFail != null)
                _callbackFail.Invoke();
        }
        else
        {
            Debug.Log("Welcome: " + task.Result.DisplayName + "!");
            Debug.Log($"ID Token : {task.Result.IdToken}");

            // Firebase Auth 
            FirebaseAuth(task.Result.IdToken);
        }
    }

    public void OnSignInSilently()
    {
      GoogleSignIn.Configuration = configuration;
      GoogleSignIn.Configuration.UseGameSignIn = false;
      GoogleSignIn.Configuration.RequestIdToken = true;
        Debug.Log("Calling SignIn Silently");

      GoogleSignIn.DefaultInstance.SignInSilently().ContinueWith(OnAuthenticationFinished,TaskScheduler.FromCurrentSynchronizationContext());
    }


    public void OnGamesSignIn()
    {
      GoogleSignIn.Configuration = configuration;
      GoogleSignIn.Configuration.UseGameSignIn = true;
      GoogleSignIn.Configuration.RequestIdToken = false;

        Debug.Log("Calling Games SignIn");

      GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished,TaskScheduler.FromCurrentSynchronizationContext());
    }


#endregion

#region Firebase Auth
    public bool CheckFirebaseAuth()
    {
        _auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        if(_auth.CurrentUser != null)
        {
            Debug.Log($"[APIManager] Firebase Auth Current User UID : {_auth.CurrentUser.UserId}");
            return true;
        }

        return false;
    }

    public string GetUserUid()
    {
        return _auth.CurrentUser.UserId;
    }

    private void FirebaseAuth(string googleIdToken)
    {
        Debug.Log("[APIManager] Firebase Auth 시작");

        _auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        Credential credential = GoogleAuthProvider.GetCredential(googleIdToken, null);

        _auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAndRetrieveDataWithCredentialAsync was canceled.");
                
                //
                if (_callbackFail != null)
                    _callbackFail.Invoke();

                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAndRetrieveDataWithCredentialAsync encountered an error: " + task.Exception);

                //
                if (_callbackFail != null)
                    _callbackFail.Invoke();

                return;
            }

            Firebase.Auth.FirebaseUser result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.DisplayName, result.UserId);

            // LocalData
            Managers.Instance.OptionManager.LocalData._lastPlatform = Define.eLoginPlatform.Google;
            Managers.Instance.OptionManager.SaveLocalData();

            if (_callbackSuccess != null)
                _callbackSuccess.Invoke();
        });
    }
#endregion

#region Firebase Firestore
    public void SetUserData(UserData userData, Action pCallback)
    {
        Debug.Log("[APIManager] Firebase Firestore Set UserData");

        _db = FirebaseFirestore.DefaultInstance;
        DocumentReference docRef = _db.Collection("users").Document(userData.uid);
    
        docRef.SetAsync(userData).ContinueWithOnMainThread(task => 
        {
            if (task.IsCompleted)
                Debug.Log("User data saved successfully!");
            else
                Debug.LogError($"Failed to save: {task.Exception}");

            if (pCallback != null)
                pCallback.Invoke();
        });
    }

    public void GetUserData(string uid, Action<UserData> pCallback)
    {
        UserData userData = null;

        _db = FirebaseFirestore.DefaultInstance;
        _db.Collection("users").Document(uid).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DocumentSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    userData = snapshot.ConvertTo<UserData>();
                    pCallback?.Invoke(userData);  // 성공 시 콜백 실행
                }
                else
                {
                    Debug.Log("No such user exists.");
                    pCallback?.Invoke(null);  // 성공 시 콜백 실행
                }
            }
            else
            {
                Debug.LogError("Failed to load user data: " + task.Exception);
                pCallback?.Invoke(null);  // 성공 시 콜백 실행
            }
        });
    }

    public void SetTransaction_StageClear(UserData userData)
    {
        _db = FirebaseFirestore.DefaultInstance;
        DocumentReference docRef = _db.Collection("users").Document(userData.uid);

        _db.RunTransactionAsync(async transaction =>
        {
            DocumentSnapshot snapshot = await transaction.GetSnapshotAsync(docRef);

            // update
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "userStaminaCurrent", userData.userStaminaCurrent },
                { "lastStaminaChangeTimestamp", userData.lastStaminaChangeTimestamp },
                { "userCurrency.gold", userData.userCurrency.gold },
                { "userExp", userData.userExp },
                { "stageClearList", userData.stageClearList }
            };

            transaction.Update(docRef, updates);

        }).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
                Debug.Log("Gold updated successfully!");
            else
                Debug.LogError("Transaction failed: " + task.Exception);
        });
    }

    public void SetTransaction_StageSelect(UserData userData)
    {
        _db = FirebaseFirestore.DefaultInstance;
        DocumentReference docRef = _db.Collection("users").Document(userData.uid);

        _db.RunTransactionAsync(async transaction =>
        {
            DocumentSnapshot snapshot = await transaction.GetSnapshotAsync(docRef);
            // update
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "lastSelectStageLevel", userData.lastSelectStageLevel},
            };

            transaction.Update(docRef, updates);

        }).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
                Debug.Log("Gold updated successfully!");
            else
                Debug.LogError("Transaction failed: " + task.Exception);
        });
    }

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
