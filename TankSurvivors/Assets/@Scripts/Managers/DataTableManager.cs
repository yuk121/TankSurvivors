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

        _creatureTable.DataLoad(creatureText.bytes);
        _waveTable.DataLoad(waveText.bytes);

        callback?.Invoke();
    }

}

