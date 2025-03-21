using System.Collections;
using System.Collections.Generic;
using System.Resources;
using Unity.VisualScripting;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers _instance;

    private static bool _isInit = false;

    public static Managers Instance
    {
        get
        {
            if (_isInit == false)
            {
                _isInit = true;

                GameObject go = GameObject.Find("Managers");

                if (go == null)
                {
                    go = new GameObject();
                    go.name = "Managers";             

                    go.AddComponent<Managers>();
                }

                DontDestroyOnLoad(go);
                _instance = go.GetComponent<Managers>();

                Init();
            }
     
            return _instance;
        }
    }

    // Contetnt 
  
    // Pool
    private PoolManager _poolManager = new PoolManager();
    public PoolManager PoolManager { get { return Instance?._poolManager; } }
  
    // Object
    private ObjectManager _objectManager = new ObjectManager();
    public ObjectManager ObjectManager { get { return Instance?._objectManager; } }
    
    // Resource
    private ResourceManager _resourceManager = new ResourceManager();
    public ResourceManager ResourceManager { get { return Instance?._resourceManager; } }
    
    // DataTable
    private DataTableManager _dataTableManager = new DataTableManager();
    public DataTableManager DataTableManager { get { return Instance?._dataTableManager; } }

    // UI
    private UIManager _uiManager = new UIManager();
    public UIManager UIMananger { get { return Instance?._uiManager; } }

    //Scene
    private SceneManager _sceneManager;
    public SceneManager SceneManager { get { return Instance?._sceneManager; } }

    private OptionManager _optionManager = new OptionManager();
    public OptionManager OptionManager { get { return Instance?._optionManager; } }

    // Core

    // UserData
    private UserDataManager _userDataManager = new UserDataManager();
    public UserDataManager UserDataManager { get {  return Instance?._userDataManager; } }

    public static void Init()
    {
        _instance._sceneManager = Instance.gameObject.AddComponent<SceneManager>();
    }

    public void Update()
    {
#if UNITY_EDITOR

        if (GameManager.Instance != null && GameManager.Instance.GetSceneState() != eGameManagerState.Game)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // 사운드
                SoundManager.Instance.Play("SFX_Touch", Define.eSoundType.SFX);
            }
        }
#elif UNITY_ANDROID
        if (GameManager.Instance != null && GameManager.Instance.GetSceneState() != eGameManagerState.Game)
        {
            if (Input.touchCount == 1)
            {
                // 사운드
                SoundManager.Instance.Play("SFX_Touch", Define.eSoundType.SFX);
            }
        }
#endif

        // 타이틀과 로비에서만 적용 , 팝업창이 없는 경우에만 뒤로가기 게임 종료 알림
        if ((GameManager.Instance != null && _uiManager.GetPopupCount() == 0 &&
            GameManager.Instance.GetSceneState() == eGameManagerState.Lobby) ||
             GameManager.Instance.GetSceneState() == eGameManagerState.Title) 
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UIPopup_Notification popup = _uiManager.OpenPopupWithTween<UIPopup_Notification>();
                popup.SetMessage("게임을 종료하시겠습니까?", () =>
                {
                    GameQuit();
                }, null);
            }
        }
    }

    private void GameQuit()
    {
        _userDataManager.LogOutUser();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
            Application.Quit(); 
#endif
    }

    private void OnApplicationQuit()
    {
        if (_userDataManager != null)
            _userDataManager.SaveUserData();
    }

    private void OnApplicationPause(bool pause)
    {
        if(pause == true)
        {
            if (_userDataManager != null)
                _userDataManager.SaveUserData();
        }
    }

    public void Clear()
    {
        ObjectManager.Clear();
        PoolManager.Clear();
    }
}
