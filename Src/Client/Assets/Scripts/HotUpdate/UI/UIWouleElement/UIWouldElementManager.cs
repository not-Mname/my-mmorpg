using Asset;
using AssetBundleFramework;
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
    string _uiNameBar = "Assets/AssetBundle/Prefab/UI/UIWorld/UINameBar.prefab";
    string _npcStatus = "Assets/AssetBundle/Prefab/UI/UIWorld/NPCStatusElement.prefab";
    string _popupText = "Assets/AssetBundle/Prefab/UI/UIWorld/UIPopupText.prefab";

    public Dictionary<Transform, GameObject> elementPlayerNames = new();
    public Dictionary<Transform, GameObject> elementStatus = new();

    protected override void OnDestroy()
    {
        base.OnDestroy();

    }

    public void AddPopupText(PopupType type, Vector3 position, float damage, bool isCritical = false)
    {
        IResource popupText = Resloader.Instance.LoadAssetSync(_popupText);
        var go = popupText.Instantiate(transform, Quaternion.identity, true);
        go.name = "PopupText";
        go.GetComponent<UIPopupText>().Init(type, damage, isCritical, User.Instance.CurrentCharacterObject.transform);
    }

    public void AddPlayerElement(Transform owner, BattleUnit character)
    {
        IResource uiNameBar = Resloader.Instance.LoadAssetSync(_uiNameBar);
        GameObject go = uiNameBar.Instantiate(this.transform, false, true);
        go.GetComponent<UINameBar>().Init(character);
        go.GetComponent<UIWouldElement>().owner = owner;
        go.name = character.Name + character.Info.EntityId.ToString();
        elementPlayerNames.Add(owner, go);
        go.SetActive(true);
    }

    public void AddStatusElement(Transform owner, NPCQuestStatus status)
    {

        if (elementStatus.ContainsKey(owner))
            this.elementStatus[owner].GetComponent<UIQuestStatus>().SetQuestStatus(status);
        else
        {
            IResource npcStatus = Resloader.Instance.LoadAssetSync(_npcStatus);
            GameObject go = npcStatus.Instantiate(this.transform, false, true);
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
