using Common;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class QuestService : Singleton<QuestService>
    {
        public QuestService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<QuestSubmitRequest>(this.OnQuestAccept);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<QuestAcceptRequest>(this.OnQuestSubmit);
        }

        private void OnQuestSubmit(NetConnection<NetSession> sender, QuestAcceptRequest message)
        {
            Log.InfoFormat("QuestSubmitResponse received CharactorID:{0} QuestID:{1} ", sender.Session.Character.Id, message.QuestId);
            sender.Session.Response.questAccept = new QuestAcceptResponse();

            Result result = QuestManager.Instance.AcceptQuest(sender, message.QuestId);
            sender.Session.Response.questAccept.Result = result;
            sender.SendResponse();
        }

        private void OnQuestAccept(NetConnection<NetSession> sender, QuestSubmitRequest message)
        {
            Log.InfoFormat("QuestSubmitRequest received CharactorID:{0} QuestID:{1} ", sender.Session.Character.Id, message.QuestId);
            sender.Session.Response.questSubmit = new QuestSubmitResponse();
            Result result = QuestManager.Instance.SubmitQuest(sender, message.QuestId);
            sender.Session.Response.questSubmit.Result = result;
            sender.SendResponse();
        }
        public void Init()
        {

        }


    }
}
