using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;

namespace GameServer.Services
{
    class TeamService : Singleton<TeamService>
    {
        public TeamService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamLeaveRequest>(this.OnTeamLeave);
        }

        void OnTeamLeave(NetConnection<NetSession> sender, TeamLeaveRequest message)
        {
            Character character = sender.Session.Character;
        }

        void OnTeamInviteResponse(NetConnection<NetSession> sender, TeamInviteResponse message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("TeamInviteResponse from{0} {1} to {2}  {3} result:{4}", character.Id, character.Info.Name, message.Request.ToId, message.Request.ToName, message.Result);
            sender.Session.Response.teamInviteRes = message;
            if (message.Result == Result.Success)
            {
                var requester = SessionManager.Instance.GetSession(message.Request.FromId);
                if (requester == null)
                {
                    sender.Session.Response.teamInviteRes.Result = Result.Failed;
                    sender.Session.Response.teamInviteRes.Errormsg = "邀请者不在线";
                }
                else
                {
                    TeamManager.Instance.AddTeamMember(requester.Session.Character, character);
                    requester.Session.Response.teamInviteRes = message;
                    requester.SendResponse();
                }
            }
            sender.SendResponse();
        }

        void OnTeamInviteRequest(NetConnection<NetSession> sender, TeamInviteRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("TeamInviteRequest from{0} {1} to {2}  {3}", message.FromId, message.FromName, message.ToId, message.ToName);

            NetConnection<NetSession> connection = SessionManager.Instance.GetSession(message.ToId);
            if (connection == null)
            {
                sender.Session.Response.teamInviteRes = new TeamInviteResponse();
                sender.Session.Response.teamInviteRes.Result = Result.Failed;
                sender.Session.Response.teamInviteRes.Errormsg = "对方不在线";
                sender.SendResponse();
                return;
            }
            if (connection.Session.Character.Team != null)
            {
                sender.Session.Response.teamInviteRes = new TeamInviteResponse();
                sender.Session.Response.teamInviteRes.Result = Result.Failed;
                sender.Session.Response.teamInviteRes.Errormsg = "对方已经在别的队伍中";
                sender.SendResponse();
                return;
            }

            Log.InfoFormat("TeamInviteRequest from{0} {1} to {2}  {3}", message.FromId, message.FromName, message.ToId, message.ToName);
            if (connection.Session.Response.teamInviteRes == null) connection.Session.Response.teamInviteReq = new TeamInviteRequest();
            connection.Session.Response.teamInviteReq = message;
            connection.SendResponse();
        }

        public void Init()
        {
            TeamManager.Instance.Init();
        }
    }
}
