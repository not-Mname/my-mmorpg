using UnityEngine;

namespace Battle
{
    internal class Bullet
    {
        private Skill _skill;
        private float _flyTime = 0;
        private int _hit = 0;
        public bool Stoped = false;
        public float Duration;

        public Bullet(Skill skill)
        {
            this._skill = skill;
            var target = skill.Target;
            this._hit = skill.Hit;
            int distance  = skill.Owner.Distance(target);
            this.Duration = distance / skill.Define.BulletSpeed;
        }

        public void Update()
        {
            if (Stoped)
            {
                return;
            }
            this._flyTime += Time.deltaTime;
            if (this._flyTime >= this.Duration)
            {
                this._skill.DoHitDamages(this._hit);
                Stop();
            }
        }

        private void Stop()
        {
            Stoped = true;
        }
    }
}
