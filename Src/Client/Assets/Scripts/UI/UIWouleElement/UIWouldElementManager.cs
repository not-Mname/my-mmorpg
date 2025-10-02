using Entities;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWouldElementManager : MonoSingleton<UIWouldElementManager>
{
    public GameObject uiNameBar;
    public GameObject npcStatus;

    public Dictionary<Transform, GameObject> elementPlayerNames = new Dictionary<Transform, GameObject>();
    public Dictionary<Transform, GameObject> elementStatus = new Dictionary<Transform, GameObject>();

    public void AddPlayerElement(Transform owner, BattleUnit character)
    {
        GameObject go = Instantiate(uiNameBar, this.transform);
        go.GetComponent<UINameBar>().characterInfo = character;
        go.GetComponent<UIWouldElement>().owner = owner;
        go.name = character.Name + character.Info.EntityId.ToString();
        elementPlayerNames.Add(owner, go);
        go.SetActive(true);
    }

    public void RemoveElement(Transform owner)
    {
        if(UIWouldElementManager.Instance != null && elementPlayerNames.ContainsKey(owner))
        {
            Destroy(elementPlayerNames[owner]);
            elementPlayerNames.Remove(owner);
        }
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
        if (UIWouldElementManager.Instance != null && elementStatus.ContainsKey(owner))
        {
            Destroy(elementStatus[owner]);
            elementStatus.Remove(owner);
        }
    }

    protected override void OnStart()
    {

    }

    void Update()
    {

    }
}
