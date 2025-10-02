using Managers;
using Models;
using Network;
using Services;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMain : MonoSingleton<UIMain>
{
    public Text avatarName;
    public Text avatarLevel;
    public UITeam TeamWindow;

    public void OnClickBag()
    {
        UIManager.Instance.Show<UIBag>();
    }

    public void OnClickEquip()
    {
        UIManager.Instance.Show<UIEquip>();
    }

    public void OnClickQuest()
    {
        UIManager.Instance.Show<UIQuest>();
    }

    public void OnClickFriend()
    {
        UIManager.Instance.Show<UIFriends>();
    }

    protected override void OnStart()
    {
        avatarName.text = User.Instance.CurrentCharacterInfo.Name;
        avatarLevel.text = User.Instance.CurrentCharacterInfo.Level.ToString();
    }

    public void ShowTeamUI(bool show)
    {
        TeamWindow.ShowTeam(show);
    }

    public void OnCliceGuild()
    {
        GuildManager.Instance.ShowGuild();
    }

    public void OnClickRide()
    {
        UIManager.Instance.Show<UIRide>();
    }

    public void OnClickSetting()
    {
        UIManager.Instance.Show<UISetting>();
    }

    public void OnClickSkill()
    {

    }
}
