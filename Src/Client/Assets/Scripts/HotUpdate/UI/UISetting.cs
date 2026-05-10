using Managers;
using MMO;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISetting : UIWindow
{
    public void OnClickReturnCharacterSelect()
    {
        SceneManager.Instance.LoadScene("CharSelect");
        SoundManager.Instance.PlayMusic(SoundDefine.Music_Select);
        UserService.Instance.SendGameLeave();
    }
    public void OnClickExit()
    {
        UserService.Instance.SendGameLeave(true);
    }

    public void OnClickSystemSetting()
    {
        UIManager.Instance.Show<UISystemConfig>();
        this.Close();
    }

}
