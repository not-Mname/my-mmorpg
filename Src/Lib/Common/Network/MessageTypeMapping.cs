//using Google.Protobuf.Reflection;
//using Google.Protobuf.WellKnownTypes;
//using SkillBridge.Message;
//using System.Collections.Generic;

//namespace Network
//{
//    /// <summary>
//    /// 消息类型映射表：加新消息只在这里加一行
//    /// </summary>
//    public static class MessageTypeMapping
//    {
//        public static readonly Dictionary<int, MessageType> RequestMap = new Dictionary<int, MessageType>
//        {
//            { 1,  MessageType.MsgUserRegister },
//            { 2,  MessageType.MsgUserLogin },
//            { 3,  MessageType.MsgCreateChar },
//            { 4,  MessageType.MsgGameEnter },
//            { 5,  MessageType.MsgGameLeave },
//            { 6,  MessageType.MsgMapCharacterEnter },
//            { 8,  MessageType.MsgMapEntitySync },
//            { 9,  MessageType.MsgMapTeleport },
//            { 10, MessageType.MsgItemBuy },
//            { 11, MessageType.MsgItemEquip },
//            { 12, MessageType.MsgQuestList },
//            { 13, MessageType.MsgQuestAccept },
//            { 14, MessageType.MsgBagSave },
//            { 15, MessageType.MsgQuestSubmit },
//            { 16, MessageType.MsgFriendAddRequest },
//            { 17, MessageType.MsgFriendAddResponse },
//            { 18, MessageType.MsgFriendList },
//            { 19, MessageType.MsgFriendRemove },
//            { 20, MessageType.MsgTeamInviteRequest },
//            { 21, MessageType.MsgTeamInviteResponse },
//            { 22, MessageType.MsgTeamInfo },
//            { 23, MessageType.MsgTeamLeave },
//            { 24, MessageType.MsgGuildCreate },
//            { 25, MessageType.MsgGuildJoinRequest },
//            { 26, MessageType.MsgGuildJoinResponse },
//            { 27, MessageType.MsgGuildInfo },
//            { 28, MessageType.MsgGuildLeave },
//            { 29, MessageType.MsgGuildList },
//            { 30, MessageType.MsgGuildAdmin },
//            { 31, MessageType.MsgChat },
//            { 50, MessageType.MsgSkillCast },
//            { 60, MessageType.MsgArenaChallengeRequest },
//            { 61, MessageType.MsgArenaChallengeResponse },
//            { 62, MessageType.MsgArenaReady },
//        };

//        public static readonly Dictionary<int, MessageType> ResponseMap = new Dictionary<int, MessageType>
//        {
//            { 1,   MessageType.MsgUserRegister },
//            { 2,   MessageType.MsgUserLogin },
//            { 3,   MessageType.MsgCreateChar },
//            { 4,   MessageType.MsgGameEnter },
//            { 5,   MessageType.MsgGameLeave },
//            { 6,   MessageType.MsgMapCharacterEnter },
//            { 7,   MessageType.MsgMapCharacterLeave },
//            { 8,   MessageType.MsgMapEntitySync },
//            { 10,  MessageType.MsgItemBuy },
//            { 11,  MessageType.MsgItemEquip },
//            { 12,  MessageType.MsgQuestList },
//            { 13,  MessageType.MsgQuestAccept },
//            { 14,  MessageType.MsgBagSave },
//            { 15,  MessageType.MsgQuestSubmit },
//            { 16,  MessageType.MsgFriendAddRequest },
//            { 17,  MessageType.MsgFriendAddResponse },
//            { 18,  MessageType.MsgFriendList },
//            { 19,  MessageType.MsgFriendRemove },
//            { 20,  MessageType.MsgTeamInviteRequest },
//            { 21,  MessageType.MsgTeamInviteResponse },
//            { 22,  MessageType.MsgTeamInfo },
//            { 23,  MessageType.MsgTeamLeave },
//            { 24,  MessageType.MsgGuildCreate },
//            { 25,  MessageType.MsgGuildJoinRequest },
//            { 26,  MessageType.MsgGuildJoinResponse },
//            { 27,  MessageType.MsgGuildInfo },
//            { 28,  MessageType.MsgGuildLeave },
//            { 29,  MessageType.MsgGuildList },
//            { 30,  MessageType.MsgGuildAdmin },
//            { 31,  MessageType.MsgChat },
//            { 50,  MessageType.MsgSkillCast },
//            { 51,  MessageType.MsgSkillHit },
//            { 52,  MessageType.MsgBuff },
//            { 60,  MessageType.MsgArenaChallengeRequest },
//            { 61,  MessageType.MsgArenaChallengeResponse },
//            { 62,  MessageType.MsgArenaBegin },
//            { 63,  MessageType.MsgArenaEnd },
//            { 64,  MessageType.MsgArenaReady },
//            { 65,  MessageType.MsgArenaRoundStart },
//            { 66,  MessageType.MsgArenaRoundEnd },
//            { 100, MessageType.MsgStatusNotify },
//        };

//        /// <summary>
//        /// 发包前自动检测 NetMessageRequest 中哪些字段被设置了，填充 PayloadTypes
//        /// </summary>
//        public static void PopulateRequestTypes(NetMessageRequest request)
//        {
//            request.PayloadTypes.Clear();
//            foreach (FieldDescriptor fd in NetMessageRequest.Descriptor.Fields.InDeclarationOrder())
//            {
//                if (fd.IsRepeated || fd.FieldNumber >= 1000) continue;
//                if (fd.Accessor.GetValue(request) != null && RequestMap.TryGetValue(fd.FieldNumber, out MessageType mt))
//                    request.PayloadTypes.Add(mt);
//            }
//        }

//        /// <summary>
//        /// 发包前自动检测 NetMessageResponse 中哪些字段被设置了，填充 PayloadTypes
//        /// </summary>
//        public static void PopulateResponseTypes(NetMessageResponse response)
//        {
//            response.PayloadTypes.Clear();
//            foreach (FieldDescriptor fd in NetMessageResponse.Descriptor.Fields.InDeclarationOrder())
//            {
//                if (fd.IsRepeated || fd.FieldNumber >= 1000) continue;
//                if (fd.Accessor.GetValue(response) != null && ResponseMap.TryGetValue(fd.FieldNumber, out MessageType mt))
//                    response.PayloadTypes.Add(mt);
//            }
//        }
//    }
//}
