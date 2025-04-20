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
        public List<string> m_InternalIds; // �ٿ�ε��� ���� ���� ����Ʈ
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
        Debug.Log("[APIManager] Firebase Auth ����");

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
                    pCallback?.Invoke(userData);  // ���� �� �ݹ� ����
                }
                else
                {
                    Debug.Log("No such user exists.");
                    pCallback?.Invoke(null);  // ���� �� �ݹ� ����
                }
            }
            else
            {
                Debug.LogError("Failed to load user data: " + task.Exception);
                pCallback?.Invoke(null);  // ���� �� �ݹ� ����
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
