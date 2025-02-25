using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class UIList_OwnedSkills : UI_Base
{
    #region Enum_UI
    enum eGameObject
    {

    }

    // 순서 건들면 꼬일수 있으므로 주의할 것
    enum eImage
    {
        Image_ActiveSkill1,
        Image_ActiveSkill2,
        Image_ActiveSkill3,
        Image_ActiveSkill4,
        Image_ActiveSkill5,

        Image_SupportSkill1,
        Image_SupportSkill2,
        Image_SupportSkill3,
        Image_SupportSkill4,
        Image_SupportSkill5,
    }
    #endregion

    public List<Image> _imgActiveSkillList = new List<Image>();
    public List<Image> _imgSupportSkillList = new List<Image>();

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        //Bind
        BindImage(typeof(eImage));

        //Get 
        int totalCount = Define.MAX_ACTIVE_SKILL_COUNT + Define.MAX_SUPPORT_SKILL_COUNT;

        for (int i = 0; i < totalCount; i++)
        {
            eImage eSkill = (eImage)i;

            if (i < Define.MAX_ACTIVE_SKILL_COUNT)
            {
                _imgActiveSkillList.Add(GetImage((int)eSkill));
            }
            else
            {
                _imgSupportSkillList.Add(GetImage((int)eSkill));
            }
        }

        InitSkillImage();

        return true;
    }

    public void InitSkillImage()
    {
        for (int i = 0; i < _imgActiveSkillList.Count; i++)
        {
            _imgActiveSkillList[i].sprite = null;
            _imgActiveSkillList[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < _imgSupportSkillList.Count; i++)
        {
            _imgSupportSkillList[i].sprite = null;
            _imgSupportSkillList[i].gameObject.SetActive(false);
        }
    }

    public void SetOwnedSkills()
    {
        if (Init() == false)
            Init();

        GetCurrentPlayerSkill();
    }

    public void GetCurrentPlayerSkill()
    {
        if (GameManager.Instance == null || GameManager.Instance.Player == null)
        {
            Debug.LogError($"{this} : GameManager Null or Player Null !!!");
        }

        // 플레이어 스킬, 보조스킬을 가져온다
        PlayerController player = GameManager.Instance.Player;
        List<ActionSkill> actionSkillList = player.GetActionSkillList().Where(skill => skill.CurSkillLevel > 0).ToList();
        List<SupportSkill> supportSkillList = player.GetSupportSkillList().Where(skill => skill.CurSkillLevel > 0).ToList(); ;

        // 스킬 목록에 맞는 이미지 넣기
        for (int i = 0; i < actionSkillList.Count; i++)
        {
            string skillImage = actionSkillList[i].SkillData.skillImage;
            Sprite sprite = Managers.Instance.ResourceManager.Load<Sprite>(skillImage);

            _imgActiveSkillList[i].sprite = sprite;
            _imgActiveSkillList[i].gameObject.SetActive(true);
        }

        for (int i = 0; i < supportSkillList.Count; i++)
        {
            string skillImage = supportSkillList[i].SupportSkillData.skillImage;
            Sprite sprite = Managers.Instance.ResourceManager.Load<Sprite>(skillImage);

            _imgSupportSkillList[i].sprite = sprite;
            _imgSupportSkillList[i].gameObject.SetActive(true);
        }
    }
}

