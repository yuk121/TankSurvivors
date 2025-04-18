using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private float _downloadProgressValue = 0f;
    public float DownloadProgressValue { get => _downloadProgressValue; }

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
        _processState = "로그인 인증 진행 중";
        // 로컬 데이터 불러오기
        LocalData localdata = Managers.Instance.OptionManager.LoadLocalData();

        // 로컬데이터가 없다면 새로 생성
        if (localdata == null)
        {
            Managers.Instance.OptionManager.NewLocalData();
            Managers.Instance.OptionManager.SaveLocalData();

            localdata = Managers.Instance.OptionManager.LocalData;
        }

        // API Init()     
        APIManager.Instance.Init();

#if !UNITY_EDITOR
        // 플랫폼 인증
        switch (localdata._lastPlatform)
        {
            case Define.eLoginPlatform.Google:
                {
                    bool userLogined = APIManager.Instance.CheckFirebaseAuth();

                    // 유저 로그인 확인 후 안된 상태라면 재로그인 실행
                    if (userLogined == false)
                    {
                        UIPopup_Login popup = Managers.Instance.UIMananger.OpenPopupWithResources<UIPopup_Login>();
                        popup.Set(() =>
                        {
                            Managers.Instance.UIMananger.ClosePopup();
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
                    Debug.Log("마지막 접속 플랫폼 존재하지않음");
                    // 플랫폼이 없다면 기기에 맞게 인증시작하기 (현재는 안드로이드만)
                    UIPopup_Login popup = Managers.Instance.UIMananger.OpenPopupWithResources<UIPopup_Login>();

                    if (popup == null)
                    {
                        Debug.Log("UIPopup_Login is Null");
                    }

                    popup.Set(() =>
                    {
                        Managers.Instance.UIMananger.ClosePopup();
                        _bWait = false;
                    });
                }
                break;
        }

        // 로그인 인증 대기
        while (_bWait)
            yield return null;
#endif

        // 어드레서블 리소스 불러오기
        _bWait = true;
        _processState = "리소스 확인 중";

#if !UNITY_EDITOR
        Managers.Instance.ResourceManager.LoadWithCatalogCheckAndDownload<UnityEngine.Object>("preload", (key, count, totlaCount) =>
        {
            _downloadProgressValue = (float)count / totlaCount;

            if (count == totlaCount)
            {
                Debug.Log("리소스 불러오기 완료!");
                Managers.Instance.ResourceManager.IsDownloadStart = false;
                _bWait = false;
            }
        });
#else

        Managers.Instance.ResourceManager.LoadAllAsyncWithLabel<UnityEngine.Object>("preload", (key, count, totlaCount) =>
        {
            _downloadProgressValue = (float)count / totlaCount;

            if (count == totlaCount)
            {
                Debug.Log("리소스 불러오기 완료!");
                _bWait = false;
            }
        });
#endif
        while (_bWait)
            yield return null;

        // 유저 데이터 정보 불러오고 확인하기
        _bWait = true;

        _processState = "유저 정보 확인 중";

        Managers.Instance.UserDataManager.LoadUserData((userdata) =>
        {
            Managers.Instance.UserDataManager.UserData = userdata;
            _bWait = false;
        });

        while (_bWait)
            yield return null;

        UserData userData = Managers.Instance.UserDataManager.UserData;
        // uid가 없는경우 신규 유저
        if (userData == null || userData.uid == null)
        {
            _bWait = true;

            Debug.Log("Server UserData is null so Create New User ServerData");

            Managers.Instance.UserDataManager.NewStartUser();
            
            // 유저 데이터 db에 저장
            Managers.Instance.UserDataManager.SaveUserData(() =>
            {
                Debug.Log("Firestore에 저장 완료");
                _bWait = false;
            });

            while (_bWait)
                yield return null;

            _bWait = true;

            Debug.Log("새로 생성된 유저데이터 다시 불러오기");
           
            Managers.Instance.UserDataManager.LoadUserData((userdata) =>
            {
                Managers.Instance.UserDataManager.UserData = userdata;
                _bWait = false;
            });

            while (_bWait)
                yield return null;
        }

        Debug.Log($"[GameManger] 유저 정보 불러오기 성공");

        // 사운드 설정값 불러오기
        SoundManager.Instance.ApplyAllVolumes();

        _bWait = true;
        // 데이터 테이블 불러오기
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

        // 사운드
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

        int userCharId = Managers.Instance.UserDataManager.UserData.userChaterId;
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

        SoundManager.Instance.Play("BGM_Game", Define.eSoundType.BGM, -20);
    }

    private void ModifyGame()
    {
        // 플레이어 사망시 Result 
        if (CheckPlayerAlive() == false)
        {
            _spawnPools.StopSpawn();
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
        // 뒤로가기 버튼 누를시 pause 창 띄우기
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
                
                // 아이템을 줄지 Gem을 줄 지 결정
                List<DropItemData> dropItemDataList = new List<DropItemData>();
                int randIndex = 0;
                float rand = UnityEngine.Random.Range(0.0f, 1.0f);

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
                    randIndex = UnityEngine.Random.Range(0, dropItemDataList.Count);
                    return dropItemDataList[randIndex];
                }
                else
                {
                    // Gem 드랍
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
            // 게임 오버 
            GameData.isGameEnd = true;

            // 사운드
            SoundManager.Instance.Play("SFX_Defeat", Define.eSoundType.SFX);

            // 패배 팝업창
            UIPopup_GameResult_Defeat popup = Managers.Instance.UIMananger.OpenPopupWithTween<UIPopup_GameResult_Defeat>();
            popup.Set();
        }
        else if (IsBossSpawned == true && CheckEnemyBossAlive() == false)
        {
            // 스테이지 클리어
            GameData.isGameEnd = true;

            // 클리어 이후 보상 처리
            StageClearProcess(() =>
           {
               // 사운드
               SoundManager.Instance.Play("BGM_Victory", Define.eSoundType.BGM);

               // 승리 팝업창
               UIPopup_GameResult_Victory popup = Managers.Instance.UIMananger.OpenPopupWithTween<UIPopup_GameResult_Victory>();
               popup.Set();
           });
        }
    }

    private void StageClearProcess(Action pCallback)
    {
        StageData stageData = _gameData.stageInfo;

        // 스테이지 클리어 요청 과정
        Managers.Instance.UserDataManager.Request_StageClear(stageData);
        
        if (pCallback != null)
            pCallback.Invoke();
    }

    private void ModifyResult()
    {
        if (_bGoLobby == true)   // 로비로 나가는 경우
        {
            _bGoLobby = false;

            Managers.Instance.SceneManager.LoadScene(eGameManagerState.Lobby.ToString(), () =>
            {
                MoveState(eGameManagerState.Lobby);
                return;
            });
        }
        else if (_bRetryStage == true)   // 재도전 하는 경우
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
