namespace GameServer.Models.Data
{
    public class TCharacterItem
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int ItemCount { get; set; }
        public int TCharacterID { get; set; }
    
        public virtual TCharacter Owner { get; set; }
    }
}
