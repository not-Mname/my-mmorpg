using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildApplyItem : ListView.ListViewItem
{
    public Text NameText;
    public Text LevelText;
    public Text ClassText;

    public NGuildApplyInfo Info;

    public void SetItemInfo(NGuildApplyInfo apply)
    {
        Info = apply;
        if (NameText!= null) NameText.text = apply.Name;
        if (LevelText!= null) LevelText.text = "Lv." + apply.Level;
        if (ClassText!= null) ClassText.text = ((CharacterClass)apply.Class).ToString();
    }

    public void OnClickApply()
    {
        MessageBox.Show(string.Format("要通过[{0}]的加入申请吗？", Info.Name), "申请加入公会", 
            MessageBoxType.Confirm, "通过", "拒绝").OnYes = () => 
            {
                GuildService.Instance.SendGuildJoinApply(true, Info);
            };
    }
    public void OnClickDecline()
    {
        MessageBox.Show(string.Format("要拒绝[{0}]的加入申请吗？", Info.Name), "申请加入公会",
            MessageBoxType.Confirm, "通过", "拒绝").OnYes = () =>
            {
                GuildService.Instance.SendGuildJoinApply(false, Info);
            };
    }
}
