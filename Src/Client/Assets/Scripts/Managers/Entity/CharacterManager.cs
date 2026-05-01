using Common;
using Entities;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;
using Utilities;

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

        public void AddCharacter(BattleUnit character)
        {
            LogHelper.LogFormat("AddCharacter:{0}:{1} Map:{2} Entity:{3}", LogUser.CharacterManager, character.EntityId, character.Name, character.Info.MapId, character.Info.Entity.String());
            this.Characters[character.EntityId] = character;
            EntityManager.Instance.AddEntity(character);
            if (OnCharacterEnter != null)
            {
                OnCharacterEnter(character);
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
