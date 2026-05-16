using Battle;
using GameServer.GBattle;
using GameServer.Entities;
using SkillBridge.Message;
using System;

namespace GameServer.AI
{
    internal class AIAgent
    {
        private AIBase _ai;
        private Monster _monster;

        public AIAgent(Monster monster)
        {
            _monster = monster;
            string aiName = monster.Define.AI;
            if (string.IsNullOrEmpty(aiName))
            {
                aiName = AIMonsterPassive.ID;
            }

            switch (aiName)
            {
                case AIMonsterPassive.ID:
                    this._ai = new AIMonsterPassive(monster);
                    break;
                case AIBoss.ID:
                    this._ai = new AIBoss(monster);
                    break;
            }
        }

        internal void Init()
        {
        }

        internal void OnDamage(NDamageInfo damage, BattleUnit source)
        {
            if(this._ai != null)
            {
                this._ai.OnDamage(damage, source);
            }
        }

        internal void ClearTarget(BattleUnit target)
        {
            if (this._ai != null)
            {
                this._ai.ClearTarget(target);
            }
        }

        internal void Update()
        {
            
            if(this._ai == null) { return; }

            this._ai.Update();
        }

       
    }
}
