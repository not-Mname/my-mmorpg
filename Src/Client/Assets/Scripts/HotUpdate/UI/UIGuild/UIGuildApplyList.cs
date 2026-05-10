using Managers;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuildApplyList : UIWindow
{
    public GameObject ItmePrefab;
    public ListView ListMain;
    public Transform ListRoot;

    void Start()
    {
        GuildService.Instance.OnGuildUpdate += UpdateList;
        GuildService.Instance.SendGuildListRequest();
        UpdateList();
    }

    void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate -= UpdateList;
    }

    void UpdateList()
    {
        ClearList();
        InitItems();
    }

     void InitItems()
    {
        foreach (var apply in GuildManager.Instance.GuildInfo.Applies)
        {
            GameObject go = Instantiate(ItmePrefab, ListMain.transform);
            UIGuildApplyItem ui = go.GetComponent<UIGuildApplyItem>();
            ui.SetItemInfo(apply);
            this.ListMain.AddItem(ui);
        }
    }

     void ClearList()
    {
        this.ListMain.RemoveAll();
    }
}
