using Entities;
using SkillBridge.Message;
using UnityEngine;

namespace Managers
{
    public class BattleManager : Singleton<BattleManager>
    {
        public BattleUnit Target { get; set; }

        public Vector3 Position { get; set; }

    }
}
