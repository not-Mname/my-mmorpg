using Battle;
using Entities;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIBuffIcons : MonoBehaviour
{
    private BattleUnit _owner;
    private Dictionary<int, UIBuffItem> _buffIcons = new ();
    private string _buffIconPrefabPath = "Prefab/UI/UISkill/Item/UIBuffItem";

    private void OnDestroy()
    {
        foreach (UIBuffItem item in this._buffIcons.Values)
        {
            Destroy(item.gameObject);
        }
        this._buffIcons.Clear();
        if (this._owner != null)
        {
            this._owner.OnBuffAdd -= this.OnBuffAdd;
            this._owner.OnBuffRemove -= this.OnBuffRemove;
        }
        
    }

    public void Init(BattleUnit characterInfo)
    {
        this._owner = characterInfo;
        this._owner.OnBuffAdd += this.OnBuffAdd;
        this._owner.OnBuffRemove += this.OnBuffRemove;
    }

    public void OnBuffAdd(Buff buff)
    {
        GameObject buffIcon = Instantiate(Resloader.Load<GameObject>(this._buffIconPrefabPath), this.transform);
        UIBuffItem item = buffIcon.GetComponent<UIBuffItem>();
        item.Init(buff);
        this._buffIcons.Add(buff.BuffId, item);
    }

    public void OnBuffRemove(Buff buff)
    {
        if (!this._buffIcons.ContainsKey(buff.BuffId))
        {
            return;
        }
        Destroy(this._buffIcons[buff.BuffId].gameObject);
        this._buffIcons.Remove(buff.BuffId);
    }
}
