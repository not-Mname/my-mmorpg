using Common;
using Common.Utils;
using GameServer.Entities;
using GameServer.Models.Logic;
using GameServer.Services.Data;
using Microsoft.EntityFrameworkCore;
using SkillBridge.Message;
using System;
using System.Collections.Generic;

namespace GameServer.Managers.Social
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
            var dbGuild = new Models.Data.TGuild() {
               Name = guildName,
                Notice = guildNotice,
                CreateTime = now,
                LeaderID = leader.Id,
                LeaderName = leader.Name,
            };

            using (var scope = DBService.Instance.BeginScope())
            {
                DBService.Instance.Entities.Guilds.Add(dbGuild);
            }

            Guild guild = new Guild(dbGuild);
            guild.AddMember(leader.Id, leader.Name, leader.Data.Class, leader.Data.Level, GuildTitle.President);
            leader.Guild = guild;
            leader.Data.GuildId = guild.Id;
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
            Log.Info("GuildManager Init...");
            this.Guilds.Clear();
            foreach (var guild in DBService.Instance.Entities.Guilds.Include(g => g.Members).Include(g => g.Applies).AsNoTracking())
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
