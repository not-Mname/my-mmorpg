namespace GameServer.Models.Data
{
    public class TCharacterQuest
    {
        public int Id { get; set; }
        public int TCharacterID { get; set; }
        public int QuestID { get; set; }
        public int Target1 { get; set; }
        public int Target2 { get; set; }
        public int Target3 { get; set; }
        public int Status { get; set; }
    
        public virtual TCharacter Owner { get; set; }
    }
}
