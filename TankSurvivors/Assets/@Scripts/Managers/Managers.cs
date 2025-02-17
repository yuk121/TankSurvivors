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
    private PoolManager _poolManager = new PoolManager();

    public PoolManager PoolManager { get { return Instance?._poolManager; } }

    private ObjectManager _objectManager = new ObjectManager();
    public ObjectManager ObjectManager { get { return Instance?._objectManager; } }

    private ResourceManager _resourceManager = new ResourceManager();
    public ResourceManager ResourceManager { get { return Instance?._resourceManager; } }

    private DataTableManager _dataTableManager = new DataTableManager();

    public DataTableManager DataTableManager { get { return Instance?._dataTableManager; } }

    private UIManager _uiManager = new UIManager();
    public UIManager UIMananger { get { return Instance?._uiManager; } }

    // Core
    private UserDataManager _userDataManager = new UserDataManager();
    public UserDataManager UserDataManager { get {  return Instance?._userDataManager; } }

    public static void Init()
    {
       
    }
}
