using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Common;

public class GameData
{
    public StageData stageInfo;
    public WaveData waveInfo;
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

    private GameData _gameData = new GameData();
    public GameData GameData { get => _gameData; set => _gameData = value; }
    public CameraController CameraController { get; set; }

    private SpawningPools _spawnPools;

    private PlayerController _player;
    public PlayerController Player { get => _player; }

    private FogController _fogController;


    public bool IsPause { get; set; }

    

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

        string curSceneName = SceneManager.GetActiveScene().name; 
        
        if(curSceneName.Equals(eGameManagerState.Title.ToString()))
        {
            MoveState(eGameManagerState.Title);
        }
        else if(curSceneName.Equals(eGameManagerState.Lobby.ToString()))
        {
            MoveState(eGameManagerState.Lobby);
        }
        else if(curSceneName.Equals(eGameManagerState.Game.ToString()))
        {
            MoveState(eGameManagerState.Game);
        }
    }

    #region Title
    private void InTitle()
    {

    }

    private void ModifyTitle()
    {

    }
    #endregion

    #region Lobby
    private void InLobby()
    {

    }

    private void ModifyLobby()
    {

    }
    #endregion

    #region Game
    private void InGame()
    { 
        int userCharId = 10001;
        // 플레이어 소환
        _player = Managers.Instance.ObjectManager.Spawn<PlayerController>(new Vector3(0f, 0.8f, 0f), userCharId);

        // 임시
        StageData stageInfo = Managers.Instance.DataTableManager.DataTableStage.DataList[0];
        GameData.stageInfo = stageInfo;
        int stageIndex = GameData.stageInfo.stageIndex;

        WaveData waveInfo = Managers.Instance.DataTableManager.DataTableWave.GetWaveData(stageIndex);
        GameData.waveInfo = waveInfo;

        // Fog 
        FogController fog = Managers.Instance.ObjectManager.Spawn<FogController>(Vector3.zero);
        fog.SetFog(_player);

        _spawnPools = Utils.GetOrAddComponent<SpawningPools>(gameObject);
        _spawnPools.StartSpawn();
    }

    private void ModifyGame()
    {
        // 플레이어 사망시 Result 
        if(CheckPlayerAlive() == false)
        {
            MoveState(eGameManagerState.Result);
            return;
        }
    }

    public bool CheckPlayerAlive()
    {
        bool isAlive = _player.CheckAlive();

        return isAlive;
    }

    public DropItemData  GetRandomDropItem(Define.eMonsterGrade monsterGrade)
    {
        DropItemData dropItemData = new DropItemData();

        // TODO :
        // 1. waveInfo.dropItemRate에 따라서 아이템을 줄지 젬을 줄지 결정
        // 2. 젬을 주게 되면 waveInfo의 젬 드랍율에 맞춰서 무슨 젬을 줄지 결정
        // 3. 아이템을 주게 되면 노말 몬스터, 엘리트 몬스터 구분하여
        // 노말 몬스터면 waveInfo.normalDropId 참조해서 아이템을 주고
        // 엘리트 몬스터면 waveInfo.EliteDropId 참조해서 줄것

        return dropItemData;
    }
    #endregion

    #region Pause
    private void InPause()
    {

    }

    private void ModifyPause()
    {

    }

    private void OutPause()
    {

    }
    #endregion

    #region Result
    private void InResult()
    {

    }

    private void ModifyResult()
    {

    }
    #endregion
}
