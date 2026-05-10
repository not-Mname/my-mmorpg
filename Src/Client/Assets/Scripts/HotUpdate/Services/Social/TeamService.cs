using GameInterFace;
using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace Services
{
    [Preserve]
    class TeamService : Singleton<TeamService>, IDisposable, IInitializable
    {

        public void Init()
        {

        }
        public TeamService()
        {
            MessageDistributer.Instance.Subscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
            MessageDistributer.Instance.Subscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer.Instance.Subscribe<TeamInfoResponse>(this.OnTeamInfo);
            MessageDistributer.Instance.Subscribe<TeamLeaveResponse>(this.OnTeamLeave);
        }



        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
            MessageDistributer.Instance.Unsubscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer.Instance.Unsubscribe<TeamInfoResponse>(this.OnTeamInfo);
            MessageDistributer.Instance.Unsubscribe<TeamLeaveResponse>(this.OnTeamLeave);
        }

        public void SendTeamInviteRequest(int friendId, string frienName)
        {
            Debug.Log("SendTeamInviteRequest");
            var message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.TeamInviteReq = new TeamInviteRequest();
            message.Request.TeamInviteReq.FromId = User.Instance.CurrentCharacterInfo.Id;
            message.Request.TeamInviteReq.FromName = User.Instance.CurrentCharacterInfo.Name;
            message.Request.TeamInviteReq.ToId = friendId;
            message.Request.TeamInviteReq.ToName = frienName;
            NetClient.Instance.SendMessage(message);
        }
        public void SendTeamInviteResponse(bool accept, TeamInviteRequest request)
        {
            Debug.Log("SendTeamInviteResponse");
            var message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.TeamInviteRes = new TeamInviteResponse();
            message.Response.TeamInviteRes.Request = request;
            message.Response.TeamInviteRes.Result = accept ? Result.Success : Result.Failed;
            message.Response.TeamInviteRes.Errormsg = accept ? "组队成功" : "组队失败";
            NetClient.Instance.SendMessage(message);
        }
        public void SendTeamLeaveRequest(int id)
        {
            Debug.Log("SendFriendRemoveRequest");
            var mes = new NetMessage();
            mes.Request = new NetMessageRequest();
            mes.Request.TeamLeave = new TeamLeaveRequest();
            mes.Request.TeamLeave.CharacterId = User.Instance.CurrentCharacterInfo.Id;
            mes.Request.TeamLeave.TeamId = User.Instance.TeamInfo.Id;
            NetClient.Instance.SendMessage(mes);
        }
        private void OnTeamLeave(object sender, TeamLeaveResponse message)
        {
            if (message.Result == Result.Success)
            {
                TeamManager.Instance.UpdateTeamInfo(null);
                MessageBox.Show("您已退出队伍", "退出队伍");
            }
            else
                MessageBox.Show("退出队伍失败", "退出队伍", MessageBoxType.Error);
        }

        private void OnTeamInfo(object sender, TeamInfoResponse message)
        {
            Debug.Log("OnFriendList");
            TeamManager.Instance.UpdateTeamInfo(message.Team);
        }

        private void OnTeamInviteResponse(object sender, TeamInviteResponse message)
        {
            if (message.Result == Result.Success)
            {
                MessageBox.Show(string.Format("{0}已成功加入的队伍", message.Request.ToName));
            }
            else
            {
                MessageBox.Show(string.Format("邀请失败，原因：{0}", message.Errormsg));
            }
        }

        private void OnTeamInviteRequest(object sender, TeamInviteRequest request)
        {
            var confirm = MessageBox.Show(string.Format("{0}请求加入队伍", request.FromName), "组队请求", MessageBoxType.Confirm,
                "同意", "拒绝");
            confirm.OnYes = () =>
            {
                SendTeamInviteResponse(true, request);
            };
            confirm.OnNo = () =>
            {
                SendTeamInviteResponse(false, request);
            };
        }
    }
}
