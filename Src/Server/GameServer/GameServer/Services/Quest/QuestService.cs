using Common;
using GameInterFace;
using GameServer.Managers.Quest;
using Network;
using SkillBridge.Message;

namespace GameServer.Services.Quest
{
    class QuestService : Singleton<QuestService>, IInitializable
    {
        public QuestService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<QuestSubmitRequest>(this.OnQuestAccept);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<QuestAcceptRequest>(this.OnQuestSubmit);
        }

        private void OnQuestSubmit(NetConnection<NetSession> sender, QuestAcceptRequest message)
        {
            Log.InfoFormat("QuestSubmitResponse received CharactorID:{0} QuestID:{1} ", sender.Session.Character.Id, message.QuestId);
            sender.Session.Response.QuestAccept = new QuestAcceptResponse();

            Result result = QuestManager.Instance.AcceptQuest(sender, message.QuestId);
            sender.Session.Response.QuestAccept.Result = result;
            sender.SendResponse();
        }

        private void OnQuestAccept(NetConnection<NetSession> sender, QuestSubmitRequest message)
        {
            Log.InfoFormat("QuestSubmitRequest received CharactorID:{0} QuestID:{1} ", sender.Session.Character.Id, message.QuestId);
            sender.Session.Response.QuestSubmit = new QuestSubmitResponse();
            Result result = QuestManager.Instance.SubmitQuest(sender, message.QuestId);
            sender.Session.Response.QuestSubmit.Result = result;
            sender.SendResponse();
        }
        public void Init()
        {
            Log.Info("QuestService Init...");
        }


    }
}
