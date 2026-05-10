using Common.Data;
using Entities;
using System;
using System.Collections.Generic;

namespace Battle
{
    public class BuffManager
    {
        private BattleUnit _owner;

        public Dictionary<int, Buff> _buffs = new Dictionary<int, Buff>();

        public BuffManager(BattleUnit battleUnit)
        {
            this._owner = battleUnit;
        }

        internal Buff RemoveBuff(int buffId)
        {
            Buff buff = null; 
            if (_buffs.TryGetValue(buffId, out buff))
            {
                buff.OnRemove();
                _buffs.Remove(buffId);
                return buff;
            }
            return buff;
        }

        public void OnUpdate(float delta)
        {
            List<int> needRemove = new();
            foreach (var kv in _buffs)
            {
                kv.Value.Update(delta);
                if (kv.Value.Stopped)
                {
                    needRemove.Add(kv.Key);
                }
            }
            foreach (var id in needRemove)
            {
                this._owner.RemoveBuff(id);
            }
        }

        internal Buff AddBuff(int buffId, int buffType, int casterId)
        {
            if(DataManager.Instance.Buffs.TryGetValue(buffType, out BuffDefine buffDefine))
            {
                Buff buff = new Buff(this._owner, buffId, buffDefine, casterId);
                this._buffs.Add(buffId, buff);
                return buff;
            }
            return null;
        }
    }
}