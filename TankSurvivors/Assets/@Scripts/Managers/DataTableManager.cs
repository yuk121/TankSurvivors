using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataTableManager
{

    public void LoadData(Action callback = null)
    {
        LoadFromLocal(callback);
    }

    DataTableCreature _creatureTable = new DataTableCreature();
    public DataTableCreature DataTableCreature { get => _creatureTable; }

    DataTableWave _waveTable = new DataTableWave();
    public DataTableWave DataTableWave { get => _waveTable; }

    DataTableSkill _skillTable = new DataTableSkill();
    public DataTableSkill DataTableSkill { get => _skillTable; }

    DataTableStage _stageTable = new DataTableStage();
    public DataTableStage DataTableStage { get => _stageTable; }

    private void LoadFromLocal(Action callback)
    {
        ResourceManager resoureceManager = Managers.Instance.ResourceManager;

        if(resoureceManager == null)
        {
            Debug.Log("ResourceManager is null");
            return;
        }

        string lable = "Bytes";

        TextAsset creatureText = resoureceManager.Load<TextAsset>($"{lable}/DataTableCreature.bytes");
        TextAsset waveText = resoureceManager.Load<TextAsset>($"{lable}/DataTableWave.bytes");
        TextAsset skillText = resoureceManager.Load<TextAsset>($"{lable}/DataTableSkill.bytes");
        TextAsset stageText = resoureceManager.Load<TextAsset>($"{lable}/DataTableStage.bytes");

        _creatureTable.DataLoad(creatureText.bytes);
        _waveTable.DataLoad(waveText.bytes);
        _skillTable.DataLoad(skillText.bytes);
        _stageTable.DataLoad(stageText.bytes);

        callback?.Invoke();
    }

}

