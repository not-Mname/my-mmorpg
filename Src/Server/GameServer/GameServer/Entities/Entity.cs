using GameServer.Core;
using GameServer.Models.Logic;
using SkillBridge.Message;

namespace GameServer.Entities
{
    class Entity
    {
        public int EntityId
        {
            get { return this.entityData.Id; }
            private set { this.entityData.Id = value; }
        }

        private Vector3Int position;

        public Vector3Int Position
        {
            get { return position; }
            set {
                position = value;
                this.entityData.Position = position;
            }
        }

        private Vector3Int direction;

        public Vector3Int Direction
        {
            get { return direction; }
            set
            {
                direction = value;
                this.entityData.Direction = direction;
            }
        }

        private int speed;

        public int Speed
        {
            get { return speed; }
            set
            {
                speed = value;
                this.entityData.Speed = speed;
            }
        }

        private NEntity entityData;
        public bool ready = true;

        public NEntity EntityData
        {
            get
            {
                return entityData;
            }
            set
            {
                entityData = value;
                this.SetEntityData(value);
            }
        }

        public Entity(Vector3Int pos,Vector3Int dir)
        {
            this.entityData = new NEntity();
            this.entityData.Position = pos;
            this.entityData.Direction = dir;
            this.SetEntityData(this.entityData);
        }

        public Map Map;

        public Entity(NEntity entity)
        {
            SetEntityData(entity);
        }

        public void SetEntityData(NEntity entity)
        {
            if(!ready){ return; }
            this.EntityId = entity.Id;
            this.entityData = entity;
            this.Position = entity.Position;
            this.Direction = entity.Direction;
            this.speed = entity.Speed;
        }

        public virtual void Update()
        {
            
        }


        internal virtual void OnEnterMap(Map map)
        {
            this.Map = map;
        }

        internal virtual void OnLeaveMap(Map map)
        {
            this.Map = null;
        }
    }
}
