using Assets.Scripts.Models;
using Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Services
{
    public class QuestService : Singleton<QuestService>, IDisposable
    {
        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<QuestAcceptResponse>(this.OnQuestAccept);
            MessageDistributer.Instance.Unsubscribe<QuestSubmitResponse>(this.OnQuestSubmit);
        }
        public QuestService()
        {
            MessageDistributer.Instance.Subscribe<QuestAcceptResponse>(this.OnQuestAccept);
            MessageDistributer.Instance.Subscribe<QuestSubmitResponse>(this.OnQuestSubmit);           
        }
        private void OnQuestSubmit(object sender, QuestSubmitResponse message)
        {
            Debug.LogFormat("OnQuestSubmit {0} : [{1}]", message.Result, message.Errormsg);
            if(message.Result == Result.Success)
            {
                QuestManager.Instance.OnQuestSubmit(message.Quest);
            }
            else
            {
                MessageBox.Show(string.Format("提交任务失败，原因：{0}", message.Errormsg), "错误", MessageBoxType.Error);
            }
        }

        private void OnQuestAccept(object sender, QuestAcceptResponse message)
        {
            Debug.LogFormat("OnQuestAccept {0} : [{1}]", message.Result, message.Errormsg);
            if (message.Result == Result.Success)
            {
                QuestManager.Instance.OnQuestAccept(message.Quest);
            }
            else
            {
                MessageBox.Show(string.Format("接受任务失败，原因：{0}", message.Errormsg), "错误", MessageBoxType.Error);
            }
        }

        public bool SendQuestAccept(Quest quest)
        {
            Debug.Log(" Send Quest Accept");
            var meg = new NetMessage();
            meg.Request = new NetMessageRequest();
            meg.Request.QuestAccept = new QuestAcceptRequest(); 
            meg.Request.QuestAccept.QuestId = quest.define.ID;
            NetClient.Instance.SendMessage(meg);
            return true;
        }

        public bool SendQuestSubmit(Quest quest)
        {
            Debug.Log(" Send Quest Submit");
            var meg = new NetMessage();
            meg.Request = new NetMessageRequest();
            meg.Request.QuestSubmit = new QuestSubmitRequest();
            meg.Request.QuestSubmit.QuestId = quest.define.ID;
            NetClient.Instance.SendMessage(meg);    
            return true;
        }
    }
}
