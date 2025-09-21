using Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Managers
{
    class GuildManager : Singleton<GuildManager>
    {
        public NGuildInfo GuildInfo;
        public NGuildMemberInfo MyMemberInfo;

        public bool HasGuild
        {
            get
            {
                return GuildInfo != null;
            }
        }
        public void Init(NGuildInfo guild)
        {
            GuildInfo = guild;
            if(guild == null)
            {
                MyMemberInfo = null;
                return;
            }
            foreach(var member in guild.Members)
            {
                if(member.characterId == User.Instance.CurrentCharacterInfo.Id)
                {
                    MyMemberInfo = member;
                    break;
                }
            }
        }

        public void ShowGuild()
        {
            if (HasGuild)
                UIManager.Instance.Show<UIGuild>();
            else
            {
                var win = UIManager.Instance.Show<UIGuildPopNoGuild>();
                win.OnClose += PipNoGuild_OnClose;
            }
        }

        private void PipNoGuild_OnClose(UIWindow sender, UIWindow.WindowResult result)
        {
            if(result == UIWindow.WindowResult.Yes)
            {//创建公会
                UIManager.Instance.Show<UIGuildPopCreate>();
            }
            else if(result == UIWindow.WindowResult.No)
            {//加入公会
                UIManager.Instance.Show<UIGuildList>();
            }
        }
    }
}
