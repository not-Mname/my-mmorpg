using Common;
using Common.Utils;
using GameServer.Entities;
using SkillBridge.Message;
using System.Collections.Generic;

namespace GameServer.Models.Logic
{
    class Team
    {
        public int Id;
        public Character Leader;

        public List<Character> Members = new List<Character>();

        public double Timestamp;//代表组队信息更改的时间

        public Team(Character leader)
        {
            this.AddMember(leader);
        }

        public void AddMember(Character member)
        {
            if (this.Members.Count == 0)
            {
                this.Leader = member;
            }
            this.Members.Add(member);
            member.Team = this;
            Timestamp = TimeUtil.timestamp;
        }

        public void Leave(Character member)
        {
            Log.InfoFormat("Team {0} leave member {1}", this.Id, member.Id);
            this.Members.Remove(member);
            if (this.Members.Count > 0)
                Leader = this.Members[0];
            else
                this.Leader = null;
            member.Team = null;
            Timestamp = TimeUtil.timestamp;
        }

        public void PostProcess(NetMessageResponse message)
        {
            if(message.TeamInfo == null)
            {
                message.TeamInfo = new TeamInfoResponse();
                message.TeamInfo.Team = new NTeamInfo();
                message.TeamInfo.Team.Id = this.Id;
                message.TeamInfo.Team.Leader = this.Leader.Id;
                message.TeamInfo.Result = Result.Success;
                foreach (var member in this.Members)
                {
                    message.TeamInfo.Team.Members.Add(member.GetBsdicInfo());
                }
            }
        }
    }
}
