using Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class UIWindow : MonoBehaviour
{
    public delegate void CloseHandler(UIWindow sender, WindowResult result);
    public event CloseHandler OnClose;
    public GameObject Root;
    public virtual Type type { get { return GetType(); } }

    public enum WindowResult
    {
        None,
        Yes,
        No,
    }

    public void Close(WindowResult result = WindowResult.None)
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_Close);
        UIManager.Instance.Close(this.type);
        if (OnClose != null)
            OnClose(this, result);
        OnClose = null;
    }

    public virtual void OnCloseClick()
    {
        this.Close();
    }

    public virtual void OnNoClick()
    {
        this.Close(WindowResult.No);
    }

    public virtual void OnYesClick()
    {
        this.Close(WindowResult.Yes);
    }
}

