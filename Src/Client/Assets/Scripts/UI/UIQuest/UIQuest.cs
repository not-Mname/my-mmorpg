using Managers;
using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum QuestStatu
{
    None,
    Running,
    Avaiable,
}

public class UIQuest : UIWindow
{
    public ListView main;
    public ListView branch;
    public GameObject uiQuestItem;
    public UITabView tabView;
    public UIQuestInfo questInfo;
    public Button btnShowAvaiable;
    public Button btnShowRunning;

    private bool showAvaiableList = false;

    void Start()
    {
        this.main.onItemSelected += OnItemSelected;
        this.branch.onItemSelected += OnItemSelected;
        btnShowRunning.onClick.AddListener(() => { this.RefreshUI(QuestStatu.Running); });
        btnShowAvaiable.onClick.AddListener(() => { this.RefreshUI(QuestStatu.Avaiable); });
        RefreshUI(QuestStatu.None);
    }

    private void OnItemSelected(ListView.ListViewItem item)
    {
        UIQuestItem questItem = item as UIQuestItem;
        this.questInfo.SetQuestInfo(questItem.quest);
    }

    public void RefreshUI(QuestStatu type)
    {
        this.ClearAllQuestList();
        this.InitAllQuetItem(type);
    }

    private void InitAllQuetItem(QuestStatu type)
    {

        foreach (var kv in QuestManager.Instance.allQuests)
        {
            if (type == QuestStatu.Avaiable)
            {//如果是可接，服务器没有信息，那么info一定为空
                if(kv.Value.info != null)
                    continue;
            }
            else if (type == QuestStatu.Running)
            {//如果是进行中，服务器有信息，那么info一定不为空
                if (kv.Value.info == null)
                    continue;
            }
            else if(kv.Value.info != null && kv.Value.info.Status == QuestStatus.QuestFinished)
            {
                continue;
            }

                GameObject item = GameObject.Instantiate(uiQuestItem,
                    kv.Value.define.Type == QuestType.Main ? main.transform : branch.transform);

            UIQuestItem ui = item.GetComponent<UIQuestItem>();
            ui.SetQuestInfo(kv.Value);
            if(kv.Value.define.Type == QuestType.Main)
            {
                main.AddItem(ui as ListView.ListViewItem);
            }
            else
            {
                branch.AddItem(ui as ListView.ListViewItem);
            }
        }

    }

    private void ClearAllQuestList()
    {
        main.RemoveAll();
        branch.RemoveAll();
    }
}
