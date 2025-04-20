using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UI;

public class UITabButton : MonoBehaviour
{
    public GameObject page;
    public Sprite selectedIcon;
    public GameObject content;

    public void Selected()
    {
        this.GetComponent<Image>().overrideSprite = selectedIcon;
        if (page != null)
            page.SetActive(true);
    }

    public void Deselected()
    {
        this.GetComponent<Image>().overrideSprite = null;
        if(page != null)
            page.SetActive(false);
    }
}
