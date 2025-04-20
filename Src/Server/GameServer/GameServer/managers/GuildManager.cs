using Common;
using Common.Utils;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;

namespace GameServer.Managers
{
    class GuildManager : Singleton<GuildManager>
    {
        public Dictionary<int, Guild> Guilds = new Dictionary<int, Guild>();
        private HashSet<string> GuildNames = new HashSet<string>();
        public bool CheckNameExisted(string guildName)
        {
            return this.GuildNames.Contains(guildName);
        }

        public bool CreateGuild(string guildName, string guildNotice, Character leader)
        {
            DateTime now = DateTime.Now;
            var dbGuild = DBService.Instance.Entities.Guilds.Create();
            dbGuild.Name = guildName;
            dbGuild.Notice = guildNotice;
            dbGuild.CreateTime = now;
            dbGuild.LeaderID = leader.Id;
            dbGuild.LeaderName = leader.Name;
            DBService.Instance.Entities.Guilds.Add(dbGuild);

            Guild guild = new Guild(dbGuild);
            guild.AddMember(leader.Id, leader.Name, leader.Data.Class, leader.Data.Level, GuildTitle.President);
            leader.Guild = guild;
            DBService.Instance.Save();
            leader.Data.GuildId = guild.Id;
            DBService.Instance.Save();
            this.AddGuild(guild);
            return true;
        
        }

        public Guild GetGuild(int guildId)
        {
            if(guildId == 0) return null;
            Guild guild = null;
            this.Guilds.TryGetValue(guildId, out guild);
            return guild;
        }

        public List<NGuildInfo> GetGuildsInfo()
        {
            List<NGuildInfo> guildsInfo = new List<NGuildInfo>();
            foreach(var kv in this.Guilds)
            {
                guildsInfo.Add(kv.Value.GuildInfo(null));
            }
            return guildsInfo;
        }

        public void Init()
        {
            this.Guilds.Clear();
            foreach (var guild in DBService.Instance.Entities.Guilds)
            {
                this.AddGuild(new Guild(guild));
            }
        }

        private void AddGuild(Guild guild)
        {
            this.Guilds.Add(guild.Id, guild);
            this.GuildNames.Add(guild.Name);
            guild.timestamp = TimeUtil.timestamp;
        }
    }
}
