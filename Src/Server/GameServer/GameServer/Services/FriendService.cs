using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Services
{
    class FriendService : Singleton<FriendService>
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
            Log.InfoFormat("OnFriendRemove: character {0} remove friend {1}", cha.Id, message.friendId);
            sender.Session.Response.friendRemove = new FriendRemoveResponse();
            sender.Session.Response.friendRemove.Result = Result.Success;

            if (cha.friendManager.RemoveFriendById(message.friendId))
            {
                sender.Session.Response.friendRemove.Result = Result.Success;
                var friend = SessionManager.Instance.GetSession(message.friendId);
                if (friend != null)
                {//在线
                    friend.Session.Character.friendManager.RemoveFriendById(cha.Id);
                    friend.Session.Character.friendManager.RemoveFriendById(message.friendId);
                }
                else
                {//不在线
                    RemoveFriend(cha.Id, message.friendId);
                }
            }
            else
            {
                sender.Session.Response.friendRemove.Result = Result.Failed;
                sender.Session.Response.friendRemove.Errormsg = "好友不存在";
            }
            DBService.Instance.Save();
            sender.SendResponse();
        }

        private void RemoveFriend(int characterId, int friendId)
        {
            var characterRemoveItem = DBService.Instance.Entities.CharacterFriends.FirstOrDefault
                (v => v.CharacterID == characterId && v.FriendID == friendId);
            var friendeRmoveItem = DBService.Instance.Entities.CharacterFriends.FirstOrDefault
               (v => v.CharacterID == friendId && v.FriendID == characterId);
            if (characterRemoveItem != null)
            {
                DBService.Instance.Entities.CharacterFriends.Remove(characterRemoveItem);
            }
            if (friendeRmoveItem != null)
            {
                DBService.Instance.Entities.CharacterFriends.Remove(friendeRmoveItem);
            }
        }

        private void OnFriendAddResponse(NetConnection<NetSession> sender, FriendAddResponse message)
        {
            Character cha = sender.Session.Character;
            Log.InfoFormat("OnFriendAddResponse：character {0} Result{1} FromId{2} ToId{3}", cha.Id, message.Result, message.Request.FromId, message.Request.ToId);
            sender.Session.Response.friendAddRes = message;
            if (message.Result == Result.Success)
            {
                NetConnection<NetSession> friend = SessionManager.Instance.GetSession(message.Request.FromId);
                if (friend == null)
                {
                    sender.Session.Response.friendAddRes.Errormsg = "请求者已下线";
                    sender.Session.Response.friendAddRes.Result = Result.Failed;
                }
                else
                {
                    friend.Session.Character.friendManager.AddFriend(sender.Session.Character);
                    cha.friendManager.AddFriend(friend.Session.Character);
                    DBService.Instance.Save();
                    friend.Session.Response.friendAddRes = message;
                    friend.Session.Response.friendAddRes.Result = Result.Success;
                    friend.Session.Response.friendAddRes.Errormsg = "添加好友成功";
                    friend.SendResponse();
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
            sender.Session.Response.friendAddRes = new FriendAddResponse();
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

            NetConnection<NetSession> friend = null;
            if (message.ToId > 0)
            {
                if (cha.friendManager.GetFriendInfo(message.ToId) != null)
                {
                    sender.Session.Response.friendAddRes.Result = Result.Failed;
                    sender.Session.Response.friendAddRes.Errormsg = "已经是好友了";
                    sender.Session.Response.friendAddRes.Request = message;
                    sender.SendResponse();
                    return;
                }
            }
            friend = SessionManager.Instance.GetSession(message.ToId);
            if (friend == null)
            {
                sender.Session.Response.friendAddRes.Result = Result.Failed;
                sender.Session.Response.friendAddRes.Errormsg = "好友不存在或已离线";
                sender.Session.Response.friendAddRes.Request = message;
                sender.SendResponse();
                return;
            }

            Log.InfoFormat("FriendAddRequest：{0} {1} -> {2} {3}", message.FromId, message.FromName, message.ToId, message.ToName);
            friend.Session.Response.friendAddReq = message;
            friend.SendResponse();
        }
    }
}
