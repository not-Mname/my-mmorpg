namespace GameServer.Models.Data
{
    using System;
    
    public class TUser
    {
        public long ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime? RegisterDate { get; set; }
        public int Player_ID { get; set; }
    
        public virtual TPlayer Player { get; set; }
    }
}
