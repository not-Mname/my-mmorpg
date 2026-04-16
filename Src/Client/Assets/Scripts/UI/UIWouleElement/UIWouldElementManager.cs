using Entities;
using Managers;
using Models;
using System.Collections.Generic;
using UI;
using UnityEngine;

/// <summary>
/// 所有世界ui都应该在这里创建和管理，包括玩家头顶的名字条和头顶的元素等。
/// </summary>
public class UIWouldElementManager : MonoSingleton<UIWouldElementManager>
{
    public GameObject uiNameBar;
    public GameObject npcStatus;
    public GameObject popupText;

    public Dictionary<Transform, GameObject> elementPlayerNames = new Dictionary<Transform, GameObject>();
    public Dictionary<Transform, GameObject> elementStatus = new Dictionary<Transform, GameObject>();

    protected override void OnAwake()
    {
        base.OnAwake();
        if (uiNameBar == null)
        {
            uiNameBar = Resloader.Load<GameObject>("Prefab/UI/UIWorld/UINameBar");
        }
        if (npcStatus == null)
        {
            npcStatus = Resloader.Load<GameObject>("Prefab/UI/UIWorld/NPCStatusElement");
        }
        if (popupText == null)
        {
            popupText = Resloader.Load<GameObject>("Prefab/UI/UIWorld/UIPopupText");
        }
    }

    public void AddPopupText(PopupType type, Vector3 position, float damage, bool isCritical = false)
    {
        var go = Instantiate(popupText, position, Quaternion.identity, this.transform);
        go.name = "PopupText";
        go.GetComponent<UIPopupText>().Init(type, damage, isCritical, User.Instance.CurrentCharacterObject.transform);
    }

    public void AddPlayerElement(Transform owner, BattleUnit character)
    {
        GameObject go = Instantiate(uiNameBar, this.transform);
        go.GetComponent<UINameBar>().Init(character);
        go.GetComponent<UIWouldElement>().owner = owner;
        go.name = character.Name + character.Info.EntityId.ToString();
        elementPlayerNames.Add(owner, go);
        go.SetActive(true);
    } 

    public void AddStatusElement(Transform owner, NPCQuestStatus status)
    {
        if(elementStatus.ContainsKey(owner)) 
            this.elementStatus[owner].GetComponent<UIQuestStatus>().SetQuestStatus(status);
        else
        {
            GameObject go = Instantiate(npcStatus, this.transform);
            go.GetComponent<UIWouldElement>().owner = owner;
            go.GetComponent<UIQuestStatus>().SetQuestStatus(status);
            go.name = "Status" + owner.name;
            elementStatus.Add(owner, go);
            go.SetActive(true);
        }
    }

    public void RemoveStatusElement(Transform owner)
    {
        if (Instance != null && elementStatus.ContainsKey(owner))
        {
            Destroy(elementStatus[owner]);
            elementStatus.Remove(owner);
        }
    }
    public void RemoveElement(Transform owner)
    {
        if (Instance != null && elementPlayerNames.ContainsKey(owner))
        {
            Destroy(elementPlayerNames[owner]);
            elementPlayerNames.Remove(owner);
        }
    }
}
