using GameServer.Entities;


namespace GameServer.AI
{
    internal class AIMonsterPassive: AIBase
    {
        public const string ID = "AIMonsterPassive";

        public AIMonsterPassive(Monster owner) : base(owner)
        {
        }
    }
}
