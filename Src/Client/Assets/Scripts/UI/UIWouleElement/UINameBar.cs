using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINameBar : MonoBehaviour
{
    public Text Name;
    public BattleUnit characterInfo;

    void Start()
    {
        
    }


    void Update()
    {
        if (characterInfo != null) UpdateInfo();
        
        
    }

    void UpdateInfo()
    {
        if (characterInfo == null) return;
        string name = string.Format("{0} Lv.[{1}]", characterInfo.Name, characterInfo.Info.Level);
        if(Name.text!= name)
            Name.text = name;
        
    }
}
