using Common;
using GameInterFace;
using GameServer.Entities;
using GameServer.Managers.MBattle;
using GameServer.Managers.Net;
using GameServer.Models.Logic;
using Network;
using SkillBridge.Message;

namespace GameServer.Services.Battle
{
    internal class ArenaService : Singleton<ArenaService>, IDisposable, IInitializable
    {
        public void Dispose()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<ArenaChallengeRequest>(OnArenaChallengeRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<ArenaChallengeResponse>(OnArenaChallengeResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<ArenaReadyRequest>(OnArenaReadyRequest);

        }

        public void Init()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ArenaChallengeRequest>(OnArenaChallengeRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ArenaChallengeResponse>(OnArenaChallengeResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ArenaReadyRequest>(OnArenaReadyRequest);
            ArenaManager.Instance.Init();
        }

        private void OnArenaReadyRequest(NetConnection<NetSession> sender, ArenaReadyRequest message)
        {
            var arena = ArenaManager.Instance.GetArena(message.ArenaId);
            arena?.EntityReady(message.EntityId);
        }

        private void OnArenaChallengeRequest(NetConnection<NetSession> sender, ArenaChallengeRequest request)
        {
            Character character = sender.Session.Character;
            Log.Info($"ArenaChallengeRequest: RedId {request.Info.Red.EntityId}, RedName {request.Info.Red.Name}, BlueId {request.Info.Blue.EntityId}, BlueName {request.Info.Blue.Name}");

            NetConnection<NetSession>? blue = null;
            if (request.Info.Blue.EntityId > 0)
            {
                blue = SessionManager.Instance.GetSession(request.Info.Blue.EntityId);
            }
            if (blue == null)
            {
                sender.Session.AddResponse(new NetMessageResponse { ArenaChallengeRes = new ArenaChallengeResponse()
                {
                    Result = Result.Failed,
                    Errormsg = "玩家不在线"
                } });
                return;
            }

            blue.Session.AddResponse(new NetMessageResponse { ArenaChallengeReq = request });
            blue.SendResponse();
        }

        private void OnArenaChallengeResponse(NetConnection<NetSession> sender, ArenaChallengeResponse response)
        {
            Log.Info($"ArenaChallengeResponse: RedId {response.ArenaInfo.Red.EntityId}, RedName {response.ArenaInfo.Red.Name}, BlueId {response.ArenaInfo.Blue.EntityId}, BlueName {response.ArenaInfo.Blue.Name}");
            NetConnection<NetSession> requester = SessionManager.Instance.GetSession(response.ArenaInfo.Red.EntityId);
            if (requester == null)
            {
                sender.Session.AddResponse(new NetMessageResponse { ArenaChallengeRes = new ArenaChallengeResponse() { Errormsg = "玩家不在线", Result = Result.Failed } });
                sender.SendResponse();
                return;
            }
            if (response.Result == Result.Failed)
            {
                requester.Session.AddResponse(new NetMessageResponse { ArenaChallengeRes = new ArenaChallengeResponse() { Result = Result.Failed } });
                requester.SendResponse();
            }

            var arena = ArenaManager.Instance.NewArena(response.ArenaInfo ,requester, sender);
            this.SendArenaBegin(arena);
        }

        private void SendArenaBegin(Arena arena)
        {
            var arenaBegin = new ArenaBeginResponse() { 
                ArenaInfo = arena.ArenaInfo,
                Result = Result.Failed,
                Errormsg = "玩家不在线"
            };
            arena.Red.Session.AddResponse(new NetMessageResponse { ArenaBeginRes = arenaBegin });
            arena.Blue.Session.AddResponse(new NetMessageResponse { ArenaBeginRes = arenaBegin });
            arena.Red.SendResponse();
            arena.Blue.SendResponse();
        }

        internal void SendArenaReady(Arena arena)
        {
            var arenaReady = new ArenaReadyResponse()
            {
                ArenaInfo = arena.ArenaInfo,
                Round = arena.Round,
            };
            arena.Red.Session.AddResponse(new NetMessageResponse { ArenaReadyRes = arenaReady });
            arena.Blue.Session.AddResponse(new NetMessageResponse { ArenaReadyRes = arenaReady });
            arena.Red.SendResponse();
            arena.Blue.SendResponse();
        }

        internal void SendArenaRoundStart(Arena arena)
        {
            var roundStart = new ArenaRoundStartResponse()
            {
                ArenaInfo = arena.ArenaInfo,
                Round = arena.Round,
            };
            arena.Red.Session.AddResponse(new NetMessageResponse { ArenaRoundStartRes = roundStart });
            arena.Blue.Session.AddResponse(new NetMessageResponse { ArenaRoundStartRes = roundStart });
            arena.Red.SendResponse();
            arena.Blue.SendResponse();
        }

        internal void SendArenaRoundEnd(Arena arena)
        {
            var roundEnd = new ArenaRoundEndResponse()
            {
                ArenaInfo = arena.ArenaInfo,
                Round = arena.Round,
            };
            arena.Red.Session.AddResponse(new NetMessageResponse { ArenaRoundEndRes = roundEnd });
            arena.Blue.Session.AddResponse(new NetMessageResponse { ArenaRoundEndRes = roundEnd });
            arena.Red.SendResponse();
            arena.Blue.SendResponse();
        }
    }
}
