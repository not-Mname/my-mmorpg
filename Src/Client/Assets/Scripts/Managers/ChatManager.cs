using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
namespace Managers
{
    class ChatManager : Singleton<ChatManager>
    {
        public List<ChatMessage>[] Messages = new List<ChatMessage>[6]
            {
                new List<ChatMessage>(),
                new List<ChatMessage>(),
                new List<ChatMessage>(),
                new List<ChatMessage>(),
                new List<ChatMessage>(),
                new List<ChatMessage>(),
            };

        public LocalChannel DisplayChannel;
        public LocalChannel sendChannel;
        public int PrivateID = 0;
        public string PrivateName = "";
        public enum LocalChannel
        {
            All = 0,
            Local = 1,
            World = 2,
            Team = 3,
            Guild = 4,
            Private = 5,
        }

        private ChatChannel[] channelFiler = new ChatChannel[6]
            {
                ChatChannel.Local | ChatChannel.World | ChatChannel.Team | ChatChannel.Guild | ChatChannel.Private | ChatChannel.System,
                ChatChannel.Local,
                ChatChannel.World,
                ChatChannel.Team,
                ChatChannel.Guild,
                ChatChannel.Private,
            };

        public Action OnChat { get; internal set; }
        public ChatChannel SendChannel
        {
            get
            {
                switch (this.sendChannel)
                {
                    case LocalChannel.Local: return ChatChannel.Local;
                    case LocalChannel.World: return ChatChannel.World;
                    case LocalChannel.Team: return ChatChannel.Team;
                    case LocalChannel.Guild: return ChatChannel.Guild;
                    case LocalChannel.Private: return ChatChannel.Private;
                }
                return ChatChannel.Local;
            }
        }
        public void Init()
        {
            foreach (var meg in Messages)
            {
                meg.Clear();
            }
        }
        public string GetCurrentMessages()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var meg in Messages[(int)DisplayChannel])
            {
                sb.AppendLine(FormatMessage(meg));
            }
            return sb.ToString();
        }

        private string FormatMessage(ChatMessage meg)
        {
            switch (meg.Channel)
            {
                case ChatChannel.Local:
                    return string.Format("[本地]{0}{1}", FormatFromPlayer(meg), meg.Message);
                case ChatChannel.World:
                    return string.Format("<color=cyan>[世界]{0}{1}</color>", FormatFromPlayer(meg), meg.Message);
                case ChatChannel.Team:
                    return string.Format("<color=green>[队伍]{0}{1}</color>", FormatFromPlayer(meg), meg.Message);
                case ChatChannel.Guild:
                    return string.Format("<color=blue>[公会]{0}{1}</color>", FormatFromPlayer(meg), meg.Message);
                case ChatChannel.Private:
                    return string.Format("<color=magenta>[私聊]{0}{1}</color>", FormatFromPlayer(meg), meg.Message);
                case ChatChannel.System:
                    return string.Format("<color=yellow>[系统]{0}</color>", meg.Message);
            }
            return "";
        }

        private object FormatFromPlayer(ChatMessage meg)
        {
            if (meg.FromId == User.Instance.CurrentCharacterInfo.Id)
            {
                return "<a name=\"\" class=\"player\">[你]</a>";
            }
            else
                return string.Format("<a name=\"c:{0}:{1}\" class=\"player\">[{1}]</a>", meg.FromId, meg.FromName);
        }

        public void StartPrivateChat(int targetId, string targetName)
        {
            PrivateID = targetId;
            PrivateName = targetName;
            sendChannel = LocalChannel.Private;
            if (this.OnChat != null)
                this.OnChat();

        }

        public void SendChat(string content, int toId = 0, string toName = "")
        {
            ChatService.Instance.SendChat(this.SendChannel, content, PrivateID, PrivateName);
        }

        public bool SetSendChannle(LocalChannel channel)
        {
            if (channel == LocalChannel.Team)
            {
                if (User.Instance.TeamInfo == null)
                {
                    this.AddSystemMessage("你还不是团队成员");
                    return false;
                }
            }
            if (channel == LocalChannel.Guild)
            {
                if (User.Instance.CurrentCharacterInfo.Guild == null)
                {
                    this.AddSystemMessage("你还没有加入公会");
                    return false;
                }
            }
            this.sendChannel = channel;
            Debug.Log("SetSendChannle: " + channel);
            return true;
        }

        public void AddSystemMessage(string message, string fromName = "")
        {
            this.Messages[(int)LocalChannel.All].Add(new ChatMessage()
            {
                Channel = ChatChannel.System,
                Message = message,
                FromName = fromName,
            });
            if (this.OnChat != null)
                this.OnChat();
        }

        public void AddMessage(ChatChannel channel, List<ChatMessage> messages)
        {
            for(int i = 0; i < 6; i++)
            {
                if ((this.channelFiler[i] & channel) == channel)
                {
                    this.Messages[i].AddRange(messages);
                }
            }
            if(OnChat != null)
                OnChat();
        }

    }
}
