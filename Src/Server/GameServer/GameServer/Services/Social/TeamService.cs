using Common;
using GameInterFace;
using GameServer.Entities;
using GameServer.Managers.Net;
using GameServer.Managers.Social;
using Network;
using SkillBridge.Message;
using System;

namespace GameServer.Services.Social
{
    class TeamService : Singleton<TeamService>, IDisposable, IInitializable
    {
        public void Init()
        {
            Log.Info("TeamService Init...");
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamLeaveRequest>(this.OnTeamLeave);
            TeamManager.Instance.Init();
        }

        public void Dispose()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<TeamLeaveRequest>(this.OnTeamLeave);
        }

        void OnTeamLeave(NetConnection<NetSession> sender, TeamLeaveRequest message)
        {
            Character character = sender.Session.Character;
        }

        void OnTeamInviteResponse(NetConnection<NetSession> sender, TeamInviteResponse message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("TeamInviteResponse from{0} {1} to {2}  {3} result:{4}", character.Id, character.Info.Name, message.Request.ToId, message.Request.ToName, message.Result);
            if (message.Result == Result.Success)
            {
                var requester = SessionManager.Instance.GetSession(message.Request.FromId);
                if (requester == null)
                {
                    message.Result = Result.Failed;
                    message.Errormsg = "邀请者不在线";
                }
                else
                {
                    TeamManager.Instance.AddTeamMember(requester.Session.Character, character);
                    requester.Session.AddResponse(new NetMessageResponse { TeamInviteRes = message });
                    requester.SendResponse();
                }
            }
            sender.Session.AddResponse(new NetMessageResponse { TeamInviteRes = message });
            sender.SendResponse();
        }

        void OnTeamInviteRequest(NetConnection<NetSession> sender, TeamInviteRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("TeamInviteRequest from{0} {1} to {2}  {3}", message.FromId, message.FromName, message.ToId, message.ToName);

            NetConnection<NetSession> connection = SessionManager.Instance.GetSession(message.ToId);
            if (connection == null)
            {
                var teamInviteRes = new TeamInviteResponse();
                teamInviteRes.Result = Result.Failed;
                teamInviteRes.Errormsg = "对方不在线";
                sender.Session.AddResponse(new NetMessageResponse { TeamInviteRes = teamInviteRes });
                sender.SendResponse();
                return;
            }
            if (connection.Session.Character.Team != null)
            {
                var teamInviteRes = new TeamInviteResponse();
                teamInviteRes.Result = Result.Failed;
                teamInviteRes.Errormsg = "对方已经在别的队伍中";
                sender.Session.AddResponse(new NetMessageResponse { TeamInviteRes = teamInviteRes });
                sender.SendResponse();
                return;
            }

            Log.InfoFormat("TeamInviteRequest from{0} {1} to {2}  {3}", message.FromId, message.FromName, message.ToId, message.ToName);
            connection.Session.AddResponse(new NetMessageResponse { TeamInviteReq = message });
            connection.SendResponse();
        }

    }
}
