
using Common;
using GameServer.Entities;
using GameServer.Managers.Entities;
using GameServer.Models.Data;
using GameServer.Services.Data;
using Microsoft.EntityFrameworkCore;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GameServer.Managers.Social
{
    class FriendManager
    {
        Character _owner;
        public List<NFriendInfo> Friends = new List<NFriendInfo>();
        bool _friendChanged = false;

        public FriendManager(Character owner)
        {
            _owner = owner;
            InitFriends();
        }
        public void GetFriendInfos(List<NFriendInfo> friends)
        {
            foreach (var friend in Friends)
            {
                friends.Add(friend);
            }
        }
        public NFriendInfo GetFriendInfo(TCharacterFriend friend)
        {
            NFriendInfo FriendInfo = new NFriendInfo();
            var friendCharacter = CharacterManager.Instance.GetCharacter(friend.FriendID);
            FriendInfo.FriendInfo = new NCharacterInfo();
            FriendInfo.Id = friend.Id;
            //Log.InfoFormat("GetFriendInfo  friendCharacter:{0} is Exist:{1}", FriendInfo.FriendInfo.Name, friendCharacter == null);
            if (friendCharacter == null)
            {
                FriendInfo.FriendInfo.Id = friend.FriendID;
                FriendInfo.FriendInfo.Name = friend.FriendName;
                FriendInfo.FriendInfo.Class = (CharacterClass)friend.Class;
                FriendInfo.FriendInfo.Level = friend.Level;
                FriendInfo.Status = 0;//没查到说明不在线
            }
            else
            {
                FriendInfo.FriendInfo = friendCharacter.GetBsdicInfo();
                FriendInfo.FriendInfo.Name = friendCharacter.Data.Name;
                FriendInfo.FriendInfo.Class = (CharacterClass)friendCharacter.Data.Class;
                FriendInfo.FriendInfo.Level = friendCharacter.Data.Level;
                friendCharacter.FriendManager.UpdataFriendInfo(this._owner.Info, 1);
                FriendInfo.Status = 1;
            }
            //Log.InfoFormat("{0}:{1} GetFriendInfo:{2}:{3}}", this._owner.Id, this._owner.Info.Name, FriendInfo.FriendInfo.Id, FriendInfo.FriendInfo.Name);
            return FriendInfo;
        }

        public void InitFriends()
        {
            Friends.Clear();
            foreach (var friend in _owner.Data.Friends)
            {
                // 同步在线好友的实时等级到缓存，供离线展示使用
                var friendChar = CharacterManager.Instance.GetCharacter(friend.FriendID);
                if (friendChar != null && friend.Level != friendChar.Info.Level)
                    friend.Level = friendChar.Info.Level;

                this.Friends.Add(GetFriendInfo(friend));
            }
        }

        public void AddFriend(Character friend)
        {
            TCharacterFriend dbFriend;
            using (var scope = DBService.Instance.BeginScope())
            {
                var dbChar = DBService.Instance.Entities.Characters
                    .Include(c => c.Friends)
                    .First(c => c.ID == this._owner.Id);
                dbFriend = new TCharacterFriend()
                {
                    FriendID = friend.Id,
                    FriendName = friend.Data.Name,
                    Class = friend.Data.Class,
                    Level = friend.Data.Level,
                };
                dbChar.Friends.Add(dbFriend);
            }

            this._owner.Data.Friends.Add(new TCharacterFriend()
            {
                Id = dbFriend.Id,
                FriendID = dbFriend.FriendID,
                FriendName = dbFriend.FriendName,
                Class = dbFriend.Class,
                Level = dbFriend.Level,
            });
            _friendChanged = true;
        }

        public NFriendInfo GetFriendInfo(int friendId)
        {
            foreach (var friend in Friends)
            {
                if (friend.Id == friendId)
                {
                    return friend;
                }
            }
            return null;
        }

        public bool RemoveFriendById(int Id)
        {
            using var scope = DBService.Instance.BeginScope();
            var dbChar = DBService.Instance.Entities.Characters
                .Include(c => c.Friends)
                .First(c => c.ID == this._owner.Id);
            var removed = dbChar.Friends.FirstOrDefault(f => f.CharacterID == Id);
            if (removed != null) dbChar.Friends.Remove(removed);
            var snapshot = this._owner.Data.Friends.FirstOrDefault(f => f.CharacterID == Id);
            if (snapshot != null) this._owner.Data.Friends.Remove(snapshot);
            _friendChanged = true;
            return true;
        }

        public bool RemoveFriendByFriendId(int friendId)
        {
            using var scope = DBService.Instance.BeginScope();
            var dbChar = DBService.Instance.Entities.Characters
                .Include(c => c.Friends)
                .First(c => c.ID == this._owner.Id);
            var removed = dbChar.Friends.FirstOrDefault(f => f.FriendID == friendId);
            if (removed != null) dbChar.Friends.Remove(removed);
            var snapshot = this._owner.Data.Friends.FirstOrDefault(f => f.FriendID == friendId);
            if (snapshot != null) this._owner.Data.Friends.Remove(snapshot);
            _friendChanged = true;
            return true;
        }

        public void UpdataFriendInfo(NCharacterInfo info, int status)
        {
            foreach (var friend in Friends)
            {
                if (friend.FriendInfo.Id == info.Id)
                {
                    var ch = CharacterManager.Instance.GetCharacter(info.Id);
                    friend.Status = status;
                    break;
                }
            }
            _friendChanged = true;
        }

        public void OfflineNotify()
        {
            foreach (var FriendInfo in Friends)
            {
                Log.InfoFormat("{0}:{1} OfflineNotify > {2}:{3}", this._owner.Id, this._owner.Info.Name, FriendInfo.FriendInfo.Id, FriendInfo.FriendInfo.Name);
                var friend = CharacterManager.Instance.GetCharacter(FriendInfo.FriendInfo.Id);
                if (friend != null)
                {
                    friend.FriendManager.UpdataFriendInfo(friend.Info, 0);
                }
            }
        }

        public void PostProcess(NetMessageResponse message)
        {
            if (_friendChanged)
            {
                Log.InfoFormat("{0}:{1} PostProcess > FriendManager", this._owner.Id, this._owner.Info.Name);
                this.InitFriends();
                if (message.FriendList == null)
                {
                    message.FriendList = new FriendListResponse();
                    message.FriendList.Friends.AddRange(this.Friends);
                }
                _friendChanged = false;
            }
        }
    }
}
