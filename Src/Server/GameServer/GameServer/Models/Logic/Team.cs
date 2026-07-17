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

        public void PostProcess(NetMessage message)
        {
            if (!message.HasResponse(NetMessageResponse.PayloadOneofCase.TeamInfo))
            {
                var teamInfo = new TeamInfoResponse();
                teamInfo.Team = new NTeamInfo();
                teamInfo.Team.Id = this.Id;
                teamInfo.Team.Leader = this.Leader.Id;
                teamInfo.Result = Result.Success;
                foreach (var member in this.Members)
                {
                    teamInfo.Team.Members.Add(member.GetBasicInfo());
                }
                message.Responses.Add(new NetMessageResponse { TeamInfo = teamInfo });
            }
        }
    }
}
