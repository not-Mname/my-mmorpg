using System;
using System.Collections.Generic;
using Battle;
using Common.Battle;
using Common.Data;
using Managers;
using SkillBridge.Message;
using UnityEngine;
using Utilities;

namespace Entities
{
    public class BattleUnit : Entity
    {
        #region 公共属性
        public NCharacterInfo Info;

        public Attributes Attributes;

        public CharacterDefine Define;

        public SkillManager SkillManager;

        public Skill CastringSkill = null;
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
#endregion

        #region 同步
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

        public void CastSkill(int skillId, BattleUnit target, NVector3 position, NDamageInfo damageInfo)
        {
            this.SetStanby(true);
            var skill = this.SkillManager.GetSkill(skillId);
            skill.BeginCast(damageInfo);
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

        public override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);
            this.SkillManager.OnUpdate(delta);
        }

        public void DoDamage(NDamageInfo damageInfo)
        {
            LogHelper.Log($"DoDamage:{damageInfo.Damage}", LogUser.Battle);
            this.Attributes.HP -= damageInfo.Damage;
            this.PlayAnim("Hurt");
            EVENT.Fire(Const.EventId.on_battle_target_updata);
        }

        internal void DoSkillHit(int skillId, int hitId, List<NDamageInfo> damages)
        {
            var skill = this.SkillManager.GetSkill(skillId);
            skill.DoHit(hitId, damages);
        }

        internal int Distance(BattleUnit target)
        {
            return (int)Vector3Int.Distance(this.Position, target.Position);
        }


        #endregion
    }
}
