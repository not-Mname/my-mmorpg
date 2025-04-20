using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestStatus : MonoBehaviour
{
    public Image questIcon;
    public Sprite availableSprite;
    public Sprite completedSprite;
    public Sprite incompleteSprite;

    public NPCQuestStatus questStatus;
    public void SetQuestStatus(NPCQuestStatus status)
    {
        questStatus = status;
        switch (status)
        {
            case NPCQuestStatus.Available:
                questIcon.sprite = availableSprite;
                break;
            case NPCQuestStatus.Complete:
                questIcon.sprite = completedSprite;
                break;
            case NPCQuestStatus.Incomplete:
                questIcon.sprite = incompleteSprite;
                break;
            case NPCQuestStatus.None:
                questIcon.sprite = null;
                break;
            default:
                break;
        }
    }
}
