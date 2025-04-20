using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UITabView : MonoBehaviour
{
    public UITabButton[] pageControler;
    public GameObject[] pages;

    public UnityAction<int> OnTabSelected;

    IEnumerator Start()
    {
        for(int i = 0; i < pageControler.Length; i++)
        {
            if(pages.Length > i && pages[i] != null)
                pageControler[i].page = pages[i];
            int index = i;
            pageControler[i].GetComponent<Button>().onClick.AddListener(() => SelectTab(index));
        }
        yield return new WaitForEndOfFrame();
        SelectTab(0);
    }

    public void SelectTab(int index)
    {
        pageControler[index].Selected();
        if(OnTabSelected != null)
            OnTabSelected(index);
        for (int i = 0; i < pageControler.Length; i++)
        {
            if(i != index)
            {
                pageControler[i].Deselected();
            }
        }
    }
}
