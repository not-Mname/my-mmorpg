using Common;
using GameServer.Managers.Entities;
using GameServer.Models.Logic;
using Network;
using SkillBridge.Message;

namespace GameServer.Managers.MBattle
{
    internal class ArenaManager : Singleton<ArenaManager>
    {
        public const int ArenaMapId = 5;
        public const int ArenaMaxInstance = 100;

        Queue<int> InstanceIndexes = new();
        Dictionary<int, Arena> Arenas = new();

        internal void Init()
        {
            for (int i = 0; i < ArenaMaxInstance; ++i)
            {
                InstanceIndexes.Enqueue(i);
            }
        }

        internal Arena NewArena(NArenaInfo arenaInfo, NetConnection<NetSession> red, NetConnection<NetSession> blue)
        {
            int instance = InstanceIndexes.Dequeue();
            var map = MapManager.Instance.GetInstance(ArenaMapId, instance);
            Arena arena = new(arenaInfo, map, red, blue);
            this.Arenas[instance] = arena;
            arena.PlayerEnter();
            return arena;
        }
    }
}
