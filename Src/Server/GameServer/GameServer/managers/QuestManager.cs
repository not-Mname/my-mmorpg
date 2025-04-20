using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class QuestManager : Singleton<QuestManager>
    {
        Character owner;
        public QuestManager()
        {

        }
        public QuestManager(Character character)
        {
            owner = character;
        }

        public Result AcceptQuest(NetConnection<NetSession> sender, int questId)
        {
            QuestDefine quest;
            if (DataManager.Instance.quests.TryGetValue(questId, out quest))
            {
                var dbQuest = DBService.Instance.Entities.CharacterQuests.Create();
                dbQuest.QuestID = questId;
                if (quest.Target1 == QuestTarget.None)
                {
                    dbQuest.Status = (int)QuestStatus.Completed;
                }
                else
                {
                    dbQuest.Status = (int)QuestStatus.InProgress;
                }
                sender.Session.Response.questAccept.Quest = this.GetQuestInfo(dbQuest);

                sender.Session.Character.Data.Quests.Add(dbQuest);
                DBService.Instance.Save();
                return Result.Success;
            }
            else
            {
                sender.Session.Response.questAccept.Errormsg = "任务不存在";
                return Result.Failed;
            }

        }

        public Result SubmitQuest(NetConnection<NetSession> sender, int questId)
        {
            QuestDefine quest;
            if (DataManager.Instance.quests.TryGetValue(questId, out quest))
            {
                var character = sender.Session.Character;
                var dbQuest = character.Data.Quests.Where(q => q.QuestID == questId).FirstOrDefault();
                if (dbQuest != null)
                {
                    if (dbQuest.Status != (int)QuestStatus.Completed)
                    {
                        sender.Session.Response.questSubmit.Errormsg = "任务未完成";
                        return Result.Failed;
                    }
                    else
                    {
                        dbQuest.Status = (int)QuestStatus.Finished;
                        sender.Session.Response.questSubmit.Quest = this.GetQuestInfo(dbQuest);
                        

                        if (quest.RewardGold > 0)
                        {
                            character.Gold += quest.RewardGold;
                        }

                        if (quest.RewardExp > 0)
                        {
                            //character.Exp += quest.RewardExp;
                        }

                        if (quest.RewardItem1 > 0)
                        {
                            character.itemManager.AddItem(quest.RewardItem1, quest.RewardItem1Count);
                        }
                        if (quest.RewardItem2 > 0)
                        {
                            character.itemManager.AddItem(quest.RewardItem2, quest.RewardItem2Count);
                        }
                        if (quest.RewardItem3 > 0)
                        {
                            character.itemManager.AddItem(quest.RewardItem3, quest.RewardItem3Count);
                        }

                        DBService.Instance.Save();
                        return Result.Success;
                    }
                }
                else
                {
                    sender.Session.Response.questSubmit.Errormsg = "任务不存在[1001]";
                    return Result.Failed;
                }
            }
            else
            {
                sender.Session.Response.questSubmit.Errormsg = "任务不存在[1002]";
                return Result.Failed;
            }


        }

        public void GetQuestInfos(List<NQuestInfo> quests)
        {
            foreach (var quest in owner.Data.Quests)
            {
                quests.Add(this.GetQuestInfo(quest));
            }
        }

        NQuestInfo GetQuestInfo(TCharacterQuest quest)
        {
            return new NQuestInfo()
            {
                QuestId = quest.Id,
                QuestGuid = quest.QuestID,
                Status = (QuestStatus)quest.Status,
                Targets = new int[]
                {
                    quest.Target1,
                    quest.Target2,
                    quest.Target3,
                }
            };
        }
    }
}
