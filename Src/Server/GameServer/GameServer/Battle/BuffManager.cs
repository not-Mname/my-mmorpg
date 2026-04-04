using Common.Data;
using GameServer.Battle;
using GameServer.Entities;
using System;
using System.Collections.Generic;

namespace Battle
{
    internal class BuffManager
    {
        private BattleUnit _owner;

        private List<Buff> _buffs = new List<Buff>();

        private int _buffIndex = 1;
        private int _buffId { get { return _buffIndex++; } }

        public BuffManager(BattleUnit owner)
        {
            _owner = owner;
        }

        internal void AddBuff(BattleContext context, BuffDefine buffDefine)
        {
            Buff buff = new Buff(this._buffId, this._owner, buffDefine, context);
            this._buffs.Add(buff);
        }

        internal void Update()
        {
            foreach (var buff in _buffs)
            {
                if (!buff.Stopped)
                {
                    buff.Update();
                }
            }
            _buffs.RemoveAll(b => b.Stopped);
        }
    }
}
