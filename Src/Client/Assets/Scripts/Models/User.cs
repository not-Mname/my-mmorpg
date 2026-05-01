using Common.Data;
using Entities;
using SkillBridge.Message;
using System;

namespace Models
{
    class User : Singleton<User>
    {
        NUserInfo userInfo;

        public NUserInfo Info
        {
            get { return userInfo; }
        }

        public Character CurrentCharacter { get; set; }

        public MapDefine CurrentMapData { get; set; }

        public NCharacterInfo CurrentCharacterInfo { get; set; }

        public PlayerController CurrentCharacterObject { get; set; }

        public NTeamInfo TeamInfo { get; set; }

        public int CurrentRide = 0;
        public void Ride(int id)
        {
            if (id == 0)
            {
                this.CurrentRide = 0;
                CurrentCharacterObject.SendEntityEvent(EntityEvent.Ride, 0);
            }
            else
            {
                this.CurrentRide = id;
                CurrentCharacterObject.SendEntityEvent(EntityEvent.Ride, id);
            }
        }

        internal void Init()
        {
        }

        public void SetupUserInfo(NUserInfo info)
        {
            this.userInfo = info;
        }

        internal void AddGold(int value)
        {
            this.CurrentCharacterInfo.Gold += value;
        }
    }
}
