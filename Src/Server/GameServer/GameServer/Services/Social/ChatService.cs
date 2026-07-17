using Common;
using GameInterFace;
using GameServer.Entities;
using GameServer.Managers.Net;
using GameServer.Managers.Social;
using Network;
using SkillBridge.Message;
using System;
using System.ComponentModel;

namespace GameServer.Services.Social
{
    class ChatService : Singleton<ChatService>, IDisposable, IInitializable
    {
        public void Init()
        {
            Log.Info("ChatService Init...");
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ChatRequest>(this.OnChat);
            ChatManager.Instance.Init();
        }

        public void Dispose()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<ChatRequest>(this.OnChat);
        }

        void OnChat(NetConnection<NetSession> sender, ChatRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("Chat: character {0} channel {1} Message {2}", character.Id, message.Message.Channel, message.Message.Message);
            if (message.Message.Channel == ChatChannel.Private)
            {
                var target = SessionManager.Instance.GetSession(message.Message.ToId);
                if (target == null)
                {
                    var chatRes = new ChatResponse();
                    chatRes.Result = Result.Success;
                    chatRes.Errormsg = "对方不在线";
                    sender.Session.AddResponse(new NetMessageResponse { Chat = chatRes });
                    sender.SendResponse();
                    return;
                }
                else
                {
                    message.Message.FromId = character.Id;
                    message.Message.FromName = character.Name;
                    var targetChatRes = new ChatResponse();
                    targetChatRes.Result = Result.Success;
                    targetChatRes.PrivateMessages.Add(message.Message);
                    target.Session.AddResponse(new NetMessageResponse { Chat = targetChatRes });
                    target.SendResponse();
                    var senderChatRes = new ChatResponse();
                    senderChatRes.Result = Result.Success;
                    senderChatRes.PrivateMessages.Add(message.Message);
                    sender.Session.AddResponse(new NetMessageResponse { Chat = senderChatRes });
                    sender.SendResponse();
                }
            }
            else
            {
                var chatRes = new ChatResponse();
                chatRes.Result = Result.Success;
                ChatManager.Instance.AddMessage(character, message.Message);
                sender.Session.AddResponse(new NetMessageResponse { Chat = chatRes });
                sender.SendResponse();
            }
        }

    }
}
