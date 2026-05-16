namespace GameServer.Models.Data
{
    using System;
    using System.Collections.Generic;
    
    public class TCharacter
    {
        public TCharacter()
        {
            this.MapID = 1;
            this.Items = new HashSet<TCharacterItem>();
            this.Quests = new HashSet<TCharacterQuest>();
            this.Friends = new HashSet<TCharacterFriend>();
        }
    
        public int ID { get; set; }
        public int TID { get; set; }
        public string Name { get; set; }
        public int Class { get; set; }
        public int MapID { get; set; }
        public int MapPosX { get; set; }
        public int MapPosY { get; set; }
        public int MapPosZ { get; set; }
        public long Gold { get; set; }
        public byte[] Equips { get; set; }
        public int Level { get; set; }
        public long EXP { get; set; }
        public int GuildId { get; set; }
        public int HP { get; set; }
        public int MP { get; set; }
        public int Player_ID { get; set; }
    
        public virtual TPlayer Player { get; set; }
        public virtual ICollection<TCharacterItem> Items { get; set; }
        public virtual TCharacterBag Bag { get; set; }
        public virtual ICollection<TCharacterQuest> Quests { get; set; }
        public virtual ICollection<TCharacterFriend> Friends { get; set; }
    }
}
