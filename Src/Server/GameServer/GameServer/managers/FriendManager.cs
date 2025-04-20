
using Common;
using GameServer.Entities;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GameServer.Managers
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
            NFriendInfo friendInfo = new NFriendInfo();
            var friendCharacter = CharacterManager.Instance.GetCharacter(friend.FriendID);
            friendInfo.friendInfo = new NCharacterInfo();
            friendInfo.Id = friend.Id;
            //Log.InfoFormat("GetFriendInfo  friendCharacter:{0} is Exist:{1}", friendInfo.friendInfo.Name, friendCharacter == null);
            if (friendCharacter == null)
            {
                friendInfo.friendInfo.Id = friend.FriendID;
                friendInfo.friendInfo.Name = friend.FriendName;
                friendInfo.friendInfo.Class = (CharacterClass)friend.Class;
                friendInfo.friendInfo.Level = friend.Level;
                friendInfo.Status = 0;//没查到说明不在线
            }
            else
            {
                friendInfo.friendInfo = friendCharacter.GetBsdicInfo();
                friendInfo.friendInfo.Name = friendCharacter.Data.Name;
                friendInfo.friendInfo.Class = (CharacterClass)friendCharacter.Data.Class;
                friendInfo.friendInfo.Level = friendCharacter.Data.Level;
                if(friend.Level != friendCharacter.Info.Level)
                    friend.Level = friendCharacter.Info.Level;
                friendCharacter.friendManager.UpdataFriendInfo(this._owner.Info, 1);
                friendInfo.Status = 1;
            }
            //Log.InfoFormat("{0}:{1} GetFriendInfo:{2}:{3}}", this._owner.Id, this._owner.Info.Name, friendInfo.friendInfo.Id, friendInfo.friendInfo.Name);
            return friendInfo;
        }

        

        public void InitFriends()
        {
            Friends.Clear();
            foreach (var friend in _owner.Data.Friends)
            {
                this.Friends.Add(GetFriendInfo(friend));
            }
        }
        public void AddFriend(Character friend)
        {
            TCharacterFriend info = new TCharacterFriend()
            {
                FriendID = friend.Id,
                FriendName = friend.Data.Name,
                Class = friend.Data.Class,
                Level = friend.Data.Level,
            };
            this._owner.Data.Friends.Add(info);
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
            var removed = this._owner.Data.Friends.FirstOrDefault(f => f.CharacterID == Id);
            if (removed != null)
                DBService.Instance.Entities.CharacterFriends.Remove(removed);
            _friendChanged = true;
            return true;
        }
        public bool RemoveFriendByFriendId(int friendId)
        {
            var removed = this._owner.Data.Friends.FirstOrDefault(f => f.FriendID == friendId);
            if (removed != null)
                DBService.Instance.Entities.CharacterFriends.Remove(removed);
            _friendChanged = true;
            return true;
        }
        public void UpdataFriendInfo(NCharacterInfo info, int status)
        {
            foreach (var friend in Friends)
            {
                if (friend.friendInfo.Id == info.Id)
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
            foreach (var friendInfo in Friends)
            {
                Log.InfoFormat("{0}:{1} OfflineNotify > {2}:{3}", this._owner.Id, this._owner.Info.Name, friendInfo.friendInfo.Id, friendInfo.friendInfo.Name);
                var friend = CharacterManager.Instance.GetCharacter(friendInfo.friendInfo.Id);
                if (friend != null)
                {
                    friend.friendManager.UpdataFriendInfo(friend.Info, 0);
                }
            }
        }
        public void PostProcess(NetMessageResponse message)
        {
            if (_friendChanged)
            {
                Log.InfoFormat("{0}:{1} PostProcess > FriendManager", this._owner.Id, this._owner.Info.Name);
                this.InitFriends();
                if (message.friendList == null)
                {
                    message.friendList = new FriendListResponse();
                    message.friendList.Friends.AddRange(this.Friends);
                }
                _friendChanged = false;
            }
        }
    }
}
