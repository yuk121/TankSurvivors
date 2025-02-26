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

    public void Clear()
    {
        stageInfo = null;
        waveInfo = null;

        curTime = 0f;
        killCount = 0;
        firstLevelUp = false;
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

    private GameData _gameData = new GameData();
    public GameData GameData { get => _gameData; set => _gameData = value; }

    // In Game
    public CameraController CameraController { get; set; }

    private SpawningPools _spawnPools;

    private PlayerController _player;
    public PlayerController Player { get => _player; }
   
    [SerializeField]
    private bool _bPause;
    public bool Pause { get => _bPause; }

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
        _gameData.Clear();

        int userCharId = 10001;
        // 플레이어 소환
        _player = Managers.Instance.ObjectManager.Spawn<PlayerController>(new Vector3(0f, 0.8f, 0f), userCharId);

        // 임시
        StageData stageInfo = Managers.Instance.DataTableManager.DataTableStage.DataList[0];
        GameData.stageInfo = stageInfo;
        int stageIndex = GameData.stageInfo.stageIndex;

        WaveData waveInfo = Managers.Instance.DataTableManager.DataTableWave.GetWaveData(stageIndex);
        GameData.waveInfo = waveInfo;

        // GridManager
        GridManager.Instance.Init();

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

        // Pause가 된 경우
        if(_bPause == true)
        {
            MoveState(eGameManagerState.Pause);
            return;
        }

        _gameData.curTime += Time.deltaTime;
    }

    public bool CheckPlayerAlive()
    {
        bool isAlive = _player.CheckAlive();

        return isAlive;
    }

    public DropItemData GetRandomDropItem(Define.eMonsterGrade monsterGrade)
    {
        DropItemData dropItemData = new DropItemData();
        WaveData wave = GameData.waveInfo;

        float rand = Random.Range(0.0f, 1.0f);

        // 아이템을 줄지 Gem을 줄 지 결정
        if (rand <= wave.dropItemRate)
        {
            // 아이템을 주는 경우
            switch (monsterGrade)
            {
                case Define.eMonsterGrade.Normal:
                    // 드랍 아이템 목록 읽어오기
                    List<DropItemData> dropItemDataList = new List<DropItemData>();
                    for (int i = 0; i < wave.normalDropId.Count; i++)
                    {
                        dropItemData = Managers.Instance.DataTableManager.DataTableDropItem.GetDropItemData(wave.normalDropId[i]);
                        dropItemDataList.Add(dropItemData);
                    }
                    // 드랍 아이템 목록중 하나만 준다.
                    int randIndex = Random.Range(0, dropItemDataList.Count);

                    return dropItemDataList[randIndex];

                case Define.eMonsterGrade.Elite:
                    // 엘리트 몬스터는 드랍 아이템이 하나뿐
                    dropItemData = Managers.Instance.DataTableManager.DataTableDropItem.GetDropItemData(wave.eliteDropId);
                    return dropItemData;
            }
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

    }

    private void ModifyResult()
    {

    }
    #endregion
}
