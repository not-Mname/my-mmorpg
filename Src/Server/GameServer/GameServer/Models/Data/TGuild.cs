namespace GameServer.Models.Data
{
    using System;
    using System.Collections.Generic;
    
    public class TGuild
    {
        public TGuild()
        {
            this.Members = new HashSet<TGuildMember>();
            this.Applies = new HashSet<TGuildApply>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public int LeaderID { get; set; }
        public string LeaderName { get; set; }
        public string Notice { get; set; }
        public DateTime CreateTime { get; set; }
    
        public virtual ICollection<TGuildMember> Members { get; set; }
        public virtual ICollection<TGuildApply> Applies { get; set; }
    }
}
