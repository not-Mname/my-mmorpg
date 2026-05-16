namespace GameServer.Models.Data
{
    using System.Collections.Generic;
    
    public class TPlayer
    {
        public TPlayer()
        {
            this.Characters = new HashSet<TCharacter>();
        }
    
        public int ID { get; set; }
    
        public virtual ICollection<TCharacter> Characters { get; set; }
        public virtual ICollection<TUser> Users { get; set; }
    }
}
