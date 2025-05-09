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
    class Character : CharacterBase, IPostResponser
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
        public Character(CharacterType type, TCharacter cha) :
            base(new Core.Vector3Int(cha.MapPosX, cha.MapPosY, cha.MapPosZ), new Core.Vector3Int(100, 0, 0))
        {
            this.Data = cha;
            this.Id = cha.ID;
            this.Info = new NCharacterInfo();
            this.Info.Id = cha.ID;
            this.Info.Type = type;
            this.Info.EntityId = this.entityId;
            this.Info.Name = cha.Name;
            this.Info.Level = 10;
            this.Info.ConfigId = cha.TID;
            this.Info.Class = (CharacterClass)cha.Class;
            this.Info.mapId = cha.MapID;
            this.Info.Entity = this.EntityData;
            this.Info.Gold = cha.Gold;
            this.Info.Ride = 0;
            if (Define == null)
                this.Define = DataManager.Instance.Characters[this.Info.ConfigId];


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

            this.Chat = new Chat(this);
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
        }

        public void PostProcess(NetMessageResponse message)
        {
            Common.Log.InfoFormat("PostProcess > character : {0} {1}",this.Id, this.Info.Name);
            this.friendManager.PostProcess(message);
            if(this.Team != null)
            {
                if(TeamUpdateTs < TimeUtil.timestamp)
                {
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
                    GuildUpdateTs = this.Guild.timestamp;
                    this.Guild.PostProcess(this, message);
                }
            }

            Chat.PostProcess(message);
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
