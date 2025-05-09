using Common.Utils;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Models
{
    class Guild
    {
        public int Id { get { return this.Data.Id; } }

        public string Name { get { return this.Data.Name; } }

        public double timestamp;

        public TGuild Data;

        public Guild(TGuild data)
        {
            this.Data = data;
        }

        public bool JoinApply(NGuildApplyInfo apply)
        {
            var oldApply = this.Data.Applies.FirstOrDefault(x => x.CharacterId == apply.characterId);
            if (oldApply != null) return false;

            var dbApply = DBService.Instance.Entities.GuildApplies.Create();

            dbApply.CharacterId = apply.characterId;
            dbApply.GuildId = this.Id;
            dbApply.Name = apply.Name;
            dbApply.Class = apply.Class;
            dbApply.Level = apply.Level;
            dbApply.ApplyTime = DateTime.Now;

            DBService.Instance.Entities.GuildApplies.Add(dbApply);
            this.Data.Applies.Add(dbApply);
            DBService.Instance.Save();
            this.timestamp = TimeUtil.timestamp;
            return true;
        }

        public NGuildInfo GuildInfo(Character from)
        {
            var info = new NGuildInfo()
            {
                Id = this.Id,
                GuildName = this.Name,
                Notice = this.Data.Notice,
                leaderId = this.Data.LeaderID,
                leaderName = this.Data.LeaderName,
                createTime = (long)TimeUtil.GetTimestamp(this.Data.CreateTime),
                memberCount = this.Data.Members.Count,
            };

            if (from != null)
            {
                info.Members.AddRange(GetMemberInfos());
                if (from.Id == this.Data.LeaderID)
                    info.Applies.AddRange(GetApplyInfos());
            }

            return info;
        }

        List<NGuildApplyInfo> GetApplyInfos()
        {
            List<NGuildApplyInfo> applies = new List<NGuildApplyInfo>();
            foreach (var apply in this.Data.Applies)
            {
                if (apply.Result != (int)ApplyResult.None) continue;
                var applyInfo = new NGuildApplyInfo()
                {
                    characterId = apply.CharacterId,
                    GuildId = apply.GuildId,
                    Name = apply.Name,
                    Class = apply.Class,
                    Level = apply.Level,
                    Result = (ApplyResult)apply.Result,
                };
                applies.Add(applyInfo);
            }
            return applies;
        }

        List<NGuildMemberInfo> GetMemberInfos()
        {
            List<NGuildMemberInfo> members = new List<NGuildMemberInfo>();
            foreach (var member in this.Data.Members)
            {
                var memberInfo = new NGuildMemberInfo()
                {
                    Id = member.Id,
                    Title = (GuildTitle)member.Title,
                    characterId = member.CharacterId,
                    joinTime = (long)TimeUtil.GetTimestamp(member.JoinTime),
                    lastTime = (long)TimeUtil.GetTimestamp(member.LastTime),
                };
                var character = CharacterManager.Instance.GetCharacter(member.CharacterId);
                if (character != null)
                {
                    memberInfo.Info = character.GetBsdicInfo();
                    memberInfo.Status = 1;
                    member.Level = character.Info.Level;
                    member.Name = character.Info.Name;
                    member.LastTime = DateTime.Now;
                }
                else
                {
                    memberInfo.Info = this.GetMemberInfo(member);
                    memberInfo.Status = 0;
                }
                members.Add(memberInfo);
            }
            return members;
        }

        NCharacterInfo GetMemberInfo(TGuildMember member)
        {
            return new NCharacterInfo()
            {
                Id = member.CharacterId,
                Name = member.Name,
                Level = member.Level,
                Class = (CharacterClass)member.Class,
            };
        }

        public void PostProcess(Character from, NetMessageResponse message)
        {
            message.Guild = new GuildResponse();
            message.Guild.Result = Result.Success;
            message.Guild.guildInfo = this.GuildInfo(from);
        }

        public bool JoinAppove(NGuildApplyInfo apply)
        {
            var oldApply = this.Data.Applies.FirstOrDefault(x => x.CharacterId == apply.characterId && x.Result == 0);
            if (oldApply == null) return false;

            oldApply.Result = (int)apply.Result;

            if (apply.Result == ApplyResult.Accept)
                this.AddMember(apply.characterId, apply.Name, apply.Class, apply.Level, GuildTitle.None);
            DBService.Instance.Save();
            this.timestamp = TimeUtil.timestamp;
            return true;
        }

        public void AddMember(int characterId, string name, int @class, int level, GuildTitle title)
        {
            DateTime now = DateTime.Now;
            TGuildMember dbmember = new TGuildMember()
            {
                CharacterId = characterId,
                Name = name,
                Class = @class,
                Level = level,
                JoinTime = now,
                LastTime = now,
                Title = (int)title,
            };
            this.Data.Members.Add(dbmember);

            var character = CharacterManager.Instance.GetCharacter(characterId);
            if (character != null)
                character.Data.GuildId = this.Id;
            else
            {
                var dbCharacter = DBService.Instance.Entities.Characters.FirstOrDefault(x => x.ID == characterId);
                dbCharacter.GuildId = this.Id;
            }
            timestamp = TimeUtil.timestamp;
        }

        public void Leave(Character character)
        {
            if (character == null) return;
            var member = this.Data.Members.FirstOrDefault(x => x.CharacterId == character.Id);
            if (member == null) return;
            this.Data.Members.Remove(member);
            var cha = CharacterManager.Instance.GetCharacter(character.Id);
            if (cha != null)
            {
                cha.Data.GuildId = 0;
            }
            else
            {
                var dbCharacter = DBService.Instance.Entities.Characters.FirstOrDefault(x => x.ID == character.Id);
                dbCharacter.GuildId = 0;
            }
            timestamp = TimeUtil.timestamp;
        }

        public void ExecuteAdmin(GuildAdminCommand command, int targetId, int sourceId)
        {
            var target = GetDBMember(targetId);
            var source = GetDBMember(sourceId);

            if (target == null || source == null) return;

            switch (command)
            {
                case GuildAdminCommand.Promote:
                    target.Title = (int)GuildTitle.VicePresident;
                    break;
                case GuildAdminCommand.Depost:
                    target.Title = (int)GuildTitle.None;
                    break;
                case GuildAdminCommand.Kickout:               
                    break;
                case GuildAdminCommand.Transfer:
                    target.Title = (int)GuildTitle.None;
                    source.Title = (int)GuildTitle.None;
                    this.Data.LeaderID = source.CharacterId;
                    this.Data.LeaderName = source.Name;
                    break;
            }

            DBService.Instance.Save();
            timestamp = TimeUtil.timestamp;
        }

        TGuildMember GetDBMember(int characterId)
        {
            foreach (var mem in this.Data.Members)
            {
                if (mem.CharacterId == characterId)
                    return mem;
            }
            return null;
        }


    }
}
