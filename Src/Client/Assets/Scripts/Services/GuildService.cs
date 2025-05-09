using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Services
{
    class GuildService : Singleton<GuildService>, IDisposable
    {
        public UnityAction OnGuildUpdate;
        public UnityAction<bool> OnCreateGuildResult;
        public UnityAction<List<NGuildInfo>> OnGuildListResult;
        public GuildService()
        {
            MessageDistributer.Instance.Subscribe<GuildCreateResponse>(this.OnGuildCreate);
            MessageDistributer.Instance.Subscribe<GuildListResponse>(this.OnGuildList);
            MessageDistributer.Instance.Subscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer.Instance.Subscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Subscribe<GuildLeaveResponse>(this.OnGuildLeave);
            MessageDistributer.Instance.Subscribe<GuildAdminResponse>(this.OnGuildAdmin);
        }
        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<GuildCreateResponse>(this.OnGuildCreate);
            MessageDistributer.Instance.Unsubscribe<GuildListResponse>(this.OnGuildList);
            MessageDistributer.Instance.Unsubscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer.Instance.Unsubscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer.Instance.Unsubscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Unsubscribe<GuildLeaveResponse>(this.OnGuildLeave);
            MessageDistributer.Instance.Unsubscribe<GuildAdminResponse>(this.OnGuildAdmin);
        }

       
        private void OnGuildCreate(object sender, GuildCreateResponse message)
        {
            Debug.Log("OnGuildCreate");
            if (OnCreateGuildResult != null)
            {
                this.OnCreateGuildResult(message.Result == Result.Success);
            }
            if (message.Result == Result.Success)
            {
                GuildManager.Instance.Init(message.guildInfo);
                MessageBox.Show(string.Format("创建公会{0}成功", message.guildInfo.GuildName), "公会");
            }
            else
                MessageBox.Show(string.Format("创建公会{0}失败", message.guildInfo.GuildName), "公会");
        }
        void OnGuildLeave(object sender, GuildLeaveResponse message)
        {
            if (message.Result == Result.Success)
                MessageBox.Show("离开公会成功", "公会");
            else
                MessageBox.Show("离开公会失败", "公会");
        }

        private void OnGuild(object sender, GuildResponse message)
        {
            Debug.Log("OnGuild");
            GuildManager.Instance.Init(message.guildInfo);
            if (OnGuildUpdate != null)
                this.OnGuildUpdate();
        }

        private void OnGuildJoinRequest(object sender, GuildJoinRequest message)
        {
            var comfirm = MessageBox.Show(string.Format("{0}请求加入公会，是否同意？", message.Apply.Name),
                "公会", MessageBoxType.Confirm, "同意", "拒绝");
            comfirm.OnYes = () =>
            {
                SendGuileJoinResponse(true, message);
            };
            comfirm.OnNo = () =>
            {
                SendGuileJoinResponse(false, message);
            };
        }

        private void OnGuildJoinResponse(object sender, GuildJoinResponse message)
        {
            Debug.Log("OnGuildJoinResponse");
            if (message.Result == Result.Success)
                MessageBox.Show("加入公会成功", "公会");
            else
                MessageBox.Show("加入公会失败", "公会");

        }

        private void OnGuildList(object sender, GuildListResponse message)
        {
           if(OnGuildListResult != null)
                OnGuildListResult(message.Guilds);
        }

        public void Init()
        {

        }

        internal void SendGuildCreate(string guildName, string notice)
        {
            Debug.Log("SendGuildCreate");
            var meg = new NetMessage();
            meg.Request = new NetMessageRequest();
            meg.Request.guildCreate = new GuildCreateRequest();
            meg.Request.guildCreate.GuildName = guildName;
            meg.Request.guildCreate.GuildNotice = notice;
            NetClient.Instance.SendMessage(meg);
        }

        public void SendGuildListRequest()
        {
            Debug.Log("SendGuildListRequest"); 
            var meg = new NetMessage();
            meg.Request = new NetMessageRequest();
            meg.Request.guildList = new GuildListRequest();
            NetClient.Instance.SendMessage(meg);
        }

        public void SendGuileJoinRequest(int id)
        {
            Debug.Log("SendGuileJoinRequest");
            var meg = new NetMessage();
            meg.Request = new NetMessageRequest();
            meg.Request.guildJoinReq = new GuildJoinRequest();
            meg.Request.guildJoinReq.Apply = new NGuildApplyInfo();
            meg.Request.guildJoinReq.Apply.GuildId = id;
            NetClient.Instance.SendMessage(meg);
        }

        public void SendGuileJoinResponse(bool result, GuildJoinRequest request)
        {
            Debug.Log("SendGuileJoinResponse");
            var meg = new NetMessage();
            meg.Request = new NetMessageRequest();
            meg.Request.guildJoinRes = new GuildJoinResponse();
            meg.Request.guildJoinRes.Result = Result.Success;
            meg.Request.guildJoinRes.Apply = request.Apply;
            meg.Request.guildJoinRes.Apply.Result = result ? ApplyResult.Accept : ApplyResult.Reject;
            NetClient.Instance.SendMessage(meg);
        }

        /// <summary>
        /// 发送公会加入审批
        /// </summary>
        /// <param name="accept"></param>
        /// <param name="apply"></param>
        public void SendGuildJoinApply(bool accept, NGuildApplyInfo apply)
        {
            Debug.Log("SendGuildJoinApply");
            var meg = new NetMessage();
            meg.Request = new NetMessageRequest();
            meg.Request.guildJoinRes = new GuildJoinResponse();
            meg.Request.guildJoinRes.Result = Result.Success;
            meg.Request.guildJoinRes.Apply = apply;
            meg.Request.guildJoinRes.Apply.Result = accept ? ApplyResult.Accept : ApplyResult.Reject;
            NetClient.Instance.SendMessage(meg);
        
        }
        /// <summary>
        /// 发送管理信息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="id"></param>
        public void SendAdminCommand(GuildAdminCommand msg, int id)
        {
            Debug.Log("SendAdminCommand");
            var meg = new NetMessage();
            meg.Request = new NetMessageRequest();
            meg.Request.guildAdmin = new GuildAdminRequest();
            meg.Request.guildAdmin.Command = msg;
            meg.Request.guildAdmin.Tatget = id;
            NetClient.Instance.SendMessage(meg);
        }
        void OnGuildAdmin(object sender, GuildAdminResponse message)
        {
            Debug.Log("OnGuildAdmin");
            MessageBox.Show(string.Format("执行操作：{0} 结果：{1} {2}", message.Command, message.Result, message.Errormsg), "公会");

        }

    }
}
