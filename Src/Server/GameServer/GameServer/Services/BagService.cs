using Common;
using GameServer.Entities;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    public class BagService : Singleton<BagService>
    {
        public BagService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<BagSaveRequest>(OnBagSave);
        }

        public void Init()
        {

        }

        void OnBagSave(NetConnection<NetSession> sender, BagSaveRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("BagSaveRequest：Character {0} Unlocked {1} ", character.Id, message.Bag.Unlocked);

            if(message.Bag == null) return;

            character.Data.Bag.Items = message.Bag.Items;
            DBService.Instance.Save();
        }
    }
}
