using Common.Utils;
using Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services
{
    class ChatService : Singleton<ChatService>, IDisposable
    {
        public ChatService()
        {
            MessageDistributer.Instance.Subscribe<ChatResponse>(OnChat);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ChatResponse>(OnChat);
        }

        public void Init()
        {

        }
        public void SendChat(ChatChannel sendChannel, string content, int toId, string toName)
        {
            var meg = new NetMessage();
            meg.Request = new NetMessageRequest();
            meg.Request.Chat = new ChatRequest();
            meg.Request.Chat.Message = new ChatMessage();
            meg.Request.Chat.Message.Message = content;
            meg.Request.Chat.Message.ToId = toId;
            meg.Request.Chat.Message.ToName = toName;
            meg.Request.Chat.Message.Channel = sendChannel;
            NetClient.Instance.SendMessage(meg);
        }

        void OnChat(object sender, ChatResponse message)
        {
            if (message.Result == Result.Success)
            {
                ChatManager.Instance.AddMessage(ChatChannel.Local, message.localMessages);
                ChatManager.Instance.AddMessage(ChatChannel.World, message.worldMessages);
                ChatManager.Instance.AddMessage(ChatChannel.System, message.systemMessages);
                ChatManager.Instance.AddMessage(ChatChannel.Private, message.Privatemessages);
                ChatManager.Instance.AddMessage(ChatChannel.Guild, message.guildMessages);
                ChatManager.Instance.AddMessage(ChatChannel.Team, message.teamMessages);
            }
            else
                ChatManager.Instance.AddSystemMessage(message.Errormsg);
        }
    }
}
