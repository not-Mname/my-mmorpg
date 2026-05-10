using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIInputBox : MonoBehaviour
{
    public Text Title;
    public InputField Input;
    public Text Message;
    public Text Tips;
    public Button ButtonYes;
    public Button ButtonCancel;

    public Text TextYesTitle;
    public Text TextNoTitle;

    public Text ButtonYesText;
    public Text ButtonCancelText;

    public delegate bool SubmitHandler(string input, out string tips);
    public SubmitHandler OnSubmit;
    public UnityAction OnCancel;

    public string EmptyTips;

    public void Init(string title, string message, string btnOK = "",string btnCancel = "", string emptyTips = "")
    {
        if(!string.IsNullOrEmpty(title)) Title.text = title;
        this.Message.text = message;
        this.Tips.text = null;
        this.OnSubmit = null;
        this.EmptyTips = emptyTips;
        if(!string.IsNullOrEmpty(btnOK)) this.ButtonYesText.text = btnOK;
        if(!string.IsNullOrEmpty(btnCancel)) this.ButtonCancelText.text = btnCancel;

        this.ButtonYes.onClick.AddListener(this.OnClickYes);
        this.ButtonCancel.onClick.AddListener(this.OnClickCancel);
    }

    private void OnClickYes()
    {
        this.Tips.text = null;
        if(string.IsNullOrEmpty(this.Input.text))
        {
            this.Tips.text = this.EmptyTips;
            return;
        }
        if(this.OnSubmit!= null)
        {
            string tips;
            if(!this.OnSubmit(this.Input.text, out tips))
            {
                this.Tips.text = tips;
                return;
            }
        }
        Destroy(this.gameObject);
        
    }

    private void OnClickCancel()
    {
        Destroy(this.gameObject);
        if(this.OnCancel!= null)
            OnCancel();
    }
}
