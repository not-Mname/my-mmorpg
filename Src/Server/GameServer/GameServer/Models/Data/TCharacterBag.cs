namespace GameServer.Models.Data
{
    public class TCharacterBag
    {
        public int Id { get; set; }
        public byte[] Items { get; set; }
        public int Unlocked { get; set; }
        public int Owner_ID { get; set; }
    
        public virtual TCharacter Owner { get; set; }
    }
}
