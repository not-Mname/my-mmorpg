using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Managers
{
    /// <summary>
    /// 任务管理器，负责处理玩家任务相关的业务逻辑
    /// </summary>
    class QuestManager : Singleton<QuestManager>
    {
        Character owner; // 当前管理的角色实例

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public QuestManager()
        {

        }

        /// <summary>
        /// 带角色参数的构造函数
        /// </summary>
        /// <param name="character">要管理的角色实例</param>
        public QuestManager(Character character)
        {
            owner = character;
        }

        /// <summary>
        /// 接受任务
        /// </summary>
        /// <param name="sender">网络连接会话</param>
        /// <param name="questId">任务ID</param>
        /// <returns>操作结果：成功或失败</returns>
        public Result AcceptQuest(NetConnection<NetSession> sender, int questId)
        {
            QuestDefine quest;
            // 检查任务是否存在
            if (DataManager.Instance.quests.TryGetValue(questId, out quest))
            {
                // 创建新的任务数据记录
                var dbQuest = DBService.Instance.Entities.CharacterQuests.Create();
                dbQuest.QuestID = questId;
                
                // 检查任务目标，如果无需目标则直接标记为已完成
                if (quest.Target1 == QuestTarget.None)
                {
                    dbQuest.Status = (int)QuestStatus.Completed;
                }
                else
                {
                    dbQuest.Status = (int)QuestStatus.InProgress;
                }
                
                // 设置响应数据
                sender.Session.Response.questAccept.Quest = this.GetQuestInfo(dbQuest);

                // 将任务添加到角色数据中并保存
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

        /// <summary>
        /// 提交完成任务
        /// </summary>
        /// <param name="sender">网络连接会话</param>
        /// <param name="questId">任务ID</param>
        /// <returns>操作结果：成功或失败</returns>
        public Result SubmitQuest(NetConnection<NetSession> sender, int questId)
        {
            QuestDefine quest;
            // 检查任务是否存在
            if (DataManager.Instance.quests.TryGetValue(questId, out quest))
            {
                var character = sender.Session.Character;
                // 查找角色对应的任务记录
                var dbQuest = character.Data.Quests.Where(q => q.QuestID == questId).FirstOrDefault();
                if (dbQuest != null)
                {
                    // 检查任务状态是否为已完成
                    if (dbQuest.Status != (int)QuestStatus.Completed)
                    {
                        sender.Session.Response.questSubmit.Errormsg = "任务未完成";
                        return Result.Failed;
                    }
                    else
                    {
                        // 标记任务为已完成状态
                        dbQuest.Status = (int)QuestStatus.Finished;
                        sender.Session.Response.questSubmit.Quest = this.GetQuestInfo(dbQuest);
                        

                        // 发放任务奖励

                        // 金币奖励
                        if (quest.RewardGold > 0)
                        {
                            character.Gold += quest.RewardGold;
                        }

                        // 经验奖励（目前注释掉了）
                        if (quest.RewardExp > 0)
                        {
                            //character.Exp += quest.RewardExp;
                        }

                        // 物品奖励
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

                        // 保存数据变更
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

        /// <summary>
        /// 获取角色所有任务的信息列表
        /// </summary>
        /// <param name="quests">用于存储任务信息的列表</param>
        public void GetQuestInfos(List<NQuestInfo> quests)
        {
            // 遍历角色所有任务并转换为网络协议格式
            foreach (var quest in owner.Data.Quests)
            {
                quests.Add(this.GetQuestInfo(quest));
            }
        }

        /// <summary>
        /// 将数据库任务数据转换为网络协议格式的任务信息
        /// </summary>
        /// <param name="quest">数据库任务数据</param>
        /// <returns>网络协议格式的任务信息</returns>
        NQuestInfo GetQuestInfo(TCharacterQuest quest)
        {
            return new NQuestInfo() 
            {
                QuestId = quest.QuestID,
                QuestGuid = quest.Id,
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
