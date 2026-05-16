using GameServer.Core;
using GameServer.Entities;

namespace GameServer.Pathfinding
{
    internal class PathfindingAgent
    {
        private PathfindingBase _pathfinding;
        private Monster _monster;

        public PathfindingAgent(Monster monster)
        {
            _monster = monster;
            string pfName = monster.Define.Pathfinding;
            if (string.IsNullOrEmpty(pfName))
            {
                pfName = PathfindingStraightLine.ID;
            }

            switch (pfName)
            {
                case PathfindingStraightLine.ID:
                    this._pathfinding = new PathfindingStraightLine(monster);
                    break;
            }
        }

        internal void Update()
        {
            if (this._pathfinding == null) return;
            this._pathfinding.Update();
        }

        internal void MoveTo(Vector3Int position)
        {
            if (this._pathfinding != null)
                this._pathfinding.MoveTo(position);
        }

        internal void StopMove()
        {
            if (this._pathfinding != null)
                this._pathfinding.StopMove();
        }
    }
}
