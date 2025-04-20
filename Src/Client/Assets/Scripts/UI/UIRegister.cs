using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRegister : MonoBehaviour 
{
	public InputField email;
	public InputField password;
    public InputField ensuredPassword;
    void Start()
	{
        UserService.Instance.OnRegister = this.OnRegister;
    }
	
	void Update () 
	{
		
	}

    void OnRegister(SkillBridge.Message.Result result, string msg)
    {
        MessageBox.Show(string.Format("结果：{0} 消息：{1}", result, msg));
    }

    public void OnClickRegister()
	{
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
        Debug.Log("OnRegister");
		if (string.IsNullOrEmpty(email.text))
		{
			MessageBox.Show("邮箱不能为空");
			return;
		}
		if (string.IsNullOrEmpty(password.text))
		{
			MessageBox.Show("密码不能为空");
			return;
		}
		if (string.IsNullOrEmpty(ensuredPassword.text))
		{
			MessageBox.Show("确认密码不能为空");
			return;
		}
		if (password.text!= ensuredPassword.text)
		{
			MessageBox.Show("两次密码输入不一致");
			return;
		}
		UserService.Instance.SendRegister(email.text, password.text);
	}
}
