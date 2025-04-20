using UnityEngine;
using UnityEngine.UI;

public class UIElement_CheatButton : MonoBehaviour
{
    [SerializeField]
    private Text _txtCheat = null;
    private Button _btnCheat = null;
    private eCheat _cheatType = eCheat.Max;

    public void Init(eCheat cheatType)
    {
        _btnCheat = GetComponent<Button>();
        _btnCheat.onClick.AddListener(UseCheat);

        _cheatType = cheatType;
        _txtCheat.text = cheatType.ToString();
    }

    private void UseCheat()
    {
        switch(_cheatType)
        {
            case eCheat.MonsterAllKill:
                Managers.Instance.ObjectManager.AllKillMonsters();
                break;
            case eCheat.PlayerLevelUp:
                PlayerController player = GameManager.Instance.Player;
                int requiredExp = Managers.Instance.DataTableManager.DataTableInGameLevel.GetNextLevelRequiredExp(player.CurLevel);
                player.GetExp(requiredExp);
                break;
            case eCheat.Timeslip:
                GameManager.Instance.GameData.curTime += 60;
                break;
            case eCheat.RefreshSkill:
                UIPopup_SkillSelect popup = Managers.Instance.UIMananger.GetLastOpenPopup<UIPopup_SkillSelect>();
                popup.Refresh();
                break;
        }
    }
}
