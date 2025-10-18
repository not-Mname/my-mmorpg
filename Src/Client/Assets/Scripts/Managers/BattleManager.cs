using Battle;
using Entities;
using Services;
using SkillBridge.Message;
using Utilities;

namespace Managers
{
    public class BattleManager : Singleton<BattleManager>
    {
        private BattleUnit _currentTarget;
        public BattleUnit CurrentTarget
        {
            get
            {
                return this._currentTarget;
            }
            set
            {
                this.SetTarget(value);

            }
        }

        private NVector3 _currentPosition;
        public NVector3 CurrentPosition
        {
            get
            {
                return this._currentPosition;
            }
            set
            {
                this.SetPosition(value);

            }
        }

        public void Init()
        {
        }

        private void SetPosition(NVector3 value)
        {
            this._currentPosition = value;
        }

        private void SetTarget(BattleUnit target)
        {
            if (this._currentTarget != target)
            {
                EVENT.Fire(Const.EventId.on_battle_target_change, target);
            }
                this._currentTarget = target;
        }

        public void CastSkill(Skill skill)
        {
            int target = this._currentTarget != null ? this._currentTarget.Id : 0;
            BattleService.Instance.SendSkillCast(skill.Define.ID, skill.Owner.EntityId, target, this._currentPosition);
        }
    }
}
