using Entities;
using Models;
using SkillBridge.Message;
using System;
using TMPro;
using UI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainUI
{

    public class UIAvatar : MonoBehaviour
    {
        /////////////////////////////// UI组件 /////////////////////////

        public TextMeshProUGUI TextName;
        public TextMeshProUGUI TextLevel;
        public Image ImageAvatar;
        public ProgressBar HPBar;
        public ProgressBar MPBar;

        /////////////////////////////// 公有变量 ///////////////////////
        
        /////////////////////////////// 私有变量 ///////////////////////

        /////////////////////////////// 公有函数 ///////////////////////
        public void UpdateUI()
        {
           
        }

        public void Init()
        {
            var cha = User.Instance.CurrentCharacter;
            TextName.text = cha.Name;
            TextLevel.text = cha.Info.Level.ToString();
            HPBar.SetData(cha.Attributes.MaxHp, cha.Info.Dynamic.Hp);
            MPBar.SetData(cha.Attributes.MaxMp, cha.Info.Dynamic.Mp);
        }

        /////////////////////////////// 私有函数 ///////////////////////

        private void Start()
        {

        }
    }


}