using Managers;
using System;
using UIFramework;
using UnityEngine;
using UnityEngine.UI;

public class UIChat : PanelController
{
    //public HyperText TextArea;

    public UITabView CannelTab;

    public InputField ChatText;

    public Text ChatTargrt;

    public Dropdown ChannelSelect;

    void Start()
    {

        CannelTab.OnTabSelected += OnDisplayChannelSelected;
        ChatManager.Instance.OnChat += RefreshUI;
    }

    void Update()
    {
        InputManager.Instance.IsInputMode = ChatText.isFocused;//检查当前input是否有焦点
    }
    void OnDestroy()
    {
        ChatManager.Instance.OnChat -= RefreshUI;
    }

    void OnDisplayChannelSelected(int idx)
    {
        ChatManager.Instance.DisplayChannel = (ChatManager.LocalChannel)idx;
        RefreshUI();
    }

    private void RefreshUI()
    {
        //this.TextArea.text = ChatManager.Instance.GetCurrentMessages();
        this.ChannelSelect.value = (int)ChatManager.Instance.sendChannel - 1;
        if (ChatManager.Instance.sendChannel == ChatManager.LocalChannel.Private)
        {
            this.ChatTargrt.gameObject.SetActive(true);
            if (ChatManager.Instance.PrivateID != 0)
            {
                this.ChatTargrt.text = ChatManager.Instance.PrivateName + ";";
            }
            else
            {
                this.ChatTargrt.text = "<无>";
            }
        }
        else
        {
            this.ChatTargrt.gameObject.SetActive(false);
        }
    }
    public void OnClickChatLink(object text, object link)
    {
        //if (string.IsNullOrEmpty(link.Name)) return;

        ////<a name = "c:1001:name" class = "play">Name</a>//c开头是角色
        //if (link.Name.StartsWith("c:"))
        //{
        //    string[] strs = link.Name.Split(":".ToCharArray());
        //    UIPopChatMenu menu = UIManager.Instance.Show<UIPopChatMenu>();
        //    menu.TargetId = int.Parse(strs[1]);
        //    menu.TargetName = strs[2];

        //}
    }
    public void OnClickSend()
    {
        OnEndInput(this.ChatText.text);
    }

    public void OnEndInput(string text)
    {
        if (!string.IsNullOrEmpty(ChatText.text.Trim()))//trim是为了去除前后空格
            this.SendChat(ChatText.text.Trim());
        this.ChatText.text = "";
    }

    void SendChat(string content)
    {
        ChatManager.Instance.SendChat(content);
    }

    public void OnSendChannelChanged(int idx)
    {
        if (ChatManager.Instance.sendChannel == (ChatManager.LocalChannel)(ChannelSelect.value + 1))
            return;
        if(!ChatManager.Instance.SetSendChannle((ChatManager.LocalChannel)(ChannelSelect.value + 1)))
        {
            this.ChannelSelect.value = (int)ChatManager.Instance.sendChannel - 1;
        }
        else
        {
            this.RefreshUI();
        }
    }

}
