using Models;
using Services;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterSelect : MonoBehaviour
{
    public GameObject characterCreatePanel;
    public Image[] logoImage;
    public Toggle[] toggles;
    public Text characterDescription;
    public UICharacterView uiCharacterView;
    public InputField characterName;
    CharacterClass characterClass = CharacterClass.Warrior;

    public GameObject uiCharacterSelectPanel;
    public GameObject uiUserInfo;
    public Transform uiChars;
    public List<GameObject> uiList = new List<GameObject>();

    int lastSelected = 1;
    int currentCharId = -1;
    #region CharacterCreate 代码

    public void OnSelectCreatedCharacter(int index)
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
        if (lastSelected == index)
        {
            //Debug.Log("lastSelected: " + lastSelected + " index: " + index);
            toggles[index - 1].isOn = true;
            return;
        }
        if (!(toggles[index - 1].isOn)) return;
        //Debug.Log(toggles[index-1].isOn+ " " + index);

        lastSelected = index;

        OnSelectClass(index);
        for (int i = 0; i < logoImage.Length; i++)
        {
            bool isActive = i + 1 == index;
            logoImage[i].gameObject.SetActive(isActive);
            toggles[i].isOn = isActive;
        }

        characterDescription.text = DataManager.Instance.Characters[index].Description;
    }

    public void OnSelectClass(int characterClass)
    {
        //Debug.Log("OnSelectClass: " + characterClass);
        this.characterClass = (CharacterClass)characterClass;
        uiCharacterView.CurrentCharacter = characterClass;
    }

    public void OnCreatCharacter(Result result, string message)
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
        if (result == Result.Success)
        {
            Debug.LogFormat("创建角色：{0}", result);
            InitCharacterSelect(true);
        }
        else
        {
            MessageBox.Show(string.Format("创建角色失败：{0} 原因： {1}", result, message));
            Debug.LogFormat("创建角色失败：{0} 原因： {1}", result, message);
        }
    }

    public void OnClickCreateCharacter()
    {
        if (string.IsNullOrEmpty(characterName.text))
        {
            MessageBox.Show("角色名不能为空！");
            return;
        }

        UserService.Instance.SendCreateCharacter(characterName.text, characterClass);
    }

    public void OnGetIntoCharacterCreatePanel()
    {
        characterCreatePanel.SetActive(true);
        uiCharacterSelectPanel.SetActive(false);
        InitCharacterCreate();
    }

    public void InitCharacterCreate()
    {
        uiCharacterView.CurrentCharacter = 1;
        characterDescription.text = DataManager.Instance.Characters[1].Description;
    }

    public void OnBackToCharacterSelect()
    {
        InitCharacterSelect(true);
        characterCreatePanel.SetActive(false);
        uiCharacterSelectPanel.SetActive(true);
    }
    #endregion

    #region CharacterSelect 代码
    public void InitCharacterSelect(bool init)
    {
        characterCreatePanel.SetActive(false);
        uiCharacterSelectPanel.SetActive(true);
        if (init)
        {
            foreach (var old in uiList)
            {
                Destroy(old);
            }
            uiList.Clear();
        }

        for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
        {
            GameObject go = Instantiate(uiUserInfo, uiChars);
            UICharInfo info = go.GetComponent<UICharInfo>();
            info.Info = User.Instance.Info.Player.Characters[i];

            int index = i;
            Toggle toggle = info.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener((bool isSelected) =>
            {
                if (isSelected)
                    OnClickSelectChar(index);
            });
            toggle.group = toggle.transform.parent.GetComponent<ToggleGroup>();
            toggle.isOn = false;

            uiList.Add(go);
            go.SetActive(true);
        }

    }

    public void OnClickSelectChar(int index)
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
        GameObject go = uiList[index];
        currentCharId = go.GetComponent<UICharInfo>().Info.EntityId;
        uiCharacterView.CurrentCharacter = (int)go.GetComponent<UICharInfo>().Info.Class;
    }

    public void OnClickPlay()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
        if (uiList.Count == 0)
        {
            MessageBox.Show("没有角色可以进入游戏！");
            return;
        }
        else
        {
            foreach (var go in uiList)
            {
                if (go.GetComponent<Toggle>().isOn)
                {
                    currentCharId = go.GetComponent<UICharInfo>().Info.Id;
                    break;
                }
            }
            UserService.Instance.SendGameEnter(currentCharId);
            SoundManager.Instance.MusicAudioSource.Stop();
        }
    }

    public void OnGameEnter(Result result, string message)
    {
        if (result == Result.Success)
            Debug.LogFormat("进入游戏：{0}", result);
        else
            Debug.LogFormat("进入游戏失败：{0} 原因： {1}", result, message);
    }

    #endregion

    void Start()
    {
        InitCharacterSelect(false);
        uiCharacterView.CurrentCharacter = -1;
        UserService.Instance.OnCreatCharacter += OnCreatCharacter;
        UserService.Instance.OnGameEnter += OnGameEnter;
    }

    void Update()
    {

    }
}
