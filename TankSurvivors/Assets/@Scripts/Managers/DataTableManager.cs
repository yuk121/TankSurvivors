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

    DataTableSupportSkill _supportSkillTable = new DataTableSupportSkill();
    public DataTableSupportSkill DataTableSupportSkill { get => _supportSkillTable; }

    DataTableStage _stageTable = new DataTableStage();
    public DataTableStage DataTableStage { get => _stageTable; }

    DataTableDropItem _dropItemTable = new DataTableDropItem();
    public DataTableDropItem DataTableDropItem { get => _dropItemTable; }

    DataTableInGameLevel _inGameLevelTable = new DataTableInGameLevel();
    public DataTableInGameLevel DataTableInGameLevel { get => _inGameLevelTable; }

    DataTableLocalization _localizationTable = new DataTableLocalization();
    public DataTableLocalization DataTableLocalization { get => _localizationTable; }

    DataTableUser _userTable = new DataTableUser();
    public DataTableUser UserTable { get => _userTable; }

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
        TextAsset supportSkillText = resoureceManager.Load<TextAsset>($"{lable}/DataTableSupportSkill.bytes");
        TextAsset stageText = resoureceManager.Load<TextAsset>($"{lable}/DataTableStage.bytes");
        TextAsset dropItemText = resoureceManager.Load<TextAsset>($"{lable}/DataTableDropItem.bytes");
        TextAsset inGameLevelText = resoureceManager.Load<TextAsset>($"{lable}/DataTableInGameLevel.bytes");
        TextAsset localizationText = resoureceManager.Load<TextAsset>($"{lable}/DataTableLocalization.bytes");
        TextAsset userTableText = resoureceManager.Load<TextAsset>($"{lable}/DataTableUser.bytes");

        _creatureTable.DataLoad(creatureText.bytes);
        _waveTable.DataLoad(waveText.bytes);
        _skillTable.DataLoad(skillText.bytes);
        _supportSkillTable.DataLoad(supportSkillText.bytes);
        _stageTable.DataLoad(stageText.bytes);
        _dropItemTable.DataLoad(dropItemText.bytes);    
        _inGameLevelTable.DataLoad(inGameLevelText.bytes);
        _localizationTable.DataLoad(localizationText.bytes);
        _userTable.DataLoad(userTableText.bytes);

        callback?.Invoke();
    }

}

