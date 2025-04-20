using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;

namespace GameServer.Services
{
    class GuildService : Singleton<GuildService>
    {
        public GuildService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildCreateRequest>(this.OnGuildCreate);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildListRequest>(this.OnGuildList);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildLeaveRequest>(this.OnGuildLeave);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildAdminRequest>(this.OnGuildAdmin);
        }

      

        public void Init()
        {
            GuildManager.Instance.Init();
        }

        private void OnGuildLeave(NetConnection<NetSession> sender, GuildLeaveRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("GuildLeave: Character {0}", character.Id);

            character.Guild.Leave(character);

            sender.Session.Response.guildLeave = new GuildLeaveResponse();
            sender.Session.Response.guildLeave.Result = Result.Success;

            DBService.Instance.Save();

            sender.SendResponse();
        }

        private void OnGuildJoinResponse(NetConnection<NetSession> sender, GuildJoinResponse message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("GuildJoinResponse: Character {0} {1} ", character.Name, character.Id);

            var guild = GuildManager.Instance.GetGuild(message.Apply.GuildId);
            if(message.Result == Result.Success)
            {
                guild.JoinAppove(message.Apply);
            }

            var requester = SessionManager.Instance.GetSession(message.Apply.characterId);
            if(requester != null)
            {
                requester.Session.Character.Guild = guild;

                requester.Session.Response.guildJoinRes = message;
                requester.Session.Response.guildJoinRes.Result = message.Result;
                requester.Session.Response.guildJoinRes.Errormsg = "加入公会成功";
                requester.SendResponse();
            }
        }

        private void OnGuildList(NetConnection<NetSession> sender, GuildListRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("GuildList: Character {0} {1} ", character.Name, character.Id);
            sender.Session.Response.guildList = new GuildListResponse();
            sender.Session.Response.guildList.Guilds.AddRange(GuildManager.Instance.GetGuildsInfo());
            sender.SendResponse();
        }

        private void OnGuildJoinRequest(NetConnection<NetSession> sender, GuildJoinRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("GuildJoinRequest: GuildName:{0}  Character {1} {2} " , message.Apply.GuildId, character.Name, character.Id);
            var guild = GuildManager.Instance.GetGuild(message.Apply.GuildId);
            if (guild == null)
            {
                sender.Session.Response.guildJoinRes = new GuildJoinResponse();
                sender.Session.Response.guildJoinRes.Result = Result.Failed;
                sender.Session.Response.guildJoinRes.Errormsg = "公会不存在";
                sender.SendResponse();
                return;
            }

            //客户端没填信息，这里补充一下
            message.Apply.characterId = character.Data.ID;
            message.Apply.Name = character.Data.Name;
            message.Apply.Level = character.Data.Level;
            message.Apply.Class = character.Data.Class;

            if(guild.JoinApply(message.Apply))
            {
                var leader = SessionManager.Instance.GetSession(guild.Data.LeaderID);
                if(leader != null)
                {
                    leader.Session.Response.guildJoinReq = new GuildJoinRequest();
                    leader.Session.Response.guildJoinReq.Apply = message.Apply;
                    leader.SendResponse();
                }
            }
            else
            {
                sender.Session.Response.guildJoinRes = new GuildJoinResponse();
                sender.Session.Response.guildJoinRes.Result = Result.Failed;
                sender.Session.Response.guildJoinRes.Errormsg = "已经在公会中";
                sender.SendResponse();
            }
        }

        void OnGuildCreate(NetConnection<NetSession> sender, GuildCreateRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("GuildCreate: GuildName:{0}  Character {1} {2} " , message.GuildName, character.Name, character.Id);
            sender.Session.Response.guildCreate = new GuildCreateResponse();

            if(character.Guild != null)
            {
                sender.Session.Response.guildCreate.Result = Result.Failed;
                sender.Session.Response.guildCreate.Errormsg = "已经有公会了";
                sender.SendResponse();
                return;
            }
            if(GuildManager.Instance.CheckNameExisted(message.GuildName))
            {
                sender.Session.Response.guildCreate.Result = Result.Failed;
                sender.Session.Response.guildCreate.Errormsg = "公会名已经存在";
                sender.SendResponse();
                return;
            }

            GuildManager.Instance.CreateGuild(message.GuildName, message.GuildNotice, character);
            sender.Session.Response.guildCreate.Result = Result.Success;
            sender.Session.Response.guildCreate.guildInfo = character.Guild.GuildInfo(character);
            sender.SendResponse();
        }
        void OnGuildAdmin(NetConnection<NetSession> sender, GuildAdminRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("GuildAdmin: {0} GCharacter {1}", message.Command,  character.Id);
            sender.Session.Response.guildAdmin = new GuildAdminResponse();

            if(character.Guild == null)
            {
                sender.Session.Response.guildAdmin.Result = Result.Failed;
                sender.Session.Response.guildAdmin.Errormsg = "没有公会，你别乱来";
                sender.SendResponse();
                return;
            }

            character.Guild.ExecuteAdmin(message.Command, message.Tatget, character.Id);

            var target = SessionManager.Instance.GetSession(message.Tatget);
            if (target != null)
            {
                target.Session.Response.guildAdmin = new GuildAdminResponse();
                target.Session.Response.guildAdmin.Result = Result.Success;
                target.Session.Response.guildAdmin.Command = message;
                target.SendResponse();
            }

            sender.Session.Response.guildAdmin.Result = Result.Success;
            sender.Session.Response.guildAdmin.Command = message;
            sender.SendResponse();
        }
    }
}
