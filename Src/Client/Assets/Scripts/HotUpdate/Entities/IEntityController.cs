using Effect;
using SkillBridge.Message;
using UnityEngine;

namespace Entities
{
    public interface IEntityController
    {
        Transform GetTransform();
        void PlayAnim(string animName);
        void PlayEffect(EffectType bullet, string bulletResource, BattleUnit target, float duration);
        void PlayEffect(EffectType type, string name, NVector3 targetPosition, float duration);
        void SetStandby(bool standby);
        void UpdateDirection();
    }
}
