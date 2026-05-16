using Managers;
using Models;
using Services;
using UnityEngine;

public class UIFriend : UIWindow
{
    public GameObject ItemPrefab;
    public ListView ListMain;
    public Transform ItemRoot;
    public UIFriendItem SelectedItem;
    void Start()
    {
        FriendService.Instance.OnFriendUpdata += RefreshUI;
        ListMain.onItemSelected += OnFriendSelected;
        RefreshUI();
    }

    public void OnFriendSelected(ListView.ListViewItem item)
    {
        this.SelectedItem = item as UIFriendItem;
    }

    public void RefreshUI()
    {
        this.ClearFriendItems();
        this.InitFriendItems();
    }

    private void InitFriendItems()
    {
        foreach (var item in FriendManager.Instance.allFriends)
        {
            GameObject go = GameObject.Instantiate(ItemPrefab, ItemRoot);
            UIFriendItem uiItem = go.GetComponent<UIFriendItem>();
            uiItem.SetFriendInfo(item);
            ListMain.AddItem(uiItem);

        }
    }

    private void ClearFriendItems()
    {
        this.ListMain.RemoveAll();
    }

    public void OnClickFriendAdd()
    {
        InputBox.Show("请输入好友ID或名称", "添加好友").OnSubmit += OnFriendAddSubmit;
    }



    bool OnFriendAddSubmit(string input, out string tips)
    {
        tips = "";
        int friendId = 0;
        string friendName = "";
        if (!int.TryParse(input, out friendId))//TryParse意味着如果input可以转换成int，则返回true，并将结果赋值给friendId，否则返回false，friendId的值不变
            friendName = input;
        if (friendId == User.Instance.CurrentCharacterInfo.Id || friendName == User.Instance.CurrentCharacterInfo.Name)
        {
            tips = "开玩笑，不能添加自己为好友";
            return false;
        }
        FriendService.Instance.SendAddFriendRequest(friendName, friendId);
        return true;
    }

    public void OnClickTeamInvite()
    {
        if (SelectedItem == null)
        {
            MessageBox.Show("请先选择好友");
            return;
        }
        if (SelectedItem.FriendInfo.Status == 0)
        {
            MessageBox.Show("该好友不在线");
            return;
        }
        MessageBox.Show(string.Format("确定邀请{0}加入你的队伍吗？", SelectedItem.NameText.text), "邀请加入队伍",
            MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
            {
                TeamService.Instance.SendTeamInviteRequest(SelectedItem.FriendInfo.FriendInfo.Id, SelectedItem.FriendInfo.FriendInfo.Name);
            };

    }

    public void OnClickChallenge()
    {
        if (SelectedItem == null)
        {
            MessageBox.Show("请先选择好友");
            return;
        }
        if (SelectedItem.FriendInfo.Status == 0)
        {
            MessageBox.Show("该好友不在线");
            return;
        }
        MessageBox.Show(string.Format("确定挑战{0}吗？", SelectedItem.FriendInfo.FriendInfo.Name), "竞技场挑战",
            MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
            {
                ArenaService.Instance.SendArenaChallengeRequest(SelectedItem.FriendInfo.FriendInfo.EntityId, SelectedItem.FriendInfo.FriendInfo.Name);
            };

    }

    public void OnClickFriendRemove()
    {
        if (SelectedItem == null)
        {
            MessageBox.Show("请先选择好友");
            return;
        }
        MessageBox.Show(string.Format("确定删除好友{0}吗？", SelectedItem.NameText.text), "删除好友",
            MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
            {
                FriendService.Instance.SendFriendRemoveRequest(User.Instance.CurrentCharacterInfo.Id, SelectedItem.FriendInfo.FriendInfo.Id);
            };
    }

    public void OnClickFriendChat()
    {

    }
}
