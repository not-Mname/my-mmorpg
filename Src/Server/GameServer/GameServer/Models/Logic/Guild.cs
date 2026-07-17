using Common.Utils;
using GameServer.Entities;
using GameServer.Managers.Entities;
using GameServer.Models.Data;
using GameServer.Services.Data;
using Microsoft.EntityFrameworkCore;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Models.Logic
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
            var oldApply = this.Data.Applies.FirstOrDefault(x => x.CharacterId == apply.CharacterId);
            if (oldApply != null) return false;

            using var scope = DBService.Instance.BeginScope();
            var dbGuild = DBService.Instance.Entities.Guilds
                .Include(g => g.Applies)
                .First(g => g.Id == this.Id);
            var dbApply = new TGuildApply()
            {
                CharacterId = apply.CharacterId,
                Name = apply.Name,
                Class = apply.Class,
                Level = apply.Level,
                ApplyTime = DateTime.Now,
            };

            dbGuild.Applies.Add(dbApply);
            this.Data.Applies.Add(new TGuildApply()
            {
                CharacterId = apply.CharacterId,
                TGuildId = this.Id,
                Name = apply.Name,
                Class = apply.Class,
                Level = apply.Level,
                ApplyTime = dbApply.ApplyTime,
            });
            this.timestamp = TimeUtil.timestamp;
            return true;
        }

        /// <summary>
        /// 获取公会信息，包含成员列表和申请列表（如果是会长）
        /// </summary>
        /// <param name="from">如果只是查看公会基本信息，传入null，如果要查看申请列表，传入会长角色，查看成员传入会员</param>
        /// <returns></returns>
        public NGuildInfo GuildInfo(Character from)
        {
            var info = new NGuildInfo()
            {
                Id = this.Id,
                GuildName = this.Name,
                Notice = this.Data.Notice,
                LeaderId = this.Data.LeaderID,
                LeaderName = this.Data.LeaderName,
                CreateTime = (long)TimeUtil.GetTimestamp(this.Data.CreateTime),
                MemberCount = this.Data.Members.Count,
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
                    CharacterId = apply.CharacterId,
                    GuildId = apply.TGuildId,
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
                    CharacterId = member.CharacterId,
                    JoinTime = (long)TimeUtil.GetTimestamp(member.JoinTime),
                    LastTime = (long)TimeUtil.GetTimestamp(member.LastTime),
                };
                var character = CharacterManager.Instance.GetCharacter(member.CharacterId);
                if (character != null)
                {
                    memberInfo.Info = character.GetBasicInfo();
                    memberInfo.Status = 1;
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

        /// <summary>
        /// 同步在线成员的实时数据到内存缓存，供离线展示使用
        /// </summary>
        void SyncOnlineMembers()
        {
            foreach (var member in this.Data.Members)
            {
                var character = CharacterManager.Instance.GetCharacter(member.CharacterId);
                if (character != null)
                {
                    member.Level = character.Info.Level;
                    member.Name = character.Info.Name;
                    member.LastTime = DateTime.Now;
                }
            }
        }

        public void PostProcess(Character from, NetMessage message)
        {
            SyncOnlineMembers();
            var guildRes = new GuildResponse();
            guildRes.Result = Result.Success;
            guildRes.GuildInfo = this.GuildInfo(from);
            message.Responses.Add(new NetMessageResponse { Guild = guildRes });
        }

        public bool JoinAppove(NGuildApplyInfo apply)
        {
            var oldApply = this.Data.Applies.FirstOrDefault(x => x.CharacterId == apply.CharacterId && x.Result == 0);
            if (oldApply == null) return false;

            using var scope = DBService.Instance.BeginScope();
            var dbGuild = DBService.Instance.Entities.Guilds
                .Include(g => g.Applies)
                .First(g => g.Id == this.Id);
            var dbApply = dbGuild.Applies.FirstOrDefault(x => x.CharacterId == apply.CharacterId && x.Result == 0);
            if (dbApply != null) dbApply.Result = (int)apply.Result;
            oldApply.Result = (int)apply.Result;

            if (apply.Result == ApplyResult.Accept)
                this.AddMember(apply.CharacterId, apply.Name, apply.Class, apply.Level, GuildTitle.None);
            this.timestamp = TimeUtil.timestamp;
            return true;
        }

        public void AddMember(int characterId, string name, int @class, int level, GuildTitle title)
        {
            DateTime now = DateTime.Now;

            // 写入公会成员列表
            TGuildMember dbmember;
            using (var scope = DBService.Instance.BeginScope())
            {
                var dbGuild = DBService.Instance.Entities.Guilds
                    .Include(g => g.Members)
                    .First(g => g.Id == this.Id);
                dbmember = new TGuildMember()
                {
                    CharacterId = characterId,
                    Name = name,
                    Class = @class,
                    Level = level,
                    JoinTime = now,
                    LastTime = now,
                    Title = (int)title,
                };
                dbGuild.Members.Add(dbmember);
            }
            this.Data.Members.Add(new TGuildMember()
            {
                Id = dbmember.Id,
                CharacterId = dbmember.CharacterId,
                Name = dbmember.Name,
                Class = dbmember.Class,
                Level = dbmember.Level,
                JoinTime = dbmember.JoinTime,
                LastTime = dbmember.LastTime,
                Title = dbmember.Title,
            });

            // 更新角色的 GuildId
            var character = CharacterManager.Instance.GetCharacter(characterId);
            using (var scope = DBService.Instance.BeginScope())
            {
                var dbChar = DBService.Instance.Entities.Characters.Find(characterId);
                if (dbChar != null) dbChar.GuildId = this.Id;
            }
            if (character != null)
                character.Data.GuildId = this.Id;

            timestamp = TimeUtil.timestamp;
        }

        public void Leave(Character character)
        {
            if (character == null) return;
            var member = this.Data.Members.FirstOrDefault(x => x.CharacterId == character.Id);
            if (member == null) return;

            using (var scope = DBService.Instance.BeginScope())
            {
                var dbGuild = DBService.Instance.Entities.Guilds
                    .Include(g => g.Members)
                    .First(g => g.Id == this.Id);
                var dbMember = dbGuild.Members.FirstOrDefault(x => x.CharacterId == character.Id);
                if (dbMember != null) dbGuild.Members.Remove(dbMember);
            }
            this.Data.Members.Remove(member);

            using (var scope = DBService.Instance.BeginScope())
            {
                var dbChar = DBService.Instance.Entities.Characters.Find(character.Id);
                if (dbChar != null) dbChar.GuildId = 0;
            }
            character.Data.GuildId = 0;

            timestamp = TimeUtil.timestamp;
        }

        public void ExecuteAdmin(GuildAdminCommand command, int targetId, int sourceId)
        {
            TGuildMember target = GetDBMember(targetId);
            TGuildMember source = GetDBMember(sourceId);

            if (target == null || source == null) return;

            using var _ = DBService.Instance.BeginScope();
            TGuild dbGuild = DBService.Instance.Entities.Guilds
                .Include(g => g.Members)
                .First(g => g.Id == this.Id);
            TGuildMember dbTarget = dbGuild.Members.FirstOrDefault(m => m.CharacterId == targetId);
            TGuildMember dbSource = dbGuild.Members.FirstOrDefault(m => m.CharacterId == sourceId);

            switch (command)
            {
                case GuildAdminCommand.Promote:
                    if (dbTarget != null) dbTarget.Title = (int)GuildTitle.VicePresident;
                    if (target != null) target.Title = (int)GuildTitle.VicePresident;
                    break;
                case GuildAdminCommand.Depost:
                    if (dbTarget != null) dbTarget.Title = (int)GuildTitle.None;
                    if (target != null) target.Title = (int)GuildTitle.None;
                    break;
                case GuildAdminCommand.Kickout:
                    break;
                case GuildAdminCommand.Transfer:
                    if (dbTarget != null) dbTarget.Title = (int)GuildTitle.None;
                    if (dbSource != null) dbSource.Title = (int)GuildTitle.None;
                    dbGuild.LeaderID = source.CharacterId;
                    dbGuild.LeaderName = source.Name;
                    if (target != null) target.Title = (int)GuildTitle.None;
                    if (source != null) source.Title = (int)GuildTitle.None;
                    this.Data.LeaderID = source.CharacterId;
                    this.Data.LeaderName = source.Name;
                    break;
            }

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
