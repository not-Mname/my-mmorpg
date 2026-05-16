using Common;
using GameServer.Entities;
using GameServer.Models.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers.Social
{
    class TeamManager : Singleton<TeamManager>
    {
        public List<Team> teams = new List<Team>();
        public Dictionary<int, Team> characterTeams = new Dictionary<int, Team>();
        public void AddTeamMember(Character leader, Character member)
        {
            if (leader.Team == null)
            {
                leader.Team = CreateTeam(leader);
            }
            leader.Team.AddMember(member);
        }
        public Team GetTeamByCharacterId(int teamId)
        {
            Team team;
            characterTeams.TryGetValue(teamId, out team);
            return team;
        }
        Team CreateTeam(Character leader)
        {
            Team team;
            for (int i = 0; i < teams.Count; i++)
            {
                team = teams[i];
                if(team.Members.Count == 0)
                {
                    team.AddMember(leader);
                    return team;
                }
            }
            team = new Team(leader);
            teams.Add(team);
            team.Id = teams.Count;
            return team;
        }

        public void Init()
        {
            Log.Info("TeamManager Init...");
        }
    }
}
