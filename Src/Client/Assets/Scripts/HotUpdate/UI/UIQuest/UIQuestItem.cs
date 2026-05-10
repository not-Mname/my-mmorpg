using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestItem : ListView.ListViewItem
{
    public Text title;
    public Text content;
    public Image icon;
    public Sprite selectBG;

    public Quest quest;

    public override void OnSelected(bool selected)
    {
       this.icon.overrideSprite = selected? selectBG : null;
    }

    public void SetQuestInfo(Quest item)
    {
        this.quest = item;
        if (title!= null) title.text = item.define.Type == Common.Data.QuestType.Main ? "[主线]" : "[支线]";
        if (content!= null) content.text = item.define.Name;
    }
}
