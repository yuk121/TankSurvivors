using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class UIPopup_SkillGet : UI_Base
{
    #region Enum_UI
    private enum eGameObject
    {
        UIElement_RandomSkill
    }

    private enum eButton
    {
        Button_TouchToClose
    }

    private enum eImage
    {
        Image_BackLight
    }

    #endregion

    private GameObject _randomSkillPrefab = null;
    private Button _btnTouchToClose = null;
    private Image _imgBackLight = null;
    private Transform _trans = null;

    private float _backLightSpeed = 1f;
    private int _getSkillCountMax = 1;   // 필요 시 조절 가능하도록 할 것
    public override bool Init()
    {
        if(_init == false)
        {
            base.Init();
        }

        _trans = transform;

        // Bind
        BindOjbect(typeof(eGameObject));
        BindButton(typeof(eButton));
        BindImage(typeof(eImage));

        // Get
        _randomSkillPrefab = GetObject((int)eGameObject.UIElement_RandomSkill);
        _btnTouchToClose = GetButton((int)eButton.Button_TouchToClose);
        _imgBackLight = GetImage((int)eImage.Image_BackLight);

        //
        _randomSkillPrefab.SetActive(false);
        _btnTouchToClose.onClick.AddListener(OnClick_TouchToClose);
        return true;
    }

    public void Set()
    {
        if (_init == false)
            Init();

        // 백라이트 돌리기
        _imgBackLight.transform.DOKill();
        _imgBackLight.transform.DORotate(new Vector3(0, 0, 360), 1 / _backLightSpeed, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);

        // 획득 스킬 이미지 보여주기
        List<SkillBase> _randomSkillList = GetRandomSkill();

        for (int i = 0; i < _randomSkillList.Count; i++)
        {
            UIElement_RandomSkill skill = Managers.Instance.UIMananger.InstantiateUI<UIElement_RandomSkill>(_trans);
            skill.SetRandomSkill(_randomSkillList[i]);
            skill.RemoveButtonEventAll();
        }

        // 획득 스킬 업그레이드
        UpgradeSkill(_randomSkillList);
    }

    private List<SkillBase> GetRandomSkill()
    {
        List<SkillBase> skillList = new List<SkillBase>();
        List<SkillBase> skillRandomList = new List<SkillBase>();

        // 플레이어가 사용가능한 action skill 목록 가져오기
        List<ActionSkill> actionSkillList = GameManager.Instance.Player.GetActionSkillList();
        // 플레이어가 배울수 있는 support skill 목록 가져오기
        List<SupportSkill> supportSkillList = GameManager.Instance.Player.GetSupportSkillList();

        for (int i = 0; i < actionSkillList.Count; i++)
        {
            if (actionSkillList[i].CurSkillLevel < Define.MAX_SKILL_LEVEL)
            {
                skillList.Add(actionSkillList[i]);
            }
        }

        for (int i = 0; i < supportSkillList.Count; i++)
        {
            if (supportSkillList[i].CurSkillLevel < Define.MAX_SKILL_LEVEL)
            {
                skillList.Add(supportSkillList[i]);
            }
        }

        int randomSkillCount = Mathf.Min(skillList.Count, _getSkillCountMax);

        if (randomSkillCount == 0)
        {
            // 더이상 스킬을 선택할 수 없는 경우 
            // 게임 재화나 체력회복
        }
        else
        {
            while (skillRandomList.Count < randomSkillCount)
            {
                int rand = Random.Range(0, skillList.Count);

                skillRandomList.Add(skillList[rand]);
                skillList.RemoveAt(rand);
            }
        }

        return skillRandomList;
    }

    private void UpgradeSkill(List<SkillBase> skillList)
    {
        PlayerController player = GameManager.Instance.Player;

        for (int i = 0; i < skillList.Count; i++)
        {
            ActionSkill actionSkill = skillList[i] as ActionSkill;
            SupportSkill supportSkill = skillList[i] as SupportSkill;

            if (actionSkill != null)
            {
                player.SkillUpgrade(actionSkill.SkillData.skillId);
            }
            else if(supportSkill != null)
            {
                player.SkillUpgrade(supportSkill.SupportSkillData.skillId);

                if (supportSkill != null && supportSkill.SupportSkillType == Define.eSupportSkillType.MaxHpUp)
                {
                    // 다음 레벨과 현재 레벨의 증가량 만큼만 회복
                    float incRate = player.GetSupportSkillValueFloatInc(supportSkill.SupportSkillData.skillId);
                    player.OnRecovery(incRate);
                }
            }
            else
            {
                Debug.Log($"{this} : Skill is Null");
            }
        }
    }

    private void OnClick_TouchToClose()
    {
        Managers.Instance.UIMananger.ClosePopup();
    }
}
