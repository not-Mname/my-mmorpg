using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharInfo : MonoBehaviour {

    private SkillBridge.Message.NCharacterInfo _info;
    public SkillBridge.Message.NCharacterInfo Info
    {
        get { return _info; }
        set
        {
            _info = value;
            SetCharInfo();
        }
    }


    public Text name;
    public Text degree;
    public Image[] img;
    public CharacterClass Class;

    public void SetCharInfo()
	{
        name.text = _info.Name;
        degree.text = _info.Level.ToString();
        Class = _info.Class;

        for (int i = 0; i < img.Length; i++)
        {
            Debug.LogFormat("index + 1:{0}, class:{1}, i + 1 == (int)Class :{2}", i + 1, Class, i + 1 == (int)Class);
            img[i].gameObject.SetActive(i + 1 == (int)Class);
        }
    }
}
