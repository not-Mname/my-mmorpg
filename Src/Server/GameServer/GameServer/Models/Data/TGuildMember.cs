namespace GameServer.Models.Data
{
    using System;
    
    public class TGuildMember
    {
        public int Id { get; set; }
        public int CharacterId { get; set; }
        public string Name { get; set; }
        public int Class { get; set; }
        public int Level { get; set; }
        public DateTime JoinTime { get; set; }
        public DateTime LastTime { get; set; }
        public int GuildId { get; set; }
        public int TGuildId { get; set; }
        public int Title { get; set; }
    
        public virtual TGuild TGuild { get; set; }
    }
}
