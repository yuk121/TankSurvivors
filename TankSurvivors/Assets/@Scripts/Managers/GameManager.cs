using System;
using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets.Initialization;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameData
{
    public StageData stageInfo;
    public WaveData waveInfo;
    public float curTime = 0f;
    public int killCount = 0;
    public bool firstLevelUp = false;
    public bool isGameEnd = false;

    public void Clear()
    {
        stageInfo = null;
        waveInfo = null;

        curTime = 0f;
        killCount = 0;
        firstLevelUp = false;
        isGameEnd = false;
    }
}

public enum eGameManagerState
{
    None,
    Title,
    Lobby,
    Game,
    Pause,
    Result,
}

public class GameManager : FSM<eGameManagerState>
{
    #region SimpleSingleTon
    public static GameManager Instance;
    #endregion

    // Title
    private bool _bTouchToStart = false;
    private bool _bWait = false;
    private bool _isStartProcessEnd = false;
    public bool IsStartProcessEnd { get => _isStartProcessEnd; }
    private string _processState = string.Empty;
    public string ProcessState { get=> _processState; }
    // Lobby
    public event Action Update_LobbyUI;
    private bool _bStageStart = false;
    private int _selectStage = 1;

    // Game

    private GameData _gameData = new GameData();
    public GameData GameData { get => _gameData; set => _gameData = value; }

    public CameraController CameraController { get; set; }

    private SpawningPools _spawnPools;

    private PlayerController _player;
    public PlayerController Player { get => _player; }

    private bool _isBossSpawned = false;
    public bool IsBossSpawned { get => _isBossSpawned; set => _isBossSpawned = value; }

    private bool _bPause;
    public bool Pause { get => _bPause; }

    private bool _bGoLobby;
    private bool _bRetryStage;

    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        _isStartProcessEnd = false;

        StartCoroutine(CorStartProcess());
    }

    private IEnumerator CorStartProcess()
    {
        _bWait = true;
        _processState = "�α��� ���� ���� ��";
        // ���� ������ �ҷ�����
        LocalData localdata = Managers.Instance.OptionManager.LoadLocalData();

        // ���õ����Ͱ� ���ٸ� ���� ����
        if(localdata == null)
        {
            Managers.Instance.OptionManager.NewLocalData();
            Managers.Instance.OptionManager.SaveLocalData();
        }

#if !UNITY_EDITOR
        // �÷��� ����
        switch (localdata._lastPlatform)
        {
            case Define.eLoginPlatform.Google:
                {
                    bool userLogined = APIManager.Instance.CheckFirebaseAuth();

                    // ���� �α��� Ȯ�� �� �ȵ� ���¶�� ��α��� ����
                    if (userLogined == false)
                    {
                        UIPopup_Login popup = Resources.Load<UIPopup_Login>("Login/UIPopup_Login");
                        popup.Set(() =>
                        {
                            _bWait = false;
                        });
                    }
                    else
                    {            
                        _bWait = false;
                    }
                }
                break;
             
            case Define.eLoginPlatform.None:
                {
                    // �÷����� ���ٸ� ��⿡ �°� ���������ϱ� (����� �ȵ���̵常)
                    UIPopup_Login popup = Resources.Load<UIPopup_Login>("Login/UIPopup_Login");
                    popup.Set(() =>
                    {
                        _bWait = false;
                    });
                }
                break;
        }

        // �α��� ���� ���
        while (_bWait)
            yield return null;
#endif

        // ��巹���� ���ҽ� �ҷ�����
        _bWait = true;
        _processState = "���ҽ� �ҷ����� ��";

        Managers.Instance.ResourceManager.LoadAllAsyncWithLabel<UnityEngine.Object>("preload", (key, count, totlaCount) =>
        {
            if (count == totlaCount)
            {
                _bWait = false;
            }
        });

        while (_bWait)
            yield return null;

        _processState = "���� ���� Ȯ�� ��";
        // ���� ������ ���� �ҷ����� Ȯ���ϱ�
        UserData user = Managers.Instance.UserDataManager.LoadUserData();

        // uid�� ���°�� �ű� ����
        if (user == null || user._uid == null)
        {
            Managers.Instance.UserDataManager.NewStartUser();
            Managers.Instance.UserDataManager.SaveUserData();
        }

        yield return new WaitForSeconds(0.5f);

        // ���� ������ �ҷ�����
        SoundManager.Instance.ApplyAllVolumes();

        _bWait = true;
        // ������ ���̺� �ҷ�����
        Managers.Instance.DataTableManager.LoadData(() =>
        {
            _bWait = false;
        });

        while (_bWait)
            yield return null;

        _processState = string.Empty;

        StartProcessEnd();
    }

    private void StartProcessEnd()
    {
        _isStartProcessEnd = true;

        AddState(eGameManagerState.Title, InTitle, ModifyTitle, null);
        AddState(eGameManagerState.Lobby, InLobby, ModifyLobby, null);
        AddState(eGameManagerState.Game, InGame, ModifyGame, null);
        AddState(eGameManagerState.Pause, InPause, ModifyPause, OutPause);
        AddState(eGameManagerState.Result, InResult, ModifyResult, null);

        string curSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if (curSceneName.Equals(eGameManagerState.Title.ToString()))
        {
            MoveState(eGameManagerState.Title);
        }
        else if (curSceneName.Equals(eGameManagerState.Lobby.ToString()))
        {
            MoveState(eGameManagerState.Lobby);
        }
        else if (curSceneName.Equals(eGameManagerState.Game.ToString()))
        {
            MoveState(eGameManagerState.Game);
        }
    }

    public eGameManagerState GetSceneState()
    {
        return curState;
    }

#region Title
    private void InTitle()
    {
        _bTouchToStart = false;

        SoundManager.Instance.Play("BGM_Title", Define.eSoundType.BGM, -10);
    }

    private void ModifyTitle()
    {
        if (_bTouchToStart == true)
        {
            Managers.Instance.UIMananger.CloseAllPopup();

            _bTouchToStart = false;

            Managers.Instance.SceneManager.LoadScene(eGameManagerState.Lobby.ToString(), () =>
            {
                MoveState(eGameManagerState.Lobby);
                return;
            });
        }
    }

    public void StartGame()
    {
        _bTouchToStart = true;
    }
#endregion

#region Lobby
    private void InLobby()
    {
        Managers.Instance.Clear();

        _bGoLobby = false;
        _bStageStart = false;

        // ����
        SoundManager.Instance.Play("BGM_Lobby", Define.eSoundType.BGM, -10);

    }

    private void ModifyLobby()
    {
        if (_bStageStart == true)
        {
            _bStageStart = false;

            Managers.Instance.SceneManager.LoadScene(eGameManagerState.Game.ToString(), () =>
            {
                MoveState(eGameManagerState.Game);
                return;
            });
        }
    }

    public void StageStart(int selectStage)
    {
        _selectStage = selectStage;
        _bStageStart = true;
    }

    public void UpdateLobby()
    {
        if (Update_LobbyUI != null)
            Update_LobbyUI.Invoke();
    }
#endregion

#region Game
    private void InGame()
    {
        Managers.Instance.Clear();

        _gameData.Clear();

        int userCharId = Managers.Instance.UserDataManager.UserData._userChaterId;
        // �÷��̾� ��ȯ
        _player = Managers.Instance.ObjectManager.Spawn<PlayerController>(new Vector3(0f, 0.8f, 0f), userCharId);

        // ����
        StageData stageInfo = Managers.Instance.DataTableManager.DataTableStage.GetStageInfo(_selectStage);
        GameData.stageInfo = stageInfo;

        WaveData waveInfo = Managers.Instance.DataTableManager.DataTableWave.GetWaveData(_selectStage);
        GameData.waveInfo = waveInfo;

        // ��
        string mapPrefab = $"MapPrefab/{stageInfo.stagePrefab}.prefab";
        GameObject map = Managers.Instance.ResourceManager.Instantiate(mapPrefab);
        map.transform.parent = GameObject.FindGameObjectWithTag("Map").transform;

        // GridManager
        GridManager.Instance.Init();

        // Init
        _isBossSpawned = false;
        _bRetryStage = false;

        _spawnPools = Utils.GetOrAddComponent<SpawningPools>(gameObject);
        _spawnPools.StartSpawn();

        SoundManager.Instance.Play("BGM_Game", Define.eSoundType.BGM, -20);
    }

    private void ModifyGame()
    {
        // �÷��̾� ����� Result 
        if (CheckPlayerAlive() == false)
        {
            _spawnPools.StopSpawn();
            MoveState(eGameManagerState.Result);
            return;
        }

        // Pause�� �� ���
        if (_bPause == true)
        {
            MoveState(eGameManagerState.Pause);
            return;
        }

        // ������ ���� ���
        if (_isBossSpawned == true && CheckEnemyBossAlive() == false)
        {
            _spawnPools.StopSpawn();
            MoveState(eGameManagerState.Result);
            return;
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            _gameData.curTime += 60;
        }

#endif
        // �ڷΰ��� ��ư ������ pause â ����
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            UIPopup_Pause popup = Managers.Instance.UIMananger.OpenPopupWithTween<UIPopup_Pause>(true);
        }

        _gameData.curTime += Time.deltaTime;
    } 

    public bool CheckPlayerAlive()
    {
        bool isAlive = _player.CheckAlive();

        return isAlive;
    }

    private bool CheckEnemyBossAlive()
    {
        bool isAlive = Managers.Instance.ObjectManager.GetBoss().IsAlive;

        return isAlive;
    }
   

    public DropItemData GetRandomDropItem(Define.eMonsterGrade monsterGrade)
    {
        DropItemData dropItemData = new DropItemData();
        WaveData wave = GameData.waveInfo;
   
        switch (monsterGrade)
        {
            case Define.eMonsterGrade.Normal:
                
                // �������� ���� Gem�� �� �� ����
                List<DropItemData> dropItemDataList = new List<DropItemData>();
                int randIndex = 0;
                float rand = UnityEngine.Random.Range(0.0f, 1.0f);

                // �������� �ִ� ���
                if (rand <= wave.dropItemRate)
                {
                    // ��� ������ ��� �о����
                    for (int i = 0; i < wave.normalDropId.Count; i++)
                    {
                        dropItemData = Managers.Instance.DataTableManager.DataTableDropItem.GetDropItemData(wave.normalDropId[i]);
                        dropItemDataList.Add(dropItemData);
                    }
                    // ��� ������ ����� �ϳ��� �ش�.
                    randIndex = UnityEngine.Random.Range(0, dropItemDataList.Count);
                    return dropItemDataList[randIndex];
                }
                else
                {
                    // Gem ���
                    float redGemRate = wave.redGemDropRate;
                    float greenGemRate = redGemRate + wave.greenGemDropRate;
                    float blueGemRate = greenGemRate + wave.blueGemDropRate;
                    float purpleGemRate = blueGemRate + wave.purpleGemDropRate;

                    rand = UnityEngine.Random.Range(0.0f, 1.0f);

                    if (rand <= redGemRate)
                    {
                        dropItemData = Managers.Instance.DataTableManager.DataTableDropItem.GetDropItemData((int)Define.eGemType.RedGem);
                    }
                    else if (rand <= greenGemRate)
                    {
                        dropItemData = Managers.Instance.DataTableManager.DataTableDropItem.GetDropItemData((int)Define.eGemType.GreenGem);
                    }
                    else if (rand <= blueGemRate)
                    {
                        dropItemData = Managers.Instance.DataTableManager.DataTableDropItem.GetDropItemData((int)Define.eGemType.BlueGem);
                    }
                    else if (rand <= purpleGemRate)
                    {
                        dropItemData = Managers.Instance.DataTableManager.DataTableDropItem.GetDropItemData((int)Define.eGemType.PurpleGem);
                    }

                    return dropItemData;
                }

            case Define.eMonsterGrade.Elite:
                // ����Ʈ ���ʹ� ��� �������� �ϳ���
                dropItemData = Managers.Instance.DataTableManager.DataTableDropItem.GetDropItemData(wave.eliteDropId);
                return dropItemData;
        }
        return null;
    }
#endregion

#region Pause
    private void InPause()
    {
        if (Managers.Instance.ObjectManager.Monsters.Count > 0)
        {
            foreach(MonsterController mon in Managers.Instance.ObjectManager.Monsters)
            {
                mon.BranchInPause();
            }
        }
    }

    private void ModifyPause()
    {
        if(_bPause == false)
        {
            UndoState();
        }

        if(_bGoLobby == true)
        {
            _spawnPools.StopSpawn();

            Managers.Instance.SceneManager.LoadScene(eGameManagerState.Lobby.ToString(), () =>
            {
                MoveState(eGameManagerState.Lobby);
                return;
            });
        }
    }

    private void OutPause()
    {
    }         
    public void SetPause(bool bPause)
    {
        _bPause = bPause;
    }
#endregion

#region Result
    private void InResult()
    {
        SoundManager.Instance.StopAllSound();

        if (CheckPlayerAlive() == false)
        {
            // ���� ���� 
            GameData.isGameEnd = true;

            // ����
            SoundManager.Instance.Play("SFX_Defeat", Define.eSoundType.SFX);

            // �й� �˾�â
            UIPopup_GameResult_Defeat popup = Managers.Instance.UIMananger.OpenPopupWithTween<UIPopup_GameResult_Defeat>();
            popup.Set();
        }
        else if (IsBossSpawned == true && CheckEnemyBossAlive() == false)
        {
            // �������� Ŭ����
            GameData.isGameEnd = true;

            // Ŭ���� ���� ���� ó��
            StageClearProcess(() =>
           {
               // ����
               SoundManager.Instance.Play("BGM_Victory", Define.eSoundType.BGM);

               // �¸� �˾�â
               UIPopup_GameResult_Victory popup = Managers.Instance.UIMananger.OpenPopupWithTween<UIPopup_GameResult_Victory>();
               popup.Set();
           });
        }
    }

    private void StageClearProcess(Action pCallback)
    {
        StageData stageData = _gameData.stageInfo;
        // ���� ������ ó��
        Managers.Instance.UserDataManager.UseStamina(stageData);

        // ���� ��ȭ ȹ��
        Managers.Instance.UserDataManager.GetReward(stageData);

        // �������� Ŭ���� ó��
        Managers.Instance.UserDataManager.StageClear(stageData);

        // ����
        Managers.Instance.UserDataManager.SaveUserData();
        
        if (pCallback != null)
            pCallback.Invoke();
    }

    private void ModifyResult()
    {
        if (_bGoLobby == true)   // �κ�� ������ ���
        {
            _bGoLobby = false;

            Managers.Instance.SceneManager.LoadScene(eGameManagerState.Lobby.ToString(), () =>
            {
                MoveState(eGameManagerState.Lobby);
                return;
            });
        }
        else if (_bRetryStage == true)   // �絵�� �ϴ� ���
        {
            _bRetryStage = false;

            Managers.Instance.SceneManager.LoadScene(eGameManagerState.Game.ToString(), () =>
            {
                MoveState(eGameManagerState.Game);
                return;
            });
        }
    }

    public void GoLobby()
    {
        _bGoLobby = true;
    }

    public void RetryStage()
    {
        _bRetryStage = true;
    }
#endregion
}
