using Common;
using GameInterFace;
using GameServer.Entities;
using GameServer.Managers.Entities;
using GameServer.Managers.Net;
using GameServer.Services.Data;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Services.Social
{
    class FriendService : Singleton<FriendService>, IInitializable
    {
        public FriendService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendAddRequest>(OnFriendAddRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendAddResponse>(OnFriendAddResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendRemoveRequest>(OnFriendRemove);
        }
        public void Init()
        {
            Log.Info("FriendService Init...");
        }
        private void OnFriendRemove(NetConnection<NetSession> sender, FriendRemoveRequest message)
        {
            Character cha = sender.Session.Character;
            Log.InfoFormat("OnFriendRemove: character {0} remove Friend {1}", cha.Id, message.FriendId);
            sender.Session.Response.FriendRemove = new FriendRemoveResponse();
            sender.Session.Response.FriendRemove.Result = Result.Success;

            if (cha.FriendManager.RemoveFriendById(message.FriendId))
            {
                sender.Session.Response.FriendRemove.Result = Result.Success;
                var Friend = SessionManager.Instance.GetSession(message.FriendId);
                if (Friend != null)
                {//在线
                    Friend.Session.Character.FriendManager.RemoveFriendById(cha.Id);
                    Friend.Session.Character.FriendManager.RemoveFriendById(message.FriendId);
                }
                else
                {//不在线
                    RemoveFriend(cha.Id, message.FriendId);
                }
            }
            else
            {
                sender.Session.Response.FriendRemove.Result = Result.Failed;
                sender.Session.Response.FriendRemove.Errormsg = "好友不存在";
            }
            sender.SendResponse();
        }

        private void RemoveFriend(int characterId, int FriendId)
        {
            using var scope = DBService.Instance.BeginScope();
            var characterRemoveItem = DBService.Instance.Entities.CharacterFriends.FirstOrDefault
                (v => v.CharacterID == characterId && v.FriendID == FriendId);
            var FriendeRmoveItem = DBService.Instance.Entities.CharacterFriends.FirstOrDefault
               (v => v.CharacterID == FriendId && v.FriendID == characterId);
            if (characterRemoveItem != null)
            {
                DBService.Instance.Entities.CharacterFriends.Remove(characterRemoveItem);
            }
            if (FriendeRmoveItem != null)
            {
                DBService.Instance.Entities.CharacterFriends.Remove(FriendeRmoveItem);
            }
        }

        private void OnFriendAddResponse(NetConnection<NetSession> sender, FriendAddResponse message)
        {
            Character cha = sender.Session.Character;
            Log.InfoFormat("OnFriendAddResponse：character {0} Result{1} FromId{2} ToId{3}", cha.Id, message.Result, message.Request.FromId, message.Request.ToId);
            sender.Session.Response.FriendAddRes = message;
            if (message.Result == Result.Success)
            {
                NetConnection<NetSession> Friend = SessionManager.Instance.GetSession(message.Request.FromId);
                if (Friend == null)
                {
                    sender.Session.Response.FriendAddRes.Errormsg = "请求者已下线";
                    sender.Session.Response.FriendAddRes.Result = Result.Failed;
                }
                else
                {
                    Friend.Session.Character.FriendManager.AddFriend(sender.Session.Character);
                    cha.FriendManager.AddFriend(Friend.Session.Character);
                    Friend.Session.Response.FriendAddRes = message;
                    Friend.Session.Response.FriendAddRes.Result = Result.Success;
                    Friend.Session.Response.FriendAddRes.Errormsg = "添加好友成功";
                    Friend.SendResponse();
                }
            }

            sender.SendResponse();
        }
        /// <summary>
        /// 处理好友添加请求，本质是转发A客户端的请求到B客户端
        /// </summary>
        private void OnFriendAddRequest(NetConnection<NetSession> sender, FriendAddRequest message)
        {
            Character cha = sender.Session.Character;
            Log.InfoFormat("OnFriendAddRequest：{0} {1} -> {2} {3}", message.FromId, message.FromName, message.ToId, message.ToName);
            sender.Session.Response.FriendAddRes = new FriendAddResponse();
            if (message.ToId == 0)
            {
                foreach (var ch in CharacterManager.Instance.Characters)
                {
                    if (ch.Value.Data.Name == message.ToName)
                    {
                        message.ToId = ch.Key;
                        break;
                    }

                }
            }

            NetConnection<NetSession> Friend = null;
            if (message.ToId > 0)
            {
                if (cha.FriendManager.GetFriendInfo(message.ToId) != null)
                {
                    sender.Session.Response.FriendAddRes.Result = Result.Failed;
                    sender.Session.Response.FriendAddRes.Errormsg = "已经是好友了";
                    sender.Session.Response.FriendAddRes.Request = message;
                    sender.SendResponse();
                    return;
                }
            }
            Friend = SessionManager.Instance.GetSession(message.ToId);
            if (Friend == null)
            {
                sender.Session.Response.FriendAddRes.Result = Result.Failed;
                sender.Session.Response.FriendAddRes.Errormsg = "好友不存在或已离线";
                sender.Session.Response.FriendAddRes.Request = message;
                sender.SendResponse();
                return;
            }

            Log.InfoFormat("FriendAddRequest：{0} {1} -> {2} {3}", message.FromId, message.FromName, message.ToId, message.ToName);
            Friend.Session.Response.FriendAddReq = message;
            Friend.SendResponse();
        }
    }
}
