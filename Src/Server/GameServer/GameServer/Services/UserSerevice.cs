using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Network;
using SkillBridge.Message;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Models;


namespace GameServer.Services
{
    class UserService : Singleton<UserService>
    {

        public UserService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserRegisterRequest>(this.OnRegister);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserLoginRequest>(this.OnLogin);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserCreateCharacterRequest>(this.OnCreateCharacter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameEnterRequest>(this.OnGameEnter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameLeaveRequest>(this.OnGameLeave);

        }

        public void Init()
        {

        }

        void OnRegister(NetConnection<NetSession> sender, UserRegisterRequest request)
        {
            Log.InfoFormat("UserRegisterRequest: User:{0}  Pass:{1}", request.User, request.Passward);

            sender.Session.Response.userRegister = new UserRegisterResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user != null)
            {
                sender.Session.Response.userRegister.Result = Result.Failed;
                sender.Session.Response.userRegister.Errormsg = "用户已存在.";
            }
            else
            {
                TPlayer player = DBService.Instance.Entities.Players.Add(new TPlayer());
                DBService.Instance.Entities.Users.Add(new TUser() { Username = request.User, Password = request.Passward, Player = player });
                DBService.Instance.Entities.SaveChanges();
                sender.Session.Response.userRegister.Result = Result.Success;
                sender.Session.Response.userRegister.Errormsg = "None";
            }

            sender.SendResponse();
        }

        void OnLogin(NetConnection<NetSession> sender, UserLoginRequest request)
        {
            Log.InfoFormat("UserLoginRequest: User:{0}  Pass:{1}", request.User, request.Passward);

            sender.Session.Response.userLogin = new UserLoginResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User && u.Password == request.Passward).FirstOrDefault();

            if (user == null)
            {
                sender.Session.Response.userLogin.Result = Result.Failed;
                sender.Session.Response.userLogin.Errormsg = "用户名或密码错误.";
            }
            else
            {
                sender.Session.Response.userLogin.Result = Result.Success;
                sender.Session.Response.userLogin.Errormsg = "None";
                sender.Session.Response.userLogin.Userinfo = new NUserInfo();
                sender.Session.Response.userLogin.Userinfo.Id = (int)user.ID;
                sender.Session.Response.userLogin.Userinfo.Player = new NPlayerInfo();
                sender.Session.Response.userLogin.Userinfo.Player.Id = user.Player.ID;
                foreach (var @char in user.Player.Characters)
                {
                    NCharacterInfo info = new NCharacterInfo();
                    info.Id = @char.ID;
                    info.Name = @char.Name;
                    info.Class = (CharacterClass)@char.Class;
                    info.Type = CharacterType.Player;
                    info.ConfigId = @char.ID;
                    sender.Session.Response.userLogin.Userinfo.Player.Characters.Add(info);
                }
            }

            sender.Session.User = user;


            sender.SendResponse();
        }

        void OnCreateCharacter(NetConnection<NetSession> sender, UserCreateCharacterRequest request)
        {
            Log.InfoFormat("UserCreateCharacterRequest: Name:{0}  Class:{1}", request.Name, request.Class);

            TCharacter character = new TCharacter()
            {
                Name = request.Name,
                Class = (int)request.Class,
                TID = (int)request.Class,
                Level = 1,
                MapID = 1,
                MapPosX = 5000,
                MapPosY = 4000,
                MapPosZ = 820,
                Gold = 1000000,
                Equips = new byte[28],
            };

            var bag = new TCharacterBag();
            bag.Items = new byte[0];
            bag.Owner = character;
            bag.Unlocked = 20;
            character.Bag = DBService.Instance.Entities.CharacterBags.Add(bag);

            character = DBService.Instance.Entities.Characters.Add(character);
            sender.Session.User.Player.Characters.Add(character);
            DBService.Instance.Entities.SaveChanges();

            sender.Session.Response.createChar = new UserCreateCharacterResponse();

            foreach (var c in sender.Session.User.Player.Characters)
            {
                NCharacterInfo info = new NCharacterInfo();
                info.Id = c.ID;
                info.Name = c.Name;
                info.Class = (CharacterClass)c.Class;
                info.Type = CharacterType.Player;
                info.ConfigId = c.ID;
                sender.Session.Response.createChar.Characters.Add(info);
            }

            sender.Session.Response.createChar.Errormsg = "None";
            sender.Session.Response.createChar.Result = Result.Success;

            sender.SendResponse();
        }

        void OnGameEnter(NetConnection<NetSession> sender, UserGameEnterRequest request)
        {
            sender.Session.Response.gameEnter = new UserGameEnterResponse();

            TCharacter dbChar = sender.Session.User.Player.Characters.Where(c => request.characterIdx == c.ID).First();

            if (dbChar == null)
            {
                sender.Session.Response.gameEnter.Result = Result.Failed;
                sender.Session.Response.gameEnter.Errormsg = "角色不存在.";
                sender.SendResponse();
                return;
            }

            Log.InfoFormat("UserGameEnterRequest: Name:{0}  Class:{1}  MapID:{2}", dbChar.Name, dbChar.Class, dbChar.MapID);
            Character character = CharacterManager.Instance.AddCharacter(dbChar);
            SessionManager.Instance.AddSession(character.Id, sender);
            sender.Session.Response.gameEnter.Errormsg = "None";
            sender.Session.Response.gameEnter.Result = Result.Success;

            sender.Session.Character = character;
            sender.Session.PostResponser = character;

            sender.Session.Response.gameEnter.Character = character.Info;
            sender.SendResponse();

            MapManager.Instance[dbChar.MapID].CharacterEnter(sender, character);
        }

        void OnGameLeave(NetConnection<NetSession> sender, UserGameLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("UserGameLeaveRequest: {0}", sender.Session.Character.Info.Name);

            sender.Session.Response.gameLeave = new UserGameLeaveResponse();

            CharacterManager.Instance.RemoveCharacter(character.Id);
           
            CharacterLeave(character);
            sender.Session.Response.gameLeave.Errormsg = "None";
            sender.Session.Response.gameLeave.Result = Result.Success;
            
            sender.SendResponse();

            MapManager.Instance[sender.Session.Character.Info.mapId].CharacterLeave(sender.Session.Character);
        }

        public void CharacterLeave(Character character)
        {
            SessionManager.Instance.RemoveSession(character.Id);
            CharacterManager.Instance.RemoveCharacter(character.Id);
            character.Clear();
            MapManager.Instance[character.Info.mapId].CharacterLeave(character);

        }
    }
}
