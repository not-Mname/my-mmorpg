using Common.Data;
using GameServer.Battle;
using GameServer.Entities;
using GameServer.Managers;
using SkillBridge.Message;
using System;

namespace Battle
{
    class Skill
    {
        public NSkillInfo Info { get; set; }
        public BattleUnit Owner { get; set; }
        public SkillDefine Define { get; set; }

        private float _cd;
        public float CD
        {
            get
            {
                return _cd;
            }
        }

        public Skill(NSkillInfo info, BattleUnit owner)
        {
            this.Info = info;
            this.Owner = owner;
            this.Define = DataManager.Instance.Skills[(int)this.Owner.Define.Class][info.Id];
        }

        public SkillResult Cast(BattleContext context)
        {
            SkillResult result = SkillResult.Ok;

            if (this.CD > 0)
            {
                return SkillResult.CoolDown;
            }

            if (context.Target != null)
            {
                this.DoSkillDamge(context);
            }
            

            this._cd = this.Define.CD;
            return result;
        }

        private void DoSkillDamge(BattleContext context)
        {
            context.Damage = new NDamageInfo();
            context.Damage.entityId = context.Target.entityId;
            context.Damage.Damage = 100;
            context.Target.DoDamage(context.Damage);
        }

        internal void Update()
        {
            UpdateCD();
        }

        private void UpdateCD()
        {
            if (this.CD > 0)
            {
                this._cd -= Time.deltaTime;
            }
            else if (this.CD < 0)
            {
                this._cd = 0;
            }
        }
    }
}
