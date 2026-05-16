using Common;
using GameInterFace;
using GameServer.Entities;
using GameServer.Services.Data;
using Microsoft.EntityFrameworkCore;
using Network;
using SkillBridge.Message;

namespace GameServer.Services.Items
{
    public class BagService : Singleton<BagService>, IInitializable
    {
        public BagService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<BagSaveRequest>(OnBagSave);
        }

        public void Init()
        {
            Log.Info("BagService Init...");    
        }

        void OnBagSave(NetConnection<NetSession> sender, BagSaveRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("BagSaveRequest：Character {0} Unlocked {1} ", character.Id, message.Bag.Unlocked);

            if(message.Bag == null) return;

            using var scope = DBService.Instance.BeginScope();
            var dbChar = DBService.Instance.Entities.Characters
                .Include(c => c.Bag)
                .First(c => c.ID == character.Id);
            if (dbChar?.Bag != null)
                dbChar.Bag.Items = message.Bag.Items.ToByteArray();
            character.Data.Bag.Items = message.Bag.Items.ToByteArray();
        }
    }
}
