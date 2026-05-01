using Common;

namespace Network
{
    public class MessageDispatch<T> : Singleton<MessageDispatch<T>>
    {
        public void Dispatch(T sender, SkillBridge.Message.NetMessageResponse message)
        {
            if (message.UserRegister != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.UserRegister); }
            if (message.UserLogin != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.UserLogin); }
            if (message.CreateChar != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.CreateChar); }
            if (message.GameEnter != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.GameEnter); }
            if (message.GameLeave != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.GameLeave); }
            if (message.MapCharacterEnter != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.MapCharacterEnter); }
            if (message.MapCharacterLeave != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.MapCharacterLeave); }
            if (message.MapEntitySync != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.MapEntitySync); }
            if (message.ItemBuy != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.ItemBuy); }
            if (message.StatusNotify != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.StatusNotify); }
            if (message.ItemEquip != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.ItemEquip); }
            if (message.BagSave != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.BagSave); }
            if (message.QuestAccept != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.QuestAccept); }
            if (message.QuestSubmit != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.QuestSubmit); }
            if (message.FriendAddReq != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.FriendAddReq); }
            if (message.FriendAddRes != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.FriendAddRes); }
            if (message.FriendList != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.FriendList); }
            if (message.FriendRemove != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.FriendRemove); }
            if (message.TeamInfo != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.TeamInfo); }
            if (message.TeamInviteReq != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.TeamInviteReq); }
            if (message.TeamInviteRes != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.TeamInviteRes); }
            if (message.TeamLeave != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.TeamLeave); }
            if (message.GuildCreate != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.GuildCreate); }
            if (message.GuildJoinReq != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.GuildJoinReq); }
            if (message.GuildJoinRes != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.GuildJoinRes); }
            if (message.GuildLeave != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.GuildLeave); }
            if (message.GuildList != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.GuildList); }
            if (message.Guild != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.Guild); }
            if (message.GuildAdmin != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.GuildAdmin); }
            if (message.Chat != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.Chat); }
            if (message.SkillCast != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.SkillCast); }
            if (message.SkillHits != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.SkillHits); }
            if (message.BuffRes != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.BuffRes); }
            if (message.ArenaBeginRes != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.ArenaBeginRes); }
            if (message.ArenaChallengeReq != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.ArenaChallengeReq); }
            if (message.ArenaChallengeRes != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.ArenaChallengeRes); }
            if (message.ArenaEndRes != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.ArenaEndRes); }

        }


        public void Dispatch(T sender, SkillBridge.Message.NetMessageRequest message)
        {
            if (message.UserRegister != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.UserRegister); }
            if (message.UserLogin != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.UserLogin); }
            if (message.CreateChar != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.CreateChar); }
            if (message.GameEnter != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.GameEnter); }
            if (message.GameLeave != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.GameLeave); }
            if (message.MapCharacterEnter != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.MapCharacterEnter); }
            if (message.MapEntitySync != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.MapEntitySync); }
            if (message.ItemBuy != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.ItemBuy); }
            if (message.ItemEquip != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.ItemEquip); }
            if (message.BagSave != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.BagSave); }
            if (message.QuestAccept != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.QuestAccept); }
            if (message.QuestSubmit != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.QuestSubmit); }
            if (message.MapTeleport != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.MapTeleport); }
            if (message.FriendAddReq != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.FriendAddReq); }
            if (message.FriendAddRes != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.FriendAddRes); }
            if (message.FriendList != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.FriendList); }
            if (message.FriendRemove != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.FriendRemove); }
            if (message.TeamInfo != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.TeamInfo); }
            if (message.TeamInviteReq != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.TeamInviteReq); }
            if (message.TeamInviteRes != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.TeamInviteRes); }
            if (message.TeamLeave != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.TeamLeave); }
            if (message.GuildCreate != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.GuildCreate); }
            if (message.GuildJoinReq != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.GuildJoinReq); }
            if (message.GuildJoinRes != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.GuildJoinRes); }
            if (message.GuildLeave != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.GuildLeave); }
            if (message.GuildList != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.GuildList); }
            if (message.Guild != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.Guild); }
            if (message.GuildAdmin != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.GuildAdmin); }
            if (message.Chat != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.Chat); }
            if (message.SkillCast != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.SkillCast); }
            if (message.ArenaChallengeReq != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.ArenaChallengeReq); }
            if (message.ArenaChallengeRes != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.ArenaChallengeRes); }
        }
    }
}