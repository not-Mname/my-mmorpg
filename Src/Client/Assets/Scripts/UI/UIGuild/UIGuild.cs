using Managers;
using Services;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuild : UIWindow
{

    public GameObject ItemPrefab;
    public ListView ListMain;
    public UIGuildInfo UIInfo;
    public UIGuildMemberItem SelectedItem;
    public Transform ItemRoot;

    public GameObject PanelAdmin;
    public GameObject PanelLeader;

    void Start()
    {
        this.ListMain.onItemSelected += OnGuildMemberSelected;
        GuildService.Instance.OnGuildUpdate += UpdateUI;
        UpdateUI();
    }
    void UpdateUI()
    {
        this.UIInfo.Info = GuildManager.Instance.GuildInfo;
        ClearList();
        InitList();

        this.PanelAdmin.SetActive(GuildManager.Instance.MyMemberInfo.Title > GuildTitle.None);
        this.PanelLeader.SetActive(GuildManager.Instance.MyMemberInfo.Title == GuildTitle.President);
    }

    private void InitList()
    {
        foreach (var member in GuildManager.Instance.GuildInfo.Members)
        {
            GameObject go = GameObject.Instantiate(ItemPrefab, this.ListMain.transform);
            UIGuildMemberItem ui = go.GetComponent<UIGuildMemberItem>();
            ui.SetGuildMemberInfo(member);
            ListMain.AddItem(ui);
        }
    }

    private void ClearList()
    {
        ListMain.RemoveAll();
    }

    void OnGuildMemberSelected(ListView.ListViewItem item)
    {
        SelectedItem = item as UIGuildMemberItem;
    }

    void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate -= UpdateUI;
        this.ListMain.onItemSelected -= OnGuildMemberSelected;
    }

    public void OnClickAppliesList()
    {
        UIManager.Instance.Show<UIGuildApplyList>();
    }
    public void OnClickLeave()
    {

    }
    public void OnClickChat()
    {

    }
    public void OnClickKickout()
    {
        if (SelectedItem != null)
        {
            MessageBox.Show("先选择成员再踢出");
            return;
        }
        MessageBox.Show(string.Format("你确定要踢出{0}吗？", SelectedItem.Info.Info.Name), "踢出成员", MessageBoxType.Confirm,
            "踢出", "取消").OnYes = () =>
            {
                GuildService.Instance.SendAdminCommand(GuildAdminCommand.Kickout, this.SelectedItem.Info.Info.Id);
            };
    }
    public void OnCkickPromote()
    {
        if (SelectedItem == null)
        {
            MessageBox.Show("先选择成员再升职");
            return;
        }
        if (SelectedItem.Info.Title == GuildTitle.VicePresident)
        {
            MessageBox.Show("对方已经一人之下万人之上了");
            return;
        }
        MessageBox.Show(string.Format("你确定要未{0}升职吗？", SelectedItem.Info.Info.Name), "升职", MessageBoxType.Confirm,
            "升职", "取消").OnYes = () =>
            {
                GuildService.Instance.SendAdminCommand(GuildAdminCommand.Promote, this.SelectedItem.Info.Info.Id);
            };
    }
    public void OnClickDepose()
    {
        if (SelectedItem == null)
        {
            MessageBox.Show("先选择成员再解散");
            return;
        }
        if (SelectedItem.Info.Title == GuildTitle.None)
        {
            MessageBox.Show("对方已经无职可罢了");
            return;
        }
        if (SelectedItem.Info.Title == GuildTitle.President)
        {
            MessageBox.Show("你不能解散，那是会长！");
            return;
        }
        MessageBox.Show(string.Format("你确定要罢免{0}的职务吗？", SelectedItem.Info.Info.Name), "罢免", MessageBoxType.Confirm,
            "罢免", "取消").OnYes = () =>
            {
                GuildService.Instance.SendAdminCommand(GuildAdminCommand.Depost, this.SelectedItem.Info.Info.Id);
            };
    }


    public void OnClickTransfer()
    {
        if (SelectedItem == null)
        {
            MessageBox.Show("先选择成员再转让");
            return;
        }
        MessageBox.Show(string.Format("你确定要将{0}转让给其他成员吗？", SelectedItem.Info.Info.Name), "转让", MessageBoxType.Confirm,
            "转让", "取消").OnYes = () =>
            {
                GuildService.Instance.SendAdminCommand(GuildAdminCommand.Transfer, this.SelectedItem.Info.Info.Id);
            };
    }

    public void OmClickSetNotice()
    {

    }
}


