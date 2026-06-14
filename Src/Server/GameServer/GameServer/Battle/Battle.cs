using GameServer.Core;
using GameServer.Entities;
using GameServer.Managers.Entities;
using GameServer.Models.Logic;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;

namespace GameServer.GBattle
{
    /// <summary>
    /// 战斗管理器，负责处理战斗逻辑、战斗单位管理和技能释放
    /// </summary>
    class Battle
    {
        public Map Map; // 所属地图

        /// <summary>
        /// 所有参与战斗的单位，Key为实体ID
        /// </summary>
        Dictionary<int, BattleUnit> _allUnits = new Dictionary<int, BattleUnit>();

        /// <summary>
        /// 待处理的技能释放队列
        /// </summary>
        Queue<NSkillCastInfo> _actions = new Queue<NSkillCastInfo>();

        /// <summary>
        /// 死亡单位临时存放池（待移除）
        /// </summary>
        List<BattleUnit> _deathPool = new List<BattleUnit>();

        /// <summary>
        /// 本帧产生的技能命中信息列表，供广播使用
        /// </summary>
        List<NSkillHitInfo> _hits = new List<NSkillHitInfo>();

        /// <summary>
        /// 本帧产生的技能释放信息列表，供广播使用
        /// </summary>
        List<NSkillCastInfo> _casts = new List<NSkillCastInfo>();

        /// <summary>
        /// 本帧产生的Buff信息列表，供广播使用
        /// </summary>
        List<NBuffInfo> _buffActions = new List<NBuffInfo>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="map">所属的地图实例</param>
        public Battle(Map map)
        {
            this.Map = map;
        }

        /// <summary>
        /// 处理客户端发来的战斗消息（技能释放请求）
        /// </summary>
        /// <param name="sender">客户端连接</param>
        /// <param name="message">技能释放请求消息</param>
        public void ProcessBattleMessage(NetConnection<NetSession> sender, SkillCastRequest message)
        {
            Character character = sender.Session.Character;

            // 检查消息中是否包含技能释放信息
            if (message.CastInfo != null)
            {
                // 验证释放者是否为当前角色（防止伪发包）
                if (character.EntityId != message.CastInfo.CasterId)
                {
                    return;
                }

                // 将技能释放信息加入待处理队列
                _actions.Enqueue(message.CastInfo);
            }
        }

        /// <summary>
        /// 战斗更新（每帧调用）
        /// 1. 处理待释放的技能
        /// 2. 更新所有战斗单位状态 包括 技能的更新 buff的更新
        /// </summary>
        public void Update()
        {
            this._casts.Clear();
            this._hits.Clear();
            this._buffActions.Clear();
            // 处理技能释放队列
            if (_actions.Count > 0)
            {
                NSkillCastInfo action = _actions.Dequeue();
                this.ExecuteAction(action);
            }

            // 更新所有战斗单位（处理死亡等状态）
            this.UpdateUnits();

            this.BroadcastHitsMessage();
        }

        /// <summary>
        /// 广播伤害信息给所有客户端
        /// </summary>
        private void BroadcastHitsMessage()
        {
            if (this._hits.Count == 0 && this._buffActions.Count == 0 && this._casts.Count == 0)
            {
                return;
            }

            var message = new NetMessageResponse();
            if (this._casts.Count > 0)
            {
                message.SkillCast = new SkillCastResponse();
                message.SkillCast.CastInfos.AddRange(this._casts);
                message.SkillCast.Result = Result.Success;
                message.SkillCast.Errormsg = "";
            }
            if (this._buffActions.Count > 0)
            {
                message.BuffRes = new BuffResponse();
                message.BuffRes.Buffs.AddRange(this._buffActions);
                message.BuffRes.Result = Result.Success;
                message.BuffRes.Errormsg = "";
            }
            if(this._hits.Count > 0)
            {
                message.SkillHits = new SkillHitResponse();
                message.SkillHits.Hits.AddRange(this._hits);
                message.SkillHits.Result = Result.Success;
                message.SkillHits.Errormsg = "";
            }

            // 广播战斗响应给地图内所有客户端
            this.Map.BroadcastBattleResponse(message);
        }

        /// <summary>
        /// 让战斗单位加入战斗
        /// </summary>
        /// <param name="unit">要加入战斗的单位</param>
        public void JoinBattle(BattleUnit unit)
        {
            this._allUnits[unit.EntityId] = unit;
        }

        /// <summary>
        /// 让战斗单位离开战斗（通常为死亡后）
        /// </summary>
        /// <param name="unit">要离开战斗的单位</param>
        public void LeaveBattle(BattleUnit unit)
        {
            this._allUnits.Remove(unit.EntityId);
        }

        /// <summary>
        /// 更新所有战斗单位状态，并处理死亡单位
        /// </summary>
        private void UpdateUnits()
        {
            // 清空死亡池
            this._deathPool.Clear();

            // 遍历所有单位进行更新
            foreach (var unit in this._allUnits.Values)
            {
                unit.Update();

                // 收集死亡单位
                if (unit.IsDeath)
                {
                    this._deathPool.Add(unit);
                }
            }
            // 将死亡单位移出战斗
            foreach (var unit in this._deathPool)
            {
                this.LeaveBattle(unit);
            }
        }

        /// <summary>
        /// 执行技能释放动作
        /// </summary>
        /// <param name="cast">技能释放信息</param>
        private void ExecuteAction(NSkillCastInfo cast)
        {
            // 创建战斗上下文
            BattleContext context = new BattleContext(this);

            // 获取释放者和目标单位
            context.Caster = EntityManager.Instance.GetUnit(cast.CasterId);
            context.Target = EntityManager.Instance.GetUnit(cast.TargetId);
            context.CastInfo = cast;

            // 确保释放者和目标已加入战斗
            if (context.Caster != null)
            {
                this.JoinBattle(context.Caster);
            }
            if (context.Target != null)
            {
                this.JoinBattle(context.Target);
            }

            // 执行技能
            context.Caster?.CastSkill(context, cast.SkillId);
        }

        internal List<BattleUnit> FindUnitsInRange(Vector3Int pos, int aoeRange)
        {
            List<BattleUnit> result = new List<BattleUnit>();
            foreach (var unit in this._allUnits.Values)
            {
                if(unit.Distance(pos) < aoeRange)
                {
                    result.Add(unit);
                }
            }
            return result;
        }

        internal List<BattleUnit> FindMapUnitsInRange(Vector3Int pos, int aoeRange)
        {
           
            return EntityManager.Instance.FindMapEntitiesInRange<BattleUnit>(Map.Define.ID, Map.InstanceId ,pos, aoeRange);
        }

        internal void AddHitInfo(NSkillHitInfo hitInfo)
        {
            this._hits.Add(hitInfo);
        }

        internal void AddCastInfo(NSkillCastInfo cast)
        {
            this._casts.Add(cast);
        }

        internal void AddBuffAction(NBuffInfo buff)
        {
            this._buffActions.Add(buff);
        }
    }
}
