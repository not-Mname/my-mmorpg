using Battle;
using Common;
using Common.Battle;
using GameServer.AI;
using GameServer.Battle;
using GameServer.Core;
using GameServer.Models;
using SkillBridge.Message;

namespace GameServer.Entities
{
    class Monster : BattleUnit
    {
        public Map Map;
        private Vector3Int _moveTarget;
        private Vector3 _movePosition;
        AIAgent _ai;

        public bool IsMoving = false;

        public Monster(int tid, int level, Vector3Int pos, Vector3Int dir) : base(CharacterType.Monster, tid, level, pos, dir)
        {
            this._ai = new AIAgent(this);
        }

        protected override void OnDamage(NDamageInfo damage, BattleUnit source)
        {
            base.OnDamage(damage, source);
            if(this._ai != null)
            {
                this._ai.OnDamage(damage, source);
            }
        }

        public void OnEnterMap(Map map)
        {
            this.Map = map;
        }

        public override void Update()
        {
            base.Update();
            this._ai.Update();
            this.UpdateMovement();
        }

        /// <summary>
        /// 执行具体的移动逻辑，包括移动到目标位置和停止移动等操作
        /// </summary>
        private void UpdateMovement()
        {
            if(this.CharacterState == CharacterState.Move)
            {
                if(Distance(_moveTarget) < 50){
                    this.StopMove();
                }

                if(this.Speed > 0)
                {
                    Vector3 dir = this.Direction;
                    this._movePosition += dir * Speed * Time.DeltaTime / 100f;
                    //Log.Info($"[UpdateMovement]-> Position :{this.Position}  Movement = {dir * Speed * Time.DeltaTime} dir = {dir} Speed = {this.Speed} Time.DeltaTime = {Time.DeltaTime}");
                    this.Position = _movePosition;
                }
            }
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
                if(skillRes == SkillResult.Casting)
                {
                    return null;
                }else if(skillRes == SkillResult.Ok)
                {
                    result = skill;
                }
            }
            return result;
        }

        /// <summary>
        /// 移动到指定位置，如果已经在目标位置则不执行任何操作
        /// </summary>
        /// <param name="position">指定位置</param>
        internal void MoveTo(Vector3Int position)
        {
            if(this.Position == position)
            {
                return;
            }
            if(CharacterState == CharacterState.Idle)
            {
                CharacterState = CharacterState.Move;
                IsMoving = true;
            }
            if(this._moveTarget != position)
            {
                this._moveTarget = position;
                _movePosition = this.Position;
                Vector3Int dir = this._moveTarget - this.Position;
                this.Direction = dir.normalizedOnNet;
                this.Speed = this.Define.Speed;
                Log.Info($"[MoveTo]-> EntityEvent.MoveFwd");
                NEntitySync sync = new NEntitySync()
                {
                    Event = EntityEvent.MoveFwd,
                    Entity = this.EntityData,
                    Id = this.entityId,
                };

                this.Map.UpdateEntity(sync);
            }
        }

        internal void StopMove()
        {
            if (!IsMoving)
            {
                return;
            }

            if(CharacterState == CharacterState.Move)
            {
                CharacterState = CharacterState.Idle;
                IsMoving = false;
            }
            
            this.Speed = 0;
            this._moveTarget = Vector3Int.zero;
            Log.Info($"[MoveTo]-> EntityEvent.Idle");
            NEntitySync sync = new NEntitySync() { 
                Event = EntityEvent.Idle,
                Entity = this.EntityData,
                Id = this.entityId,
            };
            this.Map.UpdateEntity(sync);
        }
    }
}
