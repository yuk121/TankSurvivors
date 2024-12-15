using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    //public StageDataTable.StageInfo stageInfo;
    //public WaveDataTable.Data waveData;
}

public class GameManager 
{
    public GameData gameData = new GameData();
    public CameraController CameraController { get; set; }
}
