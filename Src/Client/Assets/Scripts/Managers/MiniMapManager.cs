using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Managers
{
    public class MiniMapManager : Singleton<MiniMapManager>
    {
        public Transform PlayerTransform
        {
            get
            {
                if(User.Instance.CurrentCharacterObject == null) return null;
                return User.Instance.CurrentCharacterObject.transform;
            }
        }

        public UIMiniMap miniMap;

        private Collider _miniMapCollider;
        public Collider MiniMapCollider
        {
            get
            {
                return _miniMapCollider;
            }
        }

        public Sprite LodeCuurentMap()
        {
            return Resloader.Load<Sprite>("UI/MiniMap/" + User.Instance.CurrentMapData.MiniMap);
        }

        public void UpdateMiniMap(Collider collider)
        {
            this._miniMapCollider = collider;
            if (collider != null)
            {
                miniMap.UpdateMap();
            }
        }
    }
}
