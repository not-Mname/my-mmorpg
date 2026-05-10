using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICharacterView : MonoBehaviour
{
	public GameObject[] characters; 

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
			//Debug.Log(i + "" + currentCharacter + ""+ (i == currentCharacter));
			characters[i].gameObject.SetActive(i == currentCharacter);
		}
	}

	void Start()
	{

	}

	void Update()
	{

	}
}
