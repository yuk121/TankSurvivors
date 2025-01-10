using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    public StageData stageInfo;
    public WaveData waveInfo;
}

public class GameManager 
{
    private GameData _gameData = new GameData();
    public GameData GameData { get => _gameData; set => _gameData = value; }
    public CameraController CameraController { get; set; }

    public bool IsPause { get; set; }
}
