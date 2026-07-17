using Common;

namespace Network
{
    public class MessageDispatch<T> : Singleton<MessageDispatch<T>>
    {
        public void Dispatch(T sender, SkillBridge.Message.NetMessageResponse message)
        {
            switch (message.PayloadCase)
            {
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.UserRegister:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.UserRegister);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.UserLogin:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.UserLogin);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.CreateChar:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.CreateChar);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.GameEnter:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.GameEnter);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.GameLeave:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.GameLeave);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.MapCharacterEnter:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.MapCharacterEnter);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.MapCharacterLeave:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.MapCharacterLeave);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.MapEntitySync:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.MapEntitySync);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.ItemBuy:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.ItemBuy);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.ItemEquip:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.ItemEquip);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.QuestList:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.QuestList);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.QuestAccept:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.QuestAccept);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.BagSave:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.BagSave);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.QuestSubmit:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.QuestSubmit);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.FriendAddReq:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.FriendAddReq);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.FriendAddRes:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.FriendAddRes);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.FriendList:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.FriendList);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.FriendRemove:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.FriendRemove);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.TeamInfo:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.TeamInfo);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.TeamInviteReq:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.TeamInviteReq);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.TeamInviteRes:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.TeamInviteRes);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.TeamLeave:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.TeamLeave);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.GuildCreate:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.GuildCreate);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.GuildJoinReq:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.GuildJoinReq);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.GuildJoinRes:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.GuildJoinRes);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.GuildLeave:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.GuildLeave);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.GuildList:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.GuildList);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.Guild:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.Guild);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.GuildAdmin:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.GuildAdmin);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.Chat:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.Chat);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.SkillCast:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.SkillCast);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.SkillHits:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.SkillHits);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.BuffRes:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.BuffRes);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.ArenaBeginRes:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.ArenaBeginRes);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.ArenaChallengeReq:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.ArenaChallengeReq);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.ArenaChallengeRes:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.ArenaChallengeRes);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.ArenaEndRes:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.ArenaEndRes);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.ArenaReadyRes:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.ArenaReadyRes);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.ArenaRoundStartRes:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.ArenaRoundStartRes);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.ArenaRoundEndRes:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.ArenaRoundEndRes);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.StatusNotify:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.StatusNotify);
                    break;
                case SkillBridge.Message.NetMessageResponse.PayloadOneofCase.None:
                default:
                    Log.Warning("Received an empty response envelope");
                    break;
            }
        }

        public void Dispatch(T sender, SkillBridge.Message.NetMessageRequest message)
        {
            switch (message.PayloadCase)
            {
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.UserRegister:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.UserRegister);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.UserLogin:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.UserLogin);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.CreateChar:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.CreateChar);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.GameEnter:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.GameEnter);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.GameLeave:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.GameLeave);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.MapCharacterEnter:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.MapCharacterEnter);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.MapEntitySync:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.MapEntitySync);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.MapTeleport:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.MapTeleport);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.ItemBuy:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.ItemBuy);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.ItemEquip:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.ItemEquip);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.QuestList:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.QuestList);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.QuestAccept:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.QuestAccept);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.BagSave:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.BagSave);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.QuestSubmit:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.QuestSubmit);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.FriendAddReq:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.FriendAddReq);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.FriendAddRes:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.FriendAddRes);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.FriendList:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.FriendList);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.FriendRemove:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.FriendRemove);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.TeamInfo:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.TeamInfo);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.TeamInviteReq:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.TeamInviteReq);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.TeamInviteRes:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.TeamInviteRes);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.TeamLeave:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.TeamLeave);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.GuildCreate:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.GuildCreate);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.GuildJoinReq:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.GuildJoinReq);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.GuildJoinRes:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.GuildJoinRes);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.GuildLeave:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.GuildLeave);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.GuildList:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.GuildList);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.Guild:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.Guild);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.GuildAdmin:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.GuildAdmin);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.Chat:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.Chat);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.SkillCast:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.SkillCast);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.ArenaChallengeReq:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.ArenaChallengeReq);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.ArenaChallengeRes:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.ArenaChallengeRes);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.ArenaReadyReq:
                    MessageDistributer<T>.Instance.RaiseEvent(sender, message.ArenaReadyReq);
                    break;
                case SkillBridge.Message.NetMessageRequest.PayloadOneofCase.None:
                default:
                    Log.Warning("Received an empty request envelope");
                    break;
            }
        }
    }
}
