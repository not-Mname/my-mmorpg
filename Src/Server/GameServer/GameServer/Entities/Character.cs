using Common;
using Common.Data;
using Common.Utils;
using GameServer.Core;
using GameServer.Managers;
using GameServer.Models;
using GameServer.Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entities
{
    class Character : BattleUnit, IPostResponser
    {

        public TCharacter Data;

        public ItemManager itemManager;
        public StatusManager statusManager;
        public QuestManager questManager;
        public FriendManager friendManager;
        public Team Team;
        public double TeamUpdateTs;

        public Guild Guild;
        public double GuildUpdateTs;

        public Chat Chat;
        public double ChatUpdateTs;
        public Character(CharacterType type, TCharacter cha) :
            base(type, cha.Class, cha.Level, new Core.Vector3Int(cha.MapPosX, cha.MapPosY, cha.MapPosZ), new Core.Vector3Int(100, 0, 0))
        {
            this.Data = cha;
            this.Id = cha.ID;
            this.Info.Id = cha.ID;
            this.Info.Name = cha.Name;
            this.Info.Exp = (int)cha.EXP;
            this.Info.Class = (CharacterClass)cha.Class;
            this.Info.mapId = cha.MapID;
            this.Info.Gold = cha.Gold;
            this.Info.Ride = 0;

            itemManager = new ItemManager(this);
            itemManager.GetItemInfos(Info.Items);
            statusManager = new StatusManager(this);
            questManager = new QuestManager(this);
            questManager.GetQuestInfos(this.Info.Quests);
            friendManager = new FriendManager(this);
            friendManager.GetFriendInfos(this.Info.Friends);
            this.Info.Equips = this.Data.Equips;
            this.Info.Bag = new NBagInfo();
            this.Info.Bag.Items = this.Data.Bag.Items;
            this.Info.Bag.Unlocked = this.Data.Bag.Unlocked;
            this.Guild = GuildManager.Instance.GetGuild(this.Data.GuildId);
            if (this.Guild != null)
            {
                this.Guild.timestamp = TimeUtil.timestamp;
            }
            this.Chat = new Chat(this);

            this.Info.Dynamic = new NAttributeDynamic();
            this.Info.Dynamic.Hp = this.Data.HP;
            this.Info.Dynamic.Mp = this.Data.MP;   
        }

        public void AddExp(int exp)
        {
            this.Exp += exp;
            this.CheckLevelUp();
        }

        //经验公式 EXP = POWER(LV,3) * 10 + LV * 40 + 50
        private void CheckLevelUp()
        {
            long needExp = (long)Math.Pow(this.Level, 3) * 10 + this.Level * 40 + 50;
            if (this.Exp > needExp)
            {
                this.LevelUp();
            }
        }

        private void LevelUp()
        {
            this.Level++;
            Log.InfoFormat("Character {0} Level Up to {1}", this.Info.Name, this.Level);
            this.CheckLevelUp();
        }

        public override List<EquipDefine> GetEquips()
        {

            return base.GetEquips();
        }

        public int Exp
        {
            get
            {
                return (int)this.Data.EXP;
            }
            set
            {
                if (Exp == value) return;
                this.statusManager.AddExpChange((int)(value - this.Data.EXP));
                this.Data.EXP = value;
                this.Info.Exp = value;
            }
        }

        public int Level
        {
            get
            {
                return this.Data.Level;
            }
            set
            {
                if (Level == value) return;
                this.statusManager.AddLevelUp((int)(value - this.Data.Level));
                this.Data.Level = value;
                this.Info.Level = value;
            }
        }

        public long Gold
        {
            get
            {
                return this.Data.Gold;
            }
            set
            {
                if (Gold == value) return;
                this.statusManager.AddGlodChange((int)value - (int)Gold);
                this.Data.Gold = value;
                this.Info.Gold = value;
            }
        }

        public int Ride
        {
            get
            {
                return this.Info.Ride;
            }
            set
            {
                if (Ride == value) return;
                this.Info.Ride = value;
            }
        }

        public void Clear()
        {
            this.friendManager.OfflineNotify();
            if (this.Guild != null)
                this.Guild.timestamp = TimeUtil.timestamp;
        }

        public void PostProcess(NetMessageResponse message)
        {
            //Common.Log.InfoFormat("PostProcess > character : {0} {1}", this.Id, this.Info.Name);
            this.friendManager.PostProcess(message);
            if (this.Team != null)
            {
                if (TeamUpdateTs < TimeUtil.timestamp)
                {
                    //Common.Log.InfoFormat("PostProcess > Team > TeamUpdateTs: {0} | TimeUtil.timestamp: {1}", TeamUpdateTs, TimeUtil.timestamp);
                    TeamUpdateTs = TimeUtil.timestamp;
                    this.Team.PostProcess(message);
                }
            }

            if (this.statusManager.HasStatus)
            {
                statusManager.PostProcess(message);
            }

            if (this.Guild != null)
            {
                if (this.Info.Guild == null)
                {
                    this.Info.Guild = this.Guild.GuildInfo(this);
                    if (message.mapCharacterEnter != null && message.mapCharacterLeave != null)
                        GuildUpdateTs = this.Guild.timestamp;
                }
                if (GuildUpdateTs < this.Guild.timestamp && message.mapCharacterEnter == null)
                {
                    //Common.Log.InfoFormat("PostProcess > Guild > GuildUpdateTs: {0} | TimeUtil.timestamp: {1}", GuildUpdateTs, TimeUtil.timestamp);
                    GuildUpdateTs = this.Guild.timestamp;
                    this.Guild.PostProcess(this, message);
                }
            }

            Common.Log.InfoFormat("PostProcess >Char {0} > Chat > ChatUpdateTs: {1} | ChatManager.Instance.timestamp: {2}", this.Id, ChatUpdateTs, ChatManager.Instance.timestamp);
            Common.Log.InfoFormat("ChatUpdateTs < ChatManager.Instance.timestamp {0}", ChatUpdateTs < ChatManager.Instance.timestamp);
            if (ChatUpdateTs < ChatManager.Instance.timestamp)
            {
                ChatUpdateTs = ChatManager.Instance.timestamp;
                Chat.PostProcess(message);
            }
        }

        public NCharacterInfo GetBsdicInfo()
        {
            return new NCharacterInfo()
            {
                Id = Info.Id,
                Name = Info.Name,
                Class = Info.Class,
                Level = Info.Level,
            };
        }
    }
}
