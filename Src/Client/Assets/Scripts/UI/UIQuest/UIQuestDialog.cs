using Assets.Scripts.Models;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIQuestDialog : UIWindow
{
    public UIQuestInfo questInfo;
    public GameObject OpenQuests;
    public GameObject SubmitQuest;
    public Quest quest;


    public void SetQuest(Quest quest)
    {
        this.quest = quest;
        this.UpdateQuest();
        if(quest.info == null)
        {
            OpenQuests.gameObject.SetActive(true);
            SubmitQuest.gameObject.SetActive(false);
        }
        else
        {
           if(quest.info.Status == QuestStatus.Completed)
            {
                OpenQuests.gameObject.SetActive(false);
                SubmitQuest.gameObject.SetActive(true);
            }
           else
            {
                OpenQuests.gameObject.SetActive(false);
                SubmitQuest.gameObject.SetActive(false);
            }
        }
    }

    private void UpdateQuest()
    {
        if(quest!= null)
        {
            if(questInfo != null)
            {
                questInfo.isDiolog = true;
                questInfo.SetQuestInfo(quest);
            }
        }
    }
}
