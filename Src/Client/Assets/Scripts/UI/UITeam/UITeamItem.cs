using Managers;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITeamItem : ListView.ListViewItem
{
    public Image ClassIcon;
    public Image LeaderIcon;
    public Image BG;
    public Sprite Selectedicon;
    public Text Name;
    public Text Level;
    public int Idx;
    public NCharacterInfo Info;
    public override void OnSelected(bool selected)
    {
        BG.overrideSprite = selected ? Selectedicon : null;
    }

    public void SetMemberInfo(int idx, NCharacterInfo info, bool isLeader)
    {
        Idx = idx;
        Info = info;
        LeaderIcon.gameObject.SetActive(isLeader);
        Name.text = info.Name;
        Level.text = info.Level.ToString();
        ClassIcon.sprite = SpriteManager.Instance.sprites[(int)info.Class - 1];
    }
}
