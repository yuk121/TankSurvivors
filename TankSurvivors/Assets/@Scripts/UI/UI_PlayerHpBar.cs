using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerHpBar : UI_Base
{
    enum eImage
    {
        Image_HpBar
    }

    private Image _imgHpBar = null;
    private PlayerController _player = null;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindImage(typeof(eImage));
        _imgHpBar = GetImage((int)eImage.Image_HpBar);
        _imgHpBar.fillAmount = 1f;

        return true;
    }

    public void SetPlayer(PlayerController player)
    {
        _player = player;
    }

    private void Update()
    {
        if (_player == null)
            return;

        if(_player.IsAlive == false)
        {
            _imgHpBar.fillAmount = 0f;
        }
        else
        {
            _imgHpBar.fillAmount = _player.CurHp / _player.CurMaxHp;
        }
    }

}
