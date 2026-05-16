namespace GameServer.Models.Data
{
    using System;
    
    public class TGuildApply
    {
        public int Id { get; set; }
        public int CharacterId { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int Result { get; set; }
        public DateTime ApplyTime { get; set; }
        public int GuildId { get; set; }
        public int TGuildId { get; set; }
        public int Class { get; set; }
    
        public virtual TGuild TGuild { get; set; }
    }
}
