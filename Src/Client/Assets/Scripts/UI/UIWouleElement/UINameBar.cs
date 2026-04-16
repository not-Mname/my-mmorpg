using Entities;
using Models;
using TMPro;
using UnityEngine;

public class UINameBar : MonoBehaviour
{
    public TextMeshProUGUI Name;
    private BattleUnit _characterInfo;
    public UIBuffIcons BuffIcons;

    public void Init(BattleUnit characterInfo)
    {
        _characterInfo = characterInfo;
        UpdateInfo();
        BuffIcons.Init(_characterInfo);
    }

    void UpdateInfo()
    {
        string name = string.Format("{0} Lv.[{1}]", _characterInfo.Name, _characterInfo.Info.Level);
        if (Name.text != name)
        { Name.text = name; }

    }
}
