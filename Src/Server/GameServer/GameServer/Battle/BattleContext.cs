using GameServer.Core;
using GameServer.Entities;
using SkillBridge.Message;

namespace GameServer.GBattle
{
     class BattleContext
    {
        public Battle Battle;
        public BattleUnit Caster;
        public BattleUnit Target;
        public Vector3Int Position;
        public NSkillCastInfo CastInfo;
        public SkillResult Result;

        public BattleContext(Battle battle)
        {
            this.Battle = battle;
        }
    }
}
