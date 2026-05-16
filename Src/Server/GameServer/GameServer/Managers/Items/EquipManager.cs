using Common;
using GameServer.Entities;
using GameServer.Services.Data;
using Network;
using SkillBridge.Message;
using System;

namespace GameServer.Managers.Items
{
    class EquipManager : Singleton<EquipManager>
    {
        public Result EquipItem(NetConnection<NetSession> sender, int slot, int itemId, bool isEquip)
        {
            Character character = sender.Session.Character;
            if (!character.itemManager.items.ContainsKey(itemId)) return Result.Failed;

            using var scope = DBService.Instance.BeginScope();
            var dbChar = DBService.Instance.Entities.Characters.Find(character.Id);
            if (dbChar != null)
            {
                this.UpdateEquip(dbChar.Equips, slot, itemId, isEquip);
                this.UpdateEquip(character.Data.Equips, slot, itemId, isEquip);
            }

            return Result.Success;
        }

        unsafe private void UpdateEquip(byte[] equips, int slot, int itemId, bool isEquip)
        {
            fixed (byte* ptr = equips)
            {
                int* slotId = (int*)(ptr + slot * sizeof(int));
                if (isEquip)
                {
                    *slotId = itemId;
                }
                else
                {
                    *slotId = 0;
                }
            }
        }
    }
}
