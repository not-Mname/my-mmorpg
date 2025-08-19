using Common;
using Common.Data;
using Common.Utils;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;

namespace GameServer.Managers
{
    class ChatManager : Singleton<ChatManager>
    {
        public List<ChatMessage> System = new List<ChatMessage>();
        public List<ChatMessage> World = new List<ChatMessage>();
        public Dictionary<int, List<ChatMessage>> Local = new Dictionary<int, List<ChatMessage>>();
        public Dictionary<int, List<ChatMessage>> Guild = new Dictionary<int, List<ChatMessage>>();
        public Dictionary<int, List<ChatMessage>> Team = new Dictionary<int, List<ChatMessage>>();

        public double timestamp;

        public void Init()
        {
        }

        public void AddMessage(Character from, ChatMessage message)
        {
            message.FromId = from.Id;
            message.FromName = from.Name;
            message.Time = TimeUtil.timestamp;
            switch (message.Channel)
            {
                case ChatChannel.Local:
                    this.AddLocalMessage(from.Info.mapId, message);
                    break;
                case ChatChannel.Guild:
                    this.AddGuildMessage(from.Info.Guild.Id, message);
                    break;
                case ChatChannel.Team:
                    this.AddTeamMessage(from.Team.Id, message);
                    break;
                case ChatChannel.World:
                    this.AddWorldMessage(message);
                    break;
                case ChatChannel.System:
                    this.AddSystemMessage(message);
                    break;
            }
            timestamp = TimeUtil.timestamp;
        }

        private void AddSystemMessage(ChatMessage message)
        {
            this.System.Add(message);
        }

        private void AddWorldMessage(ChatMessage message)
        {
            this.World.Add(message);
        }

        private void AddTeamMessage(int id, ChatMessage message)
        {
            if (!this.Team.TryGetValue(id, out var messages))
            {
                messages = new List<ChatMessage>();
                this.Team.Add(id, messages);
            
            }
            messages.Add(message);
        }

        private void AddGuildMessage(int id, ChatMessage message)
        {
            if (!this.Guild.TryGetValue(id, out var messages))
            {
                messages = new List<ChatMessage>();
                this.Guild.Add(id, messages);
            
            }
            messages.Add(message);
        }

        private void AddLocalMessage(int mapId, ChatMessage message)
        {
            if(!this.Local.TryGetValue(mapId, out var messages))
            {
                messages = new List<ChatMessage>();
                this.Local.Add(mapId, messages);
            }
            messages.Add(message);
        }

        public int GetLocalMessage(int mapId, int idx, List<ChatMessage> result)
        {
            if (!this.Local.TryGetValue(mapId, out var messages))
            {
                return 0;
            }
            return GetNewMessages(idx, result, messages);
        }

        public int GetGuildMessage(int guildId, int idx, List<ChatMessage> result)
        {
            if (!this.Guild.TryGetValue(guildId, out var messages))
            {
                return 0;
            }
            return GetNewMessages(idx, result, messages);
        }
        public int GetTeamMessage(int teamId, int idx, List<ChatMessage> result)
        {
            if (!this.Team.TryGetValue(teamId, out var messages))
            {
                return 0;
            }
            return GetNewMessages(idx, result, messages);
        }

        public int GetWorldMessage(int idx, List<ChatMessage> result)
        {
            return GetNewMessages(idx, result, this.World);
        }
        public int GetSystemMessage(int idx, List<ChatMessage> result)
        {
            return GetNewMessages(idx, result, this.System);
        }

        private int GetNewMessages(int idx, List<ChatMessage> result, List<ChatMessage> messages)
        {
            if(idx == 0)//如果时第一次获取信息
            {
                if(messages.Count > GameDefine.MaxChatRecoredNums)//如果当前消息大于最大记录数量
                {
                    idx = messages.Count - GameDefine.MaxChatRecoredNums;//获取后面的消息
                }
            }

            for (; idx < messages.Count; idx++)
            {
                result.Add(messages[idx]);
            }
            return idx;
        }
    }
}
