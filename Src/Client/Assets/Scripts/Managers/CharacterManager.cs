using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Network;
using UnityEngine;
using UnityEngine.Events;

using Entities;
using SkillBridge.Message;
using Utilities;
using Models;

namespace Managers
{
    class CharacterManager : Singleton<CharacterManager>, IDisposable
    {
        public Dictionary<int, BattleUnit> Characters = new Dictionary<int, BattleUnit>();
        public UnityAction<BattleUnit> OnCharacterEnter;
        public UnityAction<BattleUnit> OnCharacterLeave;

        public CharacterManager()
        {

        }

        public void Dispose()
        {
        }

        public void Init()
        {

        }

        public void Clear()
        {
            int[] keys = this.Characters.Keys.ToArray();
            foreach (int key in keys)
            {
                this.RemoveCharacter(key);
            }
            this.Characters.Clear();
        }

        public void AddCharacter(SkillBridge.Message.NCharacterInfo cha)
        {
            LogHelper.LogFormat("AddCharacter:{0}:{1} Map:{2} Entity:{3}", LogUser.CharacterManager, cha.EntityId, cha.Name, cha.mapId, cha.Entity.String());
            BattleUnit character = new BattleUnit(cha);
            this.Characters[cha.EntityId] = character;
            EntityManager.Instance.AddEntity(character);
            if (OnCharacterEnter != null)
            {
                OnCharacterEnter(character);
            }
            if (cha.EntityId == User.Instance.CurrentCharacterInfo.EntityId)
            {
                User.Instance.CurrentCharacter = character;
            }
        }


        public void RemoveCharacter(int entityId)
        {
            LogHelper.LogFormat("RemoveCharacter:{0}", LogUser.CharacterManager, entityId);
            if (this.Characters.ContainsKey(entityId))
            {
                EntityManager.Instance.RemoveEntity(this.Characters[entityId].Info.Entity);
                if (OnCharacterLeave != null)
                {
                    OnCharacterLeave(this.Characters[entityId]);
                }
            }
            this.Characters.Remove(entityId);
        }

        public BattleUnit GetCharacter(int id)
        {
            BattleUnit cha = null;
            this.Characters.TryGetValue(id, out cha);
            return cha;
        }
    }
}
