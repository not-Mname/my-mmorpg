using GameServer.Core;
using GameServer.Entities;
using GameServer.Models.Logic;
using SkillBridge.Message;

namespace GameServer.Pathfinding
{
    internal class PathfindingStraightLine : PathfindingBase
    {
        public const string ID = "PathfindingStraightLine";
        private bool _isMoving = false;

        public PathfindingStraightLine(Monster owner) : base(owner)
        {
        }

        internal override void Update()
        {
            if (this._owner.CharacterState == CharacterState.Move)
            {
                if (this._owner.Distance(_moveTarget) < 50)
                {
                    this.StopMove();
                }

                if (this._owner.Speed > 0)
                {
                    Vector3 dir = this._owner.Direction;
                    this._movePosition += dir * this._owner.Speed * Time.DeltaTime / 100f;
                    this._owner.Position = _movePosition;
                }
            }
        }

        internal override void MoveTo(Vector3Int position)
        {
            if (this._owner.Position == position)
            {
                return;
            }
            if (this._owner.CharacterState == CharacterState.Idle)
            {
                this._owner.CharacterState = CharacterState.Move;
                _isMoving = true;
            }
            if (this._moveTarget != position)
            {
                base.MoveTo(position);
                Vector3Int dir = this._moveTarget - this._owner.Position;
                this._owner.Direction = dir.normalizedOnNet;
                this._owner.Speed = this._owner.Define.Speed;

                NEntitySync sync = new NEntitySync()
                {
                    Event = EntityEvent.MoveFwd,
                    Entity = this._owner.EntityData,
                    Id = this._owner.entityId,
                };
                this._owner.Map.UpdateEntity(sync);
            }
        }

        internal override void StopMove()
        {
            if (!_isMoving)
            {
                return;
            }

            if (this._owner.CharacterState == CharacterState.Move)
            {
                this._owner.CharacterState = CharacterState.Idle;
                _isMoving = false;
            }

            this._owner.Speed = 0;
            base.StopMove();

            NEntitySync sync = new NEntitySync()
            {
                Event = EntityEvent.Idle,
                Entity = this._owner.EntityData,
                Id = this._owner.entityId,
            };
            this._owner.Map.UpdateEntity(sync);
        }
    }
}
