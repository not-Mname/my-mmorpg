using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]//意思是这个类可以被序列化
public class ItemSelectEvent : UnityEvent<ListView.ListViewItem>
{

}

public class ListView : MonoBehaviour
{
    public UnityAction<ListViewItem> onItemSelected;
    public class ListViewItem : MonoBehaviour, IPointerClickHandler
    {
        private bool Selected;
        public bool selected
        {
            get
            {
                return Selected;
            }
            set
            {
                Selected = value;
                OnSelected(Selected);
            }
        }

        public virtual void OnSelected(bool selected)
        {

        }

        public ListView owner;

        public void OnPointerClick(PointerEventData eventData)
        {
            if(!selected)
                selected = true;
            if(owner!= null && owner.selectedItem!= this)
                owner.selectedItem = this;
        }

    }

    List<ListViewItem> items;
    ListViewItem SelectedItem = null;
    public ListViewItem selectedItem
    {
        get
        {
            return SelectedItem;
        }
        private set
        {
            if (SelectedItem != null && SelectedItem != value)
            {
                SelectedItem.selected = false;
            }
            SelectedItem = value;
            if (onItemSelected != null)
                onItemSelected(SelectedItem);
        }
    }

    public void AddItem(ListViewItem item)
    {
        if (item == null) return;
        if (items == null) items = new List<ListViewItem>();
        item.owner = this;
        items.Add(item);
    }

    public void RemoveAll()
    {   
        if (items == null) return;
        foreach (var item in items)
        {
            if (item == null) continue;
            Destroy(item.gameObject);
        }
        items.Clear();
    }
}


