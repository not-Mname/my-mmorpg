using Common.Utils;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildMemberItem : ListView.ListViewItem
{
    public Text NameText;
    public Text LevelText;
    public Text ClassText;
    public Text StatusText;
    public Text JoinTimeText;
    public Text TitleText;

    public Image Background;
    public Sprite SelectedSprite;

    public NGuildMemberInfo Info;

    public void SetGuildMemberInfo(NGuildMemberInfo info)
    {
        Info = info;
        if(NameText!= null) NameText.text = info.Info.Name;
        if(LevelText!= null) LevelText.text = info.Info.Level.ToString();
        if(ClassText!= null) ClassText.text = info.Info.Class.ToString();
        if(StatusText!= null) StatusText.text = info.Status == 1? "在线" : "离线";
        if(JoinTimeText!= null) JoinTimeText.text = TimeUtil.GetTime(info.JoinTime).ToShortDateString();
        if(TitleText!= null) TitleText.text = info.Title.ToString();
    }

    public override void OnSelected(bool selected)
    {
        Background.overrideSprite = selected? SelectedSprite : null;
    }

}
