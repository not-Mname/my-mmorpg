using Const;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class UICharacterView : MonoBehaviour
{
	public GameObject[] characters;
	public Camera Camera;

    private int currentCharacter = 0;

	public int CurrentCharacter
	{
		get
		{
			return currentCharacter;
		}
		set
		{
			currentCharacter = value;
			this.UpdateCharacter();
		}
	}

	void UpdateCharacter()
	{
		for(int i = 1; i < characters.Length; i++)
		{
			characters[i].gameObject.SetActive(i == currentCharacter);
		}
	}
    private void OnMapChange(string mapName)
    {
		if(mapName == "CharSelect")
        {
            this.Camera.targetTexture = null;
        }
        EVENT.Unsubscribe<string>(EventId.on_map_unloaded, this.OnMapChange, EventMode.Multicast);
    }

    void Start()
	{
		EVENT.Subscribe<string>(EventId.on_map_unloaded, this.OnMapChange, EventMode.Multicast);
    }
}
