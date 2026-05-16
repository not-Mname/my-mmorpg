namespace GameServer.Models.Data
{
    public class TCharacterFriend
    {
        public int Id { get; set; }
        public int CharacterID { get; set; }
        public string FriendName { get; set; }
        public int FriendID { get; set; }
        public int Level { get; set; }
        public int Class { get; set; }
    
        public virtual TCharacter Owner { get; set; }
    }
}
