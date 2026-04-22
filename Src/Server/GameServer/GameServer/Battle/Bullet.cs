using Common;
using GameServer.Entities;
using SkillBridge.Message;

namespace Battle
{
    internal class Bullet
    {
        private Skill _skill;
        private NSkillHitInfo _hitInfo;
        private float _duration;
        private bool _timeMode  = true;
        private float _flyTime = 0;
        public bool Stoped = false;

        public Bullet(Skill skill, BattleUnit target, NSkillHitInfo hitInfo)
        {
            this._skill = skill;
            this._hitInfo = hitInfo;
            if (_timeMode)
            {
                int distance = _skill.Owner.Distance(target);
                this._duration = distance / _skill.Define.BulletSpeed;
            }
            Log.Info($"Bullet [{_skill.Define.Name}] from [{_skill.Owner.Name}] to [{target.Name}] duration [{_duration}]");
        }

        public void Update()
        {
            if (Stoped)
            {
                return;
            }

            if (_timeMode)
            {
                UpdateTime();
            }
            else
            {
                UpdatePosition();
            }
        }

        private void UpdatePosition()
        {
        }

        /// <summary>
        /// 计算子弹飞行时间，飞机时间到后，执行命中逻辑
        /// 适用于高速移动的子弹，可以适当放宽子弹命中判定，减少子弹命中逻辑的计算量
        /// </summary>
        private void UpdateTime()
        {
            _flyTime += Time.DeltaTime;
            if(_flyTime > _duration)
            {
                _hitInfo.isBullet = true;
                this._skill.DoHit(_hitInfo);
                Stoped = true;
            }
        }
    }
}
