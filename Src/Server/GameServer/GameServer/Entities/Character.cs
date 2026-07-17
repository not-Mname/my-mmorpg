using Common;
using Common.Data;
using Common.Utils;
using GameServer.Models.Data;
using GameServer.Network;
using Google.Protobuf;
using SkillBridge.Message;
using GameServer.Models.Logic;
using GameServer.Managers.Social;
using GameServer.Managers.Quest;
using GameServer.Managers.Items;
using GameServer.Managers.Entities;
using GameServer.Services.Data;

namespace GameServer.Entities
{
    class Character : BattleUnit, IPostResponser
    {

        public TCharacter Data;

        public ItemManager itemManager;
        public StatusManager statusManager;
        public QuestManager questManager;
        public FriendManager FriendManager;
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
            this.Info.MapId = cha.MapID;
            this.Info.Gold = cha.Gold;
            this.Info.Ride = 0;

            this.Map = MapManager.Instance[this.Data.MapID];

            itemManager = new ItemManager(this);
            itemManager.GetItemInfos(Info.Items);
            statusManager = new StatusManager(this);
            questManager = new QuestManager(this);
            questManager.GetQuestInfos(this.Info.Quests);
            FriendManager = new FriendManager(this);
            FriendManager.GetFriendInfos(this.Info.Friends);
            this.Info.Equips = ByteString.CopyFrom(this.Data.Equips);
            this.Info.Bag = new NBagInfo()
            {
                Items = ByteString.CopyFrom(this.Data.Bag.Items),
                Unlocked = this.Data.Bag.Unlocked
            };
            this.Guild = GuildManager.Instance.GetGuild(this.Data.GuildId);
            if (this.Guild != null)
            {
                this.Guild.timestamp = TimeUtil.timestamp;
            }
            this.Chat = new Chat(this);

            this.Info.Dynamic = new NAttributeDynamic()
            {
                Hp = this.Data.HP,
                Mp = this.Data.MP
            };

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
                using var scope = DBService.Instance.BeginScope();
                var dbChar = DBService.Instance.Entities.Characters.Find(this.Id);
                if (dbChar != null) dbChar.EXP = value;
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
                using var scope = DBService.Instance.BeginScope();
                var dbChar = DBService.Instance.Entities.Characters.Find(this.Id);
                if (dbChar != null) dbChar.Level = value;
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
                using var scope = DBService.Instance.BeginScope();
                var dbChar = DBService.Instance.Entities.Characters.Find(this.Id);
                if (dbChar != null) dbChar.Gold = value;
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
            this.FriendManager.OfflineNotify();
            if (this.Guild != null)
                this.Guild.timestamp = TimeUtil.timestamp;
        }

        public void PostProcess(NetMessage message)
        {
            this.FriendManager.PostProcess(message);
            if (this.Team != null)
            {
                if (TeamUpdateTs < TimeUtil.timestamp)
                {
                    Log.InfoFormat("PostProcess > Team > TeamUpdateTs: {0} | TimeUtil.timestamp: {1}", TeamUpdateTs, TimeUtil.timestamp);
                    TeamUpdateTs = TimeUtil.timestamp;
                    this.Team.PostProcess(message);
                }
            }

            if (this.statusManager.HasStatus)
            {
                Log.InfoFormat("PostProcess > statusManager > TimeUtil.timestamp: {0}", TimeUtil.timestamp);
                statusManager.PostProcess(message);
            }

            if (this.Guild != null)
            {
                if (this.Info.Guild == null)
                {
                    this.Info.Guild = this.Guild.GuildInfo(this);
                    if (message.HasResponse(NetMessageResponse.PayloadOneofCase.MapCharacterEnter) && message.HasResponse(NetMessageResponse.PayloadOneofCase.MapCharacterLeave))
                    {
                        GuildUpdateTs = this.Guild.timestamp;
                    }
                }
                if (GuildUpdateTs < this.Guild.timestamp && !message.HasResponse(NetMessageResponse.PayloadOneofCase.MapCharacterEnter))
                {
                    Log.InfoFormat("PostProcess > Guild > GuildUpdateTs: {0} | TimeUtil.timestamp: {1}", GuildUpdateTs, TimeUtil.timestamp);
                    GuildUpdateTs = this.Guild.timestamp;
                    this.Guild.PostProcess(this, message);
                }
            }

            if (ChatUpdateTs < ChatManager.Instance.timestamp)
            {
                Log.InfoFormat("PostProcess >Char {0} > Chat > ChatUpdateTs: {1} | ChatManager.Instance.timestamp: {2}", this.Id, ChatUpdateTs, ChatManager.Instance.timestamp);
                ChatUpdateTs = ChatManager.Instance.timestamp;
                Chat.PostProcess(message);
            }
        }

        public NCharacterInfo GetBasicInfo()
        {
            return new NCharacterInfo()
            {
                Id = Info.Id,
                Name = Info.Name,
                Class = Info.Class,
                Level = Info.Level,
                EntityId = Info.EntityId,
            };
        }

        internal override void OnEnterMap(Map map)
        {
            base.OnEnterMap(map);
            this.Info.MapId = map.ID;

        }

        internal override void OnLeaveMap(Map map)
        {
            base.OnLeaveMap(map);
            this.Info.MapId = -1;

        }
    }
}
