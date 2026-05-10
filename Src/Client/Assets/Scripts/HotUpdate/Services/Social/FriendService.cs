using GameInterFace;
using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;

namespace Services
{
    [Preserve]
    class FriendService : Singleton<FriendService>, IDisposable, IInitializable
    {
        public UnityAction OnFriendUpdata;

        public Action<bool> OnCreateGuildResult { get; internal set; }

        public FriendService()
        {
            MessageDistributer.Instance.Subscribe<FriendAddRequest>(this.OnFriendAddRequest);
            MessageDistributer.Instance.Subscribe<FriendAddResponse>(this.OnFriendAddResponse);
            MessageDistributer.Instance.Subscribe<FriendRemoveResponse>(this.OnFriendRemove);
            MessageDistributer.Instance.Subscribe<FriendListResponse>(this.OnFriendList);

        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<FriendAddRequest>(this.OnFriendAddRequest);
            MessageDistributer.Instance.Unsubscribe<FriendAddResponse>(this.OnFriendAddResponse);
            MessageDistributer.Instance.Unsubscribe<FriendRemoveResponse>(this.OnFriendRemove);
            MessageDistributer.Instance.Unsubscribe<FriendListResponse>(this.OnFriendList);
        }
        public void SendAddFriendRequest(string name, int friendId)
        {
            Debug.Log("SendFriendAddRequest");
            var mes = new NetMessage();
            mes.Request = new NetMessageRequest();
            mes.Request.FriendAddReq = new FriendAddRequest();
            mes.Request.FriendAddReq.FromId = User.Instance.CurrentCharacterInfo.Id;
            mes.Request.FriendAddReq.FromName = User.Instance.CurrentCharacterInfo.Name;
            mes.Request.FriendAddReq.ToId = friendId;
            mes.Request.FriendAddReq.ToName = name;
            NetClient.Instance.SendMessage(mes);
        }

        public void SendAddFriendResponse(bool accepted, FriendAddRequest request)
        {
            Debug.Log("SendFriendAddResponse");
            var mes = new NetMessage();
            mes.Response = new NetMessageResponse();
            mes.Response.FriendAddRes = new FriendAddResponse();
            mes.Response.FriendAddRes.Result = accepted ? Result.Success : Result.Failed;
            mes.Response.FriendAddRes.Errormsg = accepted ? "对方同意" : "对方拒绝";
            mes.Response.FriendAddRes.Request = request;
            NetClient.Instance.SendMessage(mes);
        }
        
        public void SendFriendRemoveRequest(int id, int friendId)
        {
            var mes = new NetMessage();
            mes.Request = new NetMessageRequest();
            mes.Request.FriendRemove = new FriendRemoveRequest();
            mes.Request.FriendRemove.FriendId = friendId;
            mes.Request.FriendRemove.Id = id;
            NetClient.Instance.SendMessage(mes);
        }
        void OnFriendList(object sender, FriendListResponse message)
        {
            Debug.Log("OnFriendList");
            FriendManager.Instance.allFriends = message.Friends.ToList();
            if (this.OnFriendUpdata != null)
                OnFriendUpdata();
        }

        void OnFriendRemove(object sender, FriendRemoveResponse message)
        {
            if(message.Result == Result.Success)
                MessageBox.Show("成功删除","删除好友");
            else
                MessageBox.Show("删除失败","删除好友",MessageBoxType.Error);
        }

        void OnFriendAddRequest(object sender, FriendAddRequest request)
        {
            var confirm = MessageBox.Show(string.Format("{0}请求添加你为好友", request.FromName), "好友请求",MessageBoxType.Confirm,
                "同意", "拒绝");
            confirm.OnYes = () =>
            {
                SendAddFriendResponse(true, request);
            };
            confirm.OnNo = () =>
            {
                SendAddFriendResponse(false, request);
            };
        }

        void OnFriendAddResponse(object sender, FriendAddResponse message)
        {
            if(message.Result == Result.Success)
            {
                MessageBox.Show(string.Format("你已成功添加{0}为好友", message.Request.ToName));
            }
            else
            {
                MessageBox.Show(string.Format("添加好友失败，原因：{0}", message.Errormsg));
            }
        }

        public void Init()
        {
            
        }

        internal void SendGuildCreate(string text1, string text2)
        {
            throw new NotImplementedException();
        }
    }
}
