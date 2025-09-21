using Assets.Scripts.Models;
using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;


namespace Managers
{
    public enum NPCQuestStatus
    {
        None,//无任务
        Complete,//拥有已完成可交付任务
        Available,//拥有可接受的任务
        Incomplete,//拥有未完成任务
    }

    class QuestManager : Singleton<QuestManager>
    {
        //所有有效任务
        public List<NQuestInfo> questInfo = new List<NQuestInfo>();
        public Dictionary<int, Quest> allQuests = new Dictionary<int, Quest>();
        //<NPCId, <任务状态, 对应列表>>
        public Dictionary<int, Dictionary<NPCQuestStatus, List<Quest>>> npcQuests = new Dictionary<int, Dictionary<NPCQuestStatus, List<Quest>>>();
        public UnityAction<Quest> onQuestStatusChanged;

        public void Init(List<NQuestInfo> quests)
        {
            questInfo = quests;
            allQuests.Clear();
            npcQuests.Clear();
            InitQuests();
        }

        void InitQuests()
        {
            //初始化已有任务
            foreach (var info in questInfo)
            {
                Quest quest = new Quest(info);
                this.allQuests[quest.define.ID] = quest;
            }

            CheckAvailableQuests();

            foreach (var kv in allQuests)
            {
                this.AddNpcQuest(kv.Value.define.AcceptNPC, kv.Value);
                this.AddNpcQuest(kv.Value.define.SubmitNPC, kv.Value);
            }
        }

        private void CheckAvailableQuests()
        {
            //初始化可接受任务
            foreach (var kv in DataManager.Instance.Quests)
            {
                if (kv.Value.LimitLevel > User.Instance.CurrentCharacterInfo.Level)
                    continue;
                if (kv.Value.LimitClass != 0 && kv.Value.LimitClass != (int)User.Instance.CurrentCharacterInfo.Class)
                    continue;
                if (allQuests.ContainsKey(kv.Key))
                    continue;

                if (kv.Value.PreQuest > 0)
                {
                    Quest newQuest;
                    if (allQuests.TryGetValue(kv.Value.PreQuest, out newQuest))//获取前置任务
                    {
                        if (newQuest.info == null)
                            continue;//没接
                        if (newQuest.info.Status != QuestStatus.Finished)
                            continue;//没完成
                    }
                    else
                    {
                        continue;//没做前置
                    }
                }
                Quest quest = new Quest(kv.Value);
                this.allQuests[quest.define.ID] = quest;
            }
        }

        private void AddNpcQuest(int npcId, Quest quest)
        {
            if (!npcQuests.ContainsKey(npcId))
                npcQuests[npcId] = new Dictionary<NPCQuestStatus, List<Quest>>();

            List<Quest> availables;
            List<Quest> completes;
            List<Quest> incompletes;

            if (!npcQuests[npcId].ContainsKey(NPCQuestStatus.Available))
            {
                availables = new List<Quest>();
                npcQuests[npcId][NPCQuestStatus.Available] = availables;
            }
            if (!npcQuests[npcId].ContainsKey(NPCQuestStatus.Complete))
            {
                completes = new List<Quest>();
                npcQuests[npcId][NPCQuestStatus.Complete] = completes;
            }
            if (!npcQuests[npcId].ContainsKey(NPCQuestStatus.Incomplete))
            {
                incompletes = new List<Quest>();
                npcQuests[npcId][NPCQuestStatus.Incomplete] = incompletes;
            }

            if (quest.info == null)
            {
                if (npcId == quest.define.AcceptNPC && !this.npcQuests[npcId][NPCQuestStatus.Available].Contains(quest))
                    this.npcQuests[npcId][NPCQuestStatus.Available].Add(quest);
            }
            else
            {
                if (npcId == quest.define.SubmitNPC && quest.info.Status == QuestStatus.Completed)
                {
                    if (!this.npcQuests[npcId][NPCQuestStatus.Complete].Contains(quest))
                        this.npcQuests[npcId][NPCQuestStatus.Complete].Add(quest);
                }
                else if (npcId == quest.define.SubmitNPC && quest.info.Status == QuestStatus.InProgress)
                {
                    if (!this.npcQuests[npcId][NPCQuestStatus.Incomplete].Contains(quest))
                        this.npcQuests[npcId][NPCQuestStatus.Incomplete].Add(quest);
                }

            }
        }

        public NPCQuestStatus GetQuestStatusByNpc(int npcId)
        {
            Dictionary<NPCQuestStatus, List<Quest>> status;
            if (npcQuests.TryGetValue(npcId, out status))
            {
                if (status[NPCQuestStatus.Complete].Count > 0)
                    return NPCQuestStatus.Complete;
                if (status[NPCQuestStatus.Available].Count > 0)
                    return NPCQuestStatus.Available;
                if (status[NPCQuestStatus.Incomplete].Count > 0)
                    return NPCQuestStatus.Incomplete;
            }
            return NPCQuestStatus.None;
        }

        public bool OpenQuest(int npcId)
        {
            Dictionary<NPCQuestStatus, List<Quest>> status;
            if (npcQuests.TryGetValue(npcId, out status))
            {
                if (status[NPCQuestStatus.Complete].Count > 0)
                    return ShowQuestDialog(status[NPCQuestStatus.Complete].First());
                if (status[NPCQuestStatus.Available].Count > 0)
                    return ShowQuestDialog(status[NPCQuestStatus.Available].First());
                if (status[NPCQuestStatus.Incomplete].Count > 0)
                    return ShowQuestDialog(status[NPCQuestStatus.Incomplete].First());
            }
            return false;
        }

        bool ShowQuestDialog(Quest quest)
        {
            if (quest.info == null || quest.info.Status == QuestStatus.Completed)
            {
                UIQuestDialog dlg = UIManager.Instance.Show<UIQuestDialog>();
                dlg.gameObject.SetActive(true);
                dlg.SetQuest(quest);
                dlg.OnClose += OnQuestDialogClose;
                return true;
            }
            else if (quest.info != null || quest.info.Status == QuestStatus.Completed)
            {
                if (!string.IsNullOrEmpty(quest.define.DialogIncomplete))
                    MessageBox.Show(quest.define.DialogIncomplete);
            }
            return true;
        }

        void OnQuestDialogClose(UIWindow sender, UIWindow.WindowResult result)
        {
            UIQuestDialog dlg = (UIQuestDialog)sender;
            if (result == UIWindow.WindowResult.Yes)
            {
                if (dlg.quest.info == null)
                {
                    QuestService.Instance.SendQuestAccept(dlg.quest);
                }
                else if(dlg.quest.info.Status == QuestStatus.Completed)
                {
                    QuestService.Instance.SendQuestSubmit(dlg.quest);
                } 
            }
            else if (result == UIWindow.WindowResult.No)
            {
                MessageBox.Show(dlg.quest.define.DialogDeny);
            }
        }

        Quest RefreshQuestStatus(NQuestInfo info)
        {
            npcQuests.Clear();
            Quest result;
            if (allQuests.ContainsKey(info.QuestId))
            {
                allQuests[info.QuestId].info = info;
                result = allQuests[info.QuestId];
            }
            else
            {
                result = new Quest(info);
                allQuests[info.QuestId] = result;
            }

            CheckAvailableQuests();

            foreach (var kv in allQuests)
            {
                this.AddNpcQuest(kv.Value.define.AcceptNPC, kv.Value);
                this.AddNpcQuest(kv.Value.define.SubmitNPC, kv.Value);
            }

            if (onQuestStatusChanged != null)
                onQuestStatusChanged(result);

            return result;
        }

        public void OnQuestAccept(NQuestInfo info)
        {
            Quest quest = RefreshQuestStatus(info);
            MessageBox.Show(quest.define.DialogAccept);
        }

        public void OnQuestSubmit(NQuestInfo info)
        {
            Quest quest = RefreshQuestStatus(info);
            MessageBox.Show(quest.define.DialogFinish);
        }
    }
}
