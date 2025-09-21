using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SkillBridge.Message;

namespace Entities
{
    public class Entity
    {
        public int EntityId;
        public Vector3Int Position;
        public Vector3Int Direction;
        public int Speed;
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
        }
        private void UpdateEntityData()
        {
            _entityData.Position.FromVector3Int(this.Position);
            _entityData.Direction.FromVector3Int(this.Direction);
            _entityData.Speed = this.Speed;
        }
    }
}
