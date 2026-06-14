using Common;
using SkillBridge.Message;
using GameServer.Entities;
using GameServer.Models.Data;

namespace GameServer.Managers.Entities
{
    class CharacterManager : Singleton<CharacterManager>
    {
        public Dictionary<int, Character> Characters = new();

        public void Init()
        {
            Log.Info("CharacterManager Init...");
        }

        public void Clear()
        {
            this.Characters.Clear();
        }

        public Character AddCharacter(TCharacter cha)
        {
            Character character = new Character(CharacterType.Player, cha);
            EntityManager.Instance.AddEntity(cha.MapID, 0, character);
            character.Info.EntityId = character.EntityId;
            this.Characters[character.Id] = character;
            return character;
        }


        public void RemoveCharacter(int characterId)
        {
            if (this.Characters.ContainsKey(characterId))
            {
                var cha = this.Characters[characterId];
                EntityManager.Instance.RemoveEntity(cha.Info.MapId, cha.Map.InstanceId, cha);//todo:instanceId 不对
                this.Characters.Remove(characterId);
            }
        }

        public Character GetCharacter(int characterId)
        {
            Character character = null;
            this.Characters.TryGetValue(characterId, out character);
            return character;
        }
    }
}
