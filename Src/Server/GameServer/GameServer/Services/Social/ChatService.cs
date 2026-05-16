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
                    if(sender.Session.Response.Chat == null)
                        sender.Session.Response.Chat = new ChatResponse();
                    sender.Session.Response.Chat.Result = Result.Success;
                    sender.Session.Response.Chat.Errormsg = "对方不在线";
                    sender.SendResponse();
                    return;
                }
                else
                {
                    if(target.Session.Response.Chat == null)
                        target.Session.Response.Chat = new ChatResponse();
                    message.Message.FromId = character.Id;
                    message.Message.FromName = character.Name;
                    target.Session.Response.Chat.Result = Result.Success;
                    target.Session.Response.Chat.PrivateMessages.Add(message.Message);
                    target.SendResponse();
                    if (sender.Session.Response.Chat == null)
                        sender.Session.Response.Chat = new ChatResponse();
                    sender.Session.Response.Chat.Result = Result.Success;
                    sender.Session.Response.Chat.PrivateMessages.Add(message.Message);
                    sender.SendResponse();
                }
            }
            else
            {
                sender.Session.Response.Chat = new ChatResponse();
                sender.Session.Response.Chat.Result = Result.Success;
                ChatManager.Instance.AddMessage(character, message.Message);
                sender.SendResponse();
            }
        }

    }
}
