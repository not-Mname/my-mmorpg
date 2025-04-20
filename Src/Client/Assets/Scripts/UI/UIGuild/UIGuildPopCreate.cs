using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildPopCreate : UIWindow
{
    public InputField InputName;
    public InputField InputNotice;

    void Start()
    {
        GuildService.Instance.OnCreateGuildResult += OnGuildCreate;
    }

    void OnDestroy()
    {
        GuildService.Instance.OnCreateGuildResult  = null;

    }

    public override void OnYesClick()
    {
        if(string.IsNullOrEmpty(InputName.text))
        {
            MessageBox.Show("请输入公会名称！");
            return;
        }
        if (InputName.text.Length < 4 || InputName.text.Length > 10)
        {
            MessageBox.Show("公会名称长度必须在4-10个字符之间！", "错误", MessageBoxType.Error);
            return;
        }
        if (string.IsNullOrEmpty(InputNotice.text))
        {
            MessageBox.Show("请输入公会宣言！");
            return;
        }
        if(InputNotice.text.Length < 30 || InputNotice.text.Length > 50)
        {
            MessageBox.Show("公会公告长度必须在3-50个字符之间！", "错误", MessageBoxType.Error);
            return;
        }
        GuildService.Instance.SendGuildCreate(InputName.text, InputNotice.text);
    }
    void OnGuildCreate(bool result)
    {
        if (result)
            this.Close(WindowResult.Yes);
    }
}
