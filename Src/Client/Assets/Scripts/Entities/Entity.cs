using SkillBridge.Message;
using UnityEngine;
using Utilities;

namespace Entities
{
    public class Entity
    {
        public bool Ready = true;
        public int EntityId;
        public Vector3Int Position;
        public Vector3Int Direction;
        public int Speed;
        public IEntityController Controller { get; set; }
        private NEntity _entityData;

        public NEntity EntityData
        {
            get
            {
                this.UpdateEntityData();
                return _entityData;
            }
            set
            {
                _entityData = value;
                this.SetEntityData(value);
            }
        }
        public Entity(NEntity entity)
        {
            this.EntityId = entity.Id;
            this._entityData = entity;
            this.SetEntityData(entity);
        }

        public virtual void OnUpdate(float delta)
        {
            if (this.Speed != 0)
            {
                Vector3 dir = this.Direction;
                this.Position += Vector3Int.RoundToInt(dir * Speed * delta / 100f);
            }
        }

        public void SetEntityData(NEntity entity)
        {
            this.Position = this.Position.FromNVector3(entity.Position);
            this.Direction = this.Direction.FromNVector3(entity.Direction);
            this.Speed = entity.Speed;
            //LogHelper.Log($"SetEntityData : POS:{this.Position} DIR:{this.Direction} SPD:{this.Speed} ID:{this.EntityId}");

        }
        protected void UpdateEntityData()
        {
            _entityData.Position.FromVector3Int(this.Position);
            _entityData.Direction.FromVector3Int(this.Direction);
            _entityData.Speed = this.Speed;
        }
    }
}
