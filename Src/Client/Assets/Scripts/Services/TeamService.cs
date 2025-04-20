using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using UnityEngine;

namespace Services
{
    class TeamService : Singleton<TeamService>, IDisposable
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
            message.Request.teamInviteReq = new TeamInviteRequest();
            message.Request.teamInviteReq.FromId = User.Instance.CurrentCharacter.Id;
            message.Request.teamInviteReq.FromName = User.Instance.CurrentCharacter.Name;
            message.Request.teamInviteReq.ToId = friendId;
            message.Request.teamInviteReq.ToName = frienName;
            NetClient.Instance.SendMessage(message);
        }
        public void SendTeamInviteResponse(bool accept, TeamInviteRequest request)
        {
            Debug.Log("SendTeamInviteResponse");
            var message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.teamInviteRes = new TeamInviteResponse();
            message.Response.teamInviteRes.Request = request;
            message.Response.teamInviteRes.Result = accept ? Result.Success : Result.Failed;
            message.Response.teamInviteRes.Errormsg = accept ? "组队成功" : "组队失败";
            NetClient.Instance.SendMessage(message);
        }
        public void SendTeamLeaveRequest(int id)
        {
            Debug.Log("SendFriendRemoveRequest");
            var mes = new NetMessage();
            mes.Request = new NetMessageRequest();
            mes.Request.teamLeave = new TeamLeaveRequest();
            mes.Request.teamLeave.characterId = User.Instance.CurrentCharacter.Id;
            mes.Request.teamLeave.TeamId = User.Instance.TeamInfo.Id;
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
