using Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UI.MainUI;

namespace Managers
{
    
    class TeamManager : Singleton<TeamManager>
    {
        public void Init(NGuildInfo guild)
        {

        }
        public void UpdateTeamInfo(NTeamInfo team)
        {
            User.Instance.TeamInfo = team;
            ShowTeamUI(team != null);
        }

        public void ShowTeamUI(bool show)
        {
            if(UIMain.Instance!= null)
            {
                UIMain.Instance.ShowTeamUI(show);
            }
        }

        internal void ShowGuild()
        {
            throw new NotImplementedException();
        }
    }
}
