using GameServer.Core;
using GameServer.Entities;

namespace GameServer.Pathfinding
{
    internal abstract class PathfindingBase
    {
        protected Monster _owner;
        protected Vector3Int _moveTarget;
        protected Vector3 _movePosition;

        public PathfindingBase(Monster owner)
        {
            this._owner = owner;
        }

        internal abstract void Update();

        internal virtual void MoveTo(Vector3Int target)
        {
            this._moveTarget = target;
            this._movePosition = this._owner.Position;
        }

        internal virtual void StopMove()
        {
            this._moveTarget = Vector3Int.zero;
        }
    }
}
