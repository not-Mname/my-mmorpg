using Common;
using GameInterFace;
using GameServer.Entities;
using GameServer.Managers.Net;
using GameServer.Managers.Social;
using GameServer.Services.Data;
using Network;
using SkillBridge.Message;
using System;

namespace GameServer.Services.Social
{
    class GuildService : Singleton<GuildService>, IDisposable, IInitializable
    {
        public void Init()
        {
            Log.Info("GuildService Init...");
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildCreateRequest>(this.OnGuildCreate);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildListRequest>(this.OnGuildList);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildLeaveRequest>(this.OnGuildLeave);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildAdminRequest>(this.OnGuildAdmin);
            GuildManager.Instance.Init();
        }

        public void Dispose()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<GuildCreateRequest>(this.OnGuildCreate);
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<GuildListRequest>(this.OnGuildList);
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<GuildLeaveRequest>(this.OnGuildLeave);
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<GuildAdminRequest>(this.OnGuildAdmin);
        }

        private void OnGuildLeave(NetConnection<NetSession> sender, GuildLeaveRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("GuildLeave: Character {0}", character.Id);

            character.Guild.Leave(character);

            sender.Session.Response.GuildLeave = new GuildLeaveResponse();
            sender.Session.Response.GuildLeave.Result = Result.Success;

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

            var requester = SessionManager.Instance.GetSession(message.Apply.CharacterId);
            if(requester != null)
            {
                requester.Session.Character.Guild = guild;

                requester.Session.Response.GuildJoinRes = message;
                requester.Session.Response.GuildJoinRes.Result = message.Result;
                requester.Session.Response.GuildJoinRes.Errormsg = "加入公会成功";
                requester.SendResponse();
            }
        }

        private void OnGuildList(NetConnection<NetSession> sender, GuildListRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("GuildList: Character {0} {1} ", character.Name, character.Id);
            sender.Session.Response.GuildList = new GuildListResponse();
            sender.Session.Response.GuildList.Guilds.AddRange(GuildManager.Instance.GetGuildsInfo());
            sender.SendResponse();
        }

        private void OnGuildJoinRequest(NetConnection<NetSession> sender, GuildJoinRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("GuildJoinRequest: GuildName:{0}  Character {1} {2} " , message.Apply.GuildId, character.Name, character.Id);
            var guild = GuildManager.Instance.GetGuild(message.Apply.GuildId);
            if (guild == null)
            {
                sender.Session.Response.GuildJoinRes = new GuildJoinResponse();
                sender.Session.Response.GuildJoinRes.Result = Result.Failed;
                sender.Session.Response.GuildJoinRes.Errormsg = "公会不存在";
                sender.SendResponse();
                return;
            }

            //客户端没填信息，这里补充一下
            message.Apply.CharacterId = character.Data.ID;
            message.Apply.Name = character.Data.Name;
            message.Apply.Level = character.Data.Level;
            message.Apply.Class = character.Data.Class;

            if(guild.JoinApply(message.Apply))
            {
                var leader = SessionManager.Instance.GetSession(guild.Data.LeaderID);
                if(leader != null)
                {
                    leader.Session.Response.GuildJoinReq = new GuildJoinRequest();
                    leader.Session.Response.GuildJoinReq.Apply = message.Apply;
                    leader.SendResponse();
                }
            }
            else
            {
                sender.Session.Response.GuildJoinRes = new GuildJoinResponse();
                sender.Session.Response.GuildJoinRes.Result = Result.Failed;
                sender.Session.Response.GuildJoinRes.Errormsg = "已经在公会中";
                sender.SendResponse();
            }
        }

        void OnGuildCreate(NetConnection<NetSession> sender, GuildCreateRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("GuildCreate: GuildName:{0}  Character {1} {2} " , message.GuildName, character.Name, character.Id);
            sender.Session.Response.GuildCreate = new GuildCreateResponse();

            if(character.Guild != null)
            {
                sender.Session.Response.GuildCreate.Result = Result.Failed;
                sender.Session.Response.GuildCreate.Errormsg = "已经有公会了";
                sender.SendResponse();
                return;
            }
            if(GuildManager.Instance.CheckNameExisted(message.GuildName))
            {
                sender.Session.Response.GuildCreate.Result = Result.Failed;
                sender.Session.Response.GuildCreate.Errormsg = "公会名已经存在";
                sender.SendResponse();
                return;
            }

            GuildManager.Instance.CreateGuild(message.GuildName, message.GuildNotice, character);
            sender.Session.Response.GuildCreate.Result = Result.Success;
            sender.Session.Response.GuildCreate.GuildInfo = character.Guild.GuildInfo(character);
            sender.SendResponse();
        }
        void OnGuildAdmin(NetConnection<NetSession> sender, GuildAdminRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("GuildAdmin: {0} GCharacter {1}", message.Command,  character.Id);
            sender.Session.Response.GuildAdmin = new GuildAdminResponse();

            if(character.Guild == null)
            {
                sender.Session.Response.GuildAdmin.Result = Result.Failed;
                sender.Session.Response.GuildAdmin.Errormsg = "没有公会，你别乱来";
                sender.SendResponse();
                return;
            }

            character.Guild.ExecuteAdmin(message.Command, message.Tatget, character.Id);

            var target = SessionManager.Instance.GetSession(message.Tatget);
            if (target != null)
            {
                target.Session.Response.GuildAdmin = new GuildAdminResponse();
                target.Session.Response.GuildAdmin.Result = Result.Success;
                target.Session.Response.GuildAdmin.Command = message;
                target.SendResponse();
            }

            sender.Session.Response.GuildAdmin.Result = Result.Success;
            sender.Session.Response.GuildAdmin.Command = message;
            sender.SendResponse();
        }
    }
}
