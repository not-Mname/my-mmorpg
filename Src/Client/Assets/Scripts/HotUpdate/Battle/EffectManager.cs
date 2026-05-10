using Common;
using Common.Battle;
using Entities;
using System.Collections.Generic;

namespace Battle
{
    public class EffectManager
    {
        private BattleUnit _owner;
        private Dictionary<BuffEffect, int> _effects = new Dictionary<BuffEffect, int>();

        public EffectManager(BattleUnit owner)
        {
            this._owner = owner;
        }

        internal bool HasEffect(BuffEffect effect)
        {
            return _effects.ContainsKey(effect) && _effects[effect] > 0;
        }

        internal void AddBuffEffect(BuffEffect effect)
        {
            Log.Info($"[{_owner.Name}] Add Buff Effect {effect}");
            if (_effects.ContainsKey(effect))
            {
                _effects[effect]++;
            }
            else
            {
                _effects[effect] = 1;
            }
        }

        internal void RemoveBuffEffect(BuffEffect effect)
        {
            Log.Info($"[{_owner.Name}] Remove Buff Effect {effect}");
            if (_effects.ContainsKey(effect) && _effects[effect] > 0)
            {
                _effects[effect]--;
            }
        }
    }
}
