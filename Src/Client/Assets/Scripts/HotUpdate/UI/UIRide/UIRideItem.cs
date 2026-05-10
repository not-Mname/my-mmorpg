using Asset;
using Models;
using UnityEngine;
using UnityEngine.UI;


class UIRideItem : ListView.ListViewItem
{
    public Image Icon;
    public Text Name;
    public Text Level;

    public Sprite SelectedBG;
    public Image BG;
    public Item Item;

    public override void OnSelected(bool selected)
    {
        BG.overrideSprite = selected ? SelectedBG : null;
    }

    public void SetRideItem(Item item)
    {
        this.Item = item;
        if(Name != null)   Name.text = item.define.Name;
        if(Level!= null)   Level.text = item.define.Level.ToString();
        if(Icon != null)   Icon.sprite = Resloader.Instance.LoadAssetSync(item.define.Icon).GetAsset<Sprite>();
    }
}

