using Common;
using GameServer.Entities;
using GameServer.Managers;
using SkillBridge.Message;

namespace GameServer.Models
{
    class Chat
    {
        Character _owner;

        public int LocalIdx;
        public int WorldIdx;
        public int SystemIdx;
        public int TeamIdx;
        public int GuildIdx;


        public Chat(Character character)
        {
            this._owner = character;
        }

        public void PostProcess(NetMessageResponse message)
        {
            Log.InfoFormat("PostProcess: > Chat {0}", _owner.Info.Id);
            if (message.Chat == null)
            {
                message.Chat = new ChatResponse();
                message.Chat.Result = Result.Success;
            }
            this.LocalIdx = ChatManager.Instance.GetLocalMessage(this._owner.Info.mapId, this.LocalIdx, message.Chat.localMessages);
            this.WorldIdx = ChatManager.Instance.GetWorldMessage(this.WorldIdx, message.Chat.worldMessages);
            this.SystemIdx = ChatManager.Instance.GetSystemMessage(this.SystemIdx, message.Chat.systemMessages);
            if(this._owner.Team != null)
                this.TeamIdx = ChatManager.Instance.GetTeamMessage(this._owner.Team.Id, this.TeamIdx, message.Chat.teamMessages);
            if(this._owner.Guild != null)
                this.GuildIdx = ChatManager.Instance.GetGuildMessage(this._owner.Guild.Id, this.GuildIdx, message.Chat.guildMessages);
        }
    }
}
