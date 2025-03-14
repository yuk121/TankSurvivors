using Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    // Lobby
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
        Managers.Instance.ResourceManager.LoadAllAsyncWithLabel<Object>("preload", (key, count, totlaCount) =>
        {
            if (count == totlaCount)
            {
                Managers.Instance.DataTableManager.LoadData(() =>
                {
                    StartLoaded();
                });
            }
        });
    }

    private void StartLoaded()
    {
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
    }

    private void ModifyTitle()
    {
        if (_bTouchToStart == true)
        {
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
        _bGoLobby = false;
        _bStageStart = false;
    }

    private void ModifyLobby()
    {
        if(_bStageStart == true)
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
        _bStageStart=true;
    }
    #endregion

    #region Game
    private void InGame()
    {
        _gameData.Clear();

        int userCharId = 10001;
        // 플레이어 소환
        _player = Managers.Instance.ObjectManager.Spawn<PlayerController>(new Vector3(0f, 0.8f, 0f), userCharId);

        // 정보
        StageData stageInfo = Managers.Instance.DataTableManager.DataTableStage.GetStageInfo(_selectStage);
        GameData.stageInfo = stageInfo;

        WaveData waveInfo = Managers.Instance.DataTableManager.DataTableWave.GetWaveData(_selectStage);
        GameData.waveInfo = waveInfo;
        
        // 맵
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
    }

    private void ModifyGame()
    {
        // 플레이어 사망시 Result 
        if (CheckPlayerAlive() == false)
        {
            MoveState(eGameManagerState.Result);
            return;
        }

        // Pause가 된 경우
        if (_bPause == true)
        {
            MoveState(eGameManagerState.Pause);
            return;
        }

        // 보스를 잡은 경우
        if(_isBossSpawned == true && CheckEnemyBossAlive() == false)
        {
            MoveState(eGameManagerState.Result);
            return;
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
                
                // 아이템을 줄지 Gem을 줄 지 결정
                List<DropItemData> dropItemDataList = new List<DropItemData>();
                int randIndex = 0;
                float rand = Random.Range(0.0f, 1.0f);

                // 아이템을 주는 경우
                if (rand <= wave.dropItemRate)
                {
                    // 드랍 아이템 목록 읽어오기
                    for (int i = 0; i < wave.normalDropId.Count; i++)
                    {
                        dropItemData = Managers.Instance.DataTableManager.DataTableDropItem.GetDropItemData(wave.normalDropId[i]);
                        dropItemDataList.Add(dropItemData);
                    }
                    // 드랍 아이템 목록중 하나만 준다.
                    randIndex = Random.Range(0, dropItemDataList.Count);
                    return dropItemDataList[randIndex];
                }
                else
                {
                    // Gem 드랍
                    float redGemRate = wave.redGemDropRate;
                    float greenGemRate = redGemRate + wave.greenGemDropRate;
                    float blueGemRate = greenGemRate + wave.blueGemDropRate;
                    float purpleGemRate = blueGemRate + wave.purpleGemDropRate;

                    rand = Random.Range(0.0f, 1.0f);

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
                // 엘리트 몬스터는 드랍 아이템이 하나뿐
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
        if(CheckPlayerAlive() == false)
        {
            // 게임 오버 
            GameData.isGameEnd = true;
           
            // 패배 팝업창
            UIPopup_GameResult_Defeat popup =  Managers.Instance.UIMananger.OpenPopupWithTween<UIPopup_GameResult_Defeat>();
            popup.Set();
        }
        else if(IsBossSpawned == true && CheckEnemyBossAlive() == false)
        {
            // 스테이지 클리어
            GameData.isGameEnd = true;

            // 승리 팝업창
            UIPopup_GameResult_Victory popup = Managers.Instance.UIMananger.OpenPopupWithTween<UIPopup_GameResult_Victory>();
            popup.Set();
        }
    }

    private void ModifyResult()
    {
        if(_bGoLobby == true)   // 로비로 나가는 경우
        {
            MoveState(eGameManagerState.Lobby);
            
            // TODO : 씬 이동 Game -> Loading -> Lobby
            
            return;
        }
        else if(_bRetryStage == true)   // 재도전 하는 경우
        {
            MoveState(eGameManagerState.Game);

            // TODO : Game -> Loading -> Game
            return;
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
