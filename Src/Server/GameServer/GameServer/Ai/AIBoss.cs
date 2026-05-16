using GameServer.Entities;


namespace GameServer.AI
{
    internal class AIBoss : AIBase
    {
        public const string ID = "AIBoss";

        public AIBoss(Monster owner) : base(owner)
        {
        }
    }
}
