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

    public void SetFriendInfo(NFriendInfo FriendInfo)
	{
		this.FriendInfo = FriendInfo;
		if(this.NameText != null)this.NameText.text = FriendInfo.FriendInfo.Name;
		if(this.LevelText != null) this.LevelText.text = "Lv." + FriendInfo.FriendInfo.Level.ToString();
		if(this.ClassText != null) this.ClassText.text = FriendInfo.FriendInfo.Class.ToString();
		if(this.StatusText != null) this.StatusText.text = FriendInfo.Status == 1 ? "在线" : "离线";
	}
}
