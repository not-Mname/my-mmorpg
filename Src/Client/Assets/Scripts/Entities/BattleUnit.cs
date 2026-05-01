using Battle;
using Common.Battle;
using Common.Data;
using Effect;
using Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using Utilities;
using static UnityEngine.GraphicsBuffer;

namespace Entities
{
    public class BattleUnit : Entity
    {
        #region 公共属性
        public NCharacterInfo Info;

        public Attributes Attributes;

        public CharacterDefine Define;

        public SkillManager SkillManager;

        public BuffManager BuffManager;

        public EffectManager EffectManager;

        public Skill CastringSkill = null;

        public Action<Buff> OnBuffAdd;

        public Action<Buff> OnBuffRemove;
        #endregion

        #region getter setter
        private bool _battleState;

        public bool BattleState
        {
            get { return _battleState; }
            set
            {
                if (_battleState != value)
                {
                    _battleState = value;
                    this.SetStanby(value);
                }
            }
        }

        public int Id
        {
            get { return this.Info.Id; }
        }

        public string Name
        {
            get
            {
                if (this.Info.Type == CharacterType.Player)
                    return this.Info.Name;
                else
                    return this.Define.Name;
            }
        }

        public bool IsPlayer
        {
            get { return this.Info.Type == CharacterType.Player; }
        }

        public bool IsCurrentPlayer
        {
            get
            {
                if (!this.IsPlayer) return false;
                return this.Info.Id == Models.User.Instance.CurrentCharacterInfo.Id;
            }
        }
        #endregion

        #region 公共函数

        public BattleUnit(NCharacterInfo info) : base(info.Entity)
        {
            this.Info = info;
            this.Define = DataManager.Instance.Characters[info.ConfigId];
            this.Attributes = new Attributes();
            this.Attributes.Init(this.Define, this.GetEquip(), Info.Level, this.Info.Dynamic);
            this.SkillManager = new SkillManager(this);
            this.BuffManager = new BuffManager(this);
            this.EffectManager = new EffectManager(this);
        }

        public virtual List<EquipDefine> GetEquip()
        {
            return null;
        }

        public void UpdateInfo(NCharacterInfo cha)
        {
            this.SetEntityData(cha.Entity);
            this.Info = cha;
            this.Attributes.Init(this.Define, this.GetEquip(), Info.Level, this.Info.Dynamic);
            this.SkillManager.UpdateSkills();
        }

        internal int Distance(BattleUnit target)
        {
            return (int)Vector3Int.Distance(this.Position, target.Position);
        }
        #endregion

        #region 状态同步
        public void MoveForward()
        {
            //Debug.LogFormat("MoveForward");
            this.Speed = this.Define.Speed;
        }

        public void MoveBack()
        {
            //Debug.LogFormat("MoveBack");
            this.Speed = -this.Define.Speed;
        }

        public void Stop()
        {
            //Debug.LogFormat("Stop");
            this.Speed = 0;
        }

        public void SetDirection(Vector3Int direction)
        {
            //Debug.LogFormat("SetDirection:{0}", direction);
            this.Direction = direction;
        }

        public void SetPosition(Vector3Int position)
        {
            //Debug.LogFormat("SetPosition:{0}", position);
            this.Position = position;
        }

        public void CastSkill(int skillId, BattleUnit target, NVector3 position)
        {
            this.SetStanby(true);
            var skill = this.SkillManager.GetSkill(skillId);
            skill.BeginCast(target, position);
        }

        private void SetStanby(bool standby)
        {
            if (this.Controller != null)
            {
                this.Controller.SetStandby(standby);
            }
        }

        public void PlayAnim(string animName)
        {
            if (this.Controller != null)
            {
                this.Controller.PlayAnim(animName);
            }
        }
        #endregion

        #region 战斗逻辑
        public override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);
            this.SkillManager.OnUpdate(delta);
            this.BuffManager.OnUpdate(delta);

        }

        public void DoDamage(NDamageInfo damageInfo, bool playHurt = true)
        {
            LogHelper.Log($"DoDamage: [name : {this.Name}] [critical : {damageInfo.Critical}] [damage : {damageInfo.Damage}]", LogUser.Battle);
            this.Attributes.HP -= damageInfo.Damage;
            if (playHurt) { this.PlayAnim("Hurt"); }
            if (this.Controller != null) { UIWouldElementManager.Instance.AddPopupText(PopupType.Damage, this.Controller.GetTransform().position + this.GetPopupOffset(), -damageInfo.Damage, damageInfo.Critical ); }
            
            EVENT.Fire(Const.EventId.on_battle_target_updata);
        }

        internal void DoSkillHit(NSkillHitInfo hit)
        {
            var skill = this.SkillManager.GetSkill(hit.SkillId);
            skill.DoHit(hit);
        }

        internal void DoBuffAction(NBuffInfo buff)
        {
            LogHelper.Log($"DoBuffAction: [name : {this.Name}] [BuffId : {buff.BuffId}] [action : {buff.Action}]", LogUser.Battle);
            switch (buff.Action)
            {
                case BuffAction.Add:
                    this.AddBuff(buff.BuffId, buff.BuffType, buff.CasterId);
                    break;
                case BuffAction.Remove:
                    this.RemoveBuff(buff.BuffId);
                    break;
                case BuffAction.Hit:
                    this.DoDamage(buff.Damage, false);
                    break;
            }
        }

        public void RemoveBuff(int BuffId)
        {
            var buff = this.BuffManager.RemoveBuff(BuffId);
            if (buff != null)
            {
                OnBuffRemove?.Invoke(buff);
            }
        }

        private void AddBuff(int BuffId, int buffType, int casterId)
        {
            var buff = this.BuffManager.AddBuff(BuffId, buffType, casterId);
            OnBuffAdd?.Invoke(buff);
        }

        internal void AddBuffEffect(BuffEffect effect)
        {
            this.EffectManager.AddBuffEffect(effect);
        }

        internal void RemoveBuffEffect(BuffEffect effect)
        {
            this.EffectManager.RemoveBuffEffect(effect);
        }

        /// <summary>
        /// 面向目标位置
        /// </summary>
        /// <param name="position"></param>
        internal void FaceTo(Vector3Int position)
        {
            this.SetDirection(GameObjectTool.WorldToLogic(GameObjectTool.LogicToWorld(position - this.Position)));
            this.UpdateEntityData();
            if(this.Controller != null)
            {
                this.Controller.UpdateDirection();
            }
        }

        /// <summary>
        /// 对一个目标播放特效
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="target"></param>
        /// <param name="duration"></param>
        internal void PlayEffect(EffectType type, string name, BattleUnit target, float duration = 0)
        {
                if (string.IsNullOrEmpty(name))
                {
                    return;
                }
            if(this.Controller != null)
            {
                this.Controller.PlayEffect(type, name, target, duration);
            }
        }

        /// <summary>
        /// 在一个位置播放特效
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="targetPosition"></param>
        internal void PlayEffect(EffectType type, string name, NVector3 targetPosition)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }
            if (this.Controller != null)
            {
                this.Controller.PlayEffect(type, name, targetPosition, 0);
            }
        }

        internal Vector3 GetHitOffset()
        {
            return new Vector3(0, this.Define.Height * 0.8f, 0);
        }

        internal Vector3 GetPopupOffset()
        {
            return new Vector3(0, this.Define.Height, 0);
        }

        #endregion
    }
}
