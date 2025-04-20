using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFriendItem : ListView.ListViewItem 
{
	public Text NameText;
	public Text LevelText;
	public Text ClassText;
	public Text StatusText;
	public Image OriginImage;
    public Sprite SelectedBG;

	public NFriendInfo FriendInfo;

    public  override void OnSelected(bool selected)
	{
        OriginImage.overrideSprite = selected ? SelectedBG : null;
    }

    public void SetFriendInfo(NFriendInfo friendInfo)
	{
		this.FriendInfo = friendInfo;
		if(this.NameText != null)this.NameText.text = friendInfo.friendInfo.Name;
		if(this.LevelText != null) this.LevelText.text = "Lv." + friendInfo.friendInfo.Level.ToString();
		if(this.ClassText != null) this.ClassText.text = friendInfo.friendInfo.Class.ToString();
		if(this.StatusText != null) this.StatusText.text = friendInfo.Status == 1 ? "在线" : "离线";
	}
}
