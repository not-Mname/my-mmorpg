using Battle;
using Common;
using Common.Battle;
using GameServer.AI;
using GameServer.GBattle;
using GameServer.Core;
using GameServer.Models.Logic;
using GameServer.Pathfinding;
using SkillBridge.Message;

namespace GameServer.Entities
{
    class Monster : BattleUnit
    {
        AIAgent _ai;
        PathfindingAgent _pathfinding;

        public Monster(int tid, int level, Vector3Int pos, Vector3Int dir) : base(CharacterType.Monster, tid, level, pos, dir)
        {
            this._ai = new AIAgent(this);
            this._pathfinding = new PathfindingAgent(this);
        }

        protected override void OnDamage(NDamageInfo damage, BattleUnit source)
        {
            base.OnDamage(damage, source);
            if(this._ai != null)
            {
                this._ai.OnDamage(damage, source);
            }
        }

        internal override void OnEnterMap(Map map)
        {
            base.OnEnterMap(map);
            this._ai.Init();
        }

        internal override void OnLeaveMap(Map map)
        {
            base.OnLeaveMap(map);
        }

        public override void Update()
        {
            base.Update();
            this._ai.Update();
            this._pathfinding.Update();
        }

        /// <summary>
        /// 移动到指定位置（委托给寻路代理）
        /// </summary>
        internal void MoveTo(Vector3Int position)
        {
            this._pathfinding?.MoveTo(position);
        }

        internal void StopMove()
        {
            this._pathfinding?.StopMove();
        }

        /// <summary>
        /// 查找可以释放的技能，支持多种技能类型组合，用位运算来判断技能类型是否匹配
        /// </summary>
        /// <param name="context">释放技能的战斗上下文</param>
        /// <param name="type">位运算匹配项</param>
        /// <returns></returns>
        public Skill FindSkill(BattleContext context, SkillType type)
        {
            Skill result = null;
            foreach(var skill in this.SkillManager.Skills)
            {
                if(skill.Define.Type != (type & skill.Define.Type))
                {
                    continue;
                }

                SkillResult skillRes = skill.CanCast(context);
                if(skillRes == SkillResult.SkillCasting)
                {
                    return null;
                }else if(skillRes == SkillResult.Ok)
                {
                    result = skill;
                }
            }
            return result;
        }

    }
}
