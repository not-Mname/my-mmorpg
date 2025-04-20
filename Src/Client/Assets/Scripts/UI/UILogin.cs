using Models;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILogin : MonoBehaviour
{
	public InputField userName;
	public InputField password;

	void Start()
	{
		UserService.Instance.OnLogin = this.OnLogin;
	}


	void Update()
	{

	}

	public void OnClickLogin()
	{
		SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
		if (string.IsNullOrEmpty(userName.text))
		{
			MessageBox.Show("请输入用户名");
		}
		if (string.IsNullOrEmpty(password.text))
		{
			MessageBox.Show("请输入密码");

        }
		UserService.Instance.SendLogin(userName.text, password.text);

	}

	public void OnLogin(SkillBridge.Message.Result result, string msg)
	{
		if (result == SkillBridge.Message.Result.Success)
		{
            Debug.Log("登录成功");
			SceneManager.Instance.LoadScene("CharSelect");
            SoundManager.Instance.PlayMusic(SoundDefine.Music_Select);
        }
		else
			MessageBox.Show(string.Format("登录失败，原因：{0}", msg));

	}
}
