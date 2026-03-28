using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIIconItem : MonoBehaviour
{
    public Image main;
    public Text mainText;
    
    public void SetMainIcon(string iconName, string text)
    {
        main.overrideSprite = Resloader.Load<Sprite>(iconName);
        mainText.text = text;
    }

   
}
