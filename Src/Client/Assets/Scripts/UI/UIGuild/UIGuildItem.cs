using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildItem : ListView.ListViewItem
{

    public Text IDText;
    public Text NameText;
    public Text MemberNumberText;
    public Text LeaderText;
    public NGuildInfo Info;

    public Image Background;
    public Sprite SelectedSprite;

    public void SetInfo(NGuildInfo guild)
    {
        Info = guild;
        IDText.text = guild.Id.ToString();
        NameText.text = guild.GuildName;
        MemberNumberText.text = guild.MemberCount.ToString();
        LeaderText.text = guild.LeaderName;
    }

    public override void OnSelected(bool selected)
    {
        Background.overrideSprite = selected ? SelectedSprite : null;
    }
}
