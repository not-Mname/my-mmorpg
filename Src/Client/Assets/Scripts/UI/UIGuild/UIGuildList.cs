using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildList : UIWindow
{
    public GameObject ItemPrefab;
    public ListView ListMain;
    public UIGuildInfo UIInfo;
    public UIGuildItem SelectedItem;
    public Transform itemRoot;

    void Start()
    {
        this.ListMain.onItemSelected += OnGuildMemberSelected;
        this.UIInfo.Info = null;
        GuildService.Instance.OnGuildListResult += UpdateGuildList;
        GuildService.Instance.SendGuildListRequest();
    }

    void UpdateGuildList(List<NGuildInfo> guilds)
    {
        ClearList();
        InitList(guilds);
    }

    private void InitList(List<NGuildInfo> guilds)
    {
        foreach (var guild in guilds)
        {
            GameObject go = GameObject.Instantiate(ItemPrefab, this.ListMain.transform);
            UIGuildItem ui = go.GetComponent<UIGuildItem>();
            ui.SetInfo(guild);
            ListMain.AddItem(ui);
        }
    }

    private void ClearList()
    {
        ListMain.RemoveAll();
    }

    void OnGuildMemberSelected(ListView.ListViewItem item)
    {
        SelectedItem = item as UIGuildItem;
        this.UIInfo.Info = SelectedItem.Info;
    }

    void OnDestroy()
    {
        this.ListMain.onItemSelected -= OnGuildMemberSelected;
    }

    public void OnClickJoin()
    {
        Debug.Log("OnClickJoin");
        if (SelectedItem == null)
        {
            MessageBox.Show("请选择一个公会");
            return;
        }
        MessageBox.Show(string.Format("你确定要加入公会{0}吗？", SelectedItem.Info.GuildName), "加入公会",MessageBoxType.Confirm,
            "确定", "取消").OnYes = () => 
            {
                GuildService.Instance.SendGuileJoinRequest(SelectedItem.Info.Id);
            };
    }
}
