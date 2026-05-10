using Common.Battle;
using Common.Data;
using Entities;

namespace Battle
{
    public class Buff
    {
        public bool Stopped = false;
        public int BuffId;
        public BuffDefine BuffDefine;
        public float Time;
        private int _hit;
        private BattleUnit _owner;
        private int _casterId;

        public Buff(BattleUnit owner, int buffId, BuffDefine buffDefine, int casterId)
        {
            this._owner = owner;
            this.BuffId = buffId;
            this.BuffDefine = buffDefine;
            this._casterId = casterId;
            this.OnAdd();
        }

        public void OnAdd()
        {
            if(this.BuffDefine.Effect != BuffEffect.None)
            {
                this._owner.AddBuffEffect(this.BuffDefine.Effect);
            }
            AddAttribute();

        }

        public void OnRemove()
        {
            RemoveAttribute();
            this.Stopped = true;
            if (this.BuffDefine.Effect != BuffEffect.None)
            {
                this._owner.RemoveBuffEffect(this.BuffDefine.Effect);
            }
            
        }

        private void AddAttribute()
        {//todo 这里先只写一种情况，后续再扩展
            if (this.BuffDefine.DEFRatio != 0)
            {
                this._owner.Attributes.Buff.DEF += this._owner.Attributes.Basic.DEF * this.BuffDefine.DEFRatio;
            }

            this._owner.Attributes.InitFinalAttribute();
        }

        private void RemoveAttribute()
        {//todo 这里先只写一种情况，后续再扩展
            if (this.BuffDefine.DEFRatio != 0)
            {
                this._owner.Attributes.Buff.DEF -= this._owner.Attributes.Basic.DEF * this.BuffDefine.DEFRatio;
            }
            this._owner.Attributes.InitFinalAttribute();
        }

        internal void Update(float delta)
        {
            if (this.Stopped)
            {
                return;
            }
            Time += UnityEngine.Time.deltaTime;

            if (Time > this.BuffDefine.Duration)
            {
                this.OnRemove();
            }
        }
    }
}