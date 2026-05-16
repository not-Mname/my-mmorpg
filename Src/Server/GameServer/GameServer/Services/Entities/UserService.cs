using Common;
using GameInterFace;
using GameServer.Entities;
using GameServer.Managers.Data;
using GameServer.Managers.Entities;
using GameServer.Managers.Net;
using GameServer.Models.Data;
using GameServer.Services.Data;
using Microsoft.EntityFrameworkCore;
using Network;
using SkillBridge.Message;

namespace GameServer.Services.Entities
{
    class UserService : Singleton<UserService>, IInitializable, IDisposable
    {
        public void Dispose()
        {
            
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<UserRegisterRequest>(this.OnRegister);
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<UserLoginRequest>(this.OnLogin);
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<UserCreateCharacterRequest>(this.OnCreateCharacter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<UserGameEnterRequest>(this.OnGameEnter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<UserGameLeaveRequest>(this.OnGameLeave);
        }

        public void Init()
        {
            Log.Info("UserService Init...");
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserRegisterRequest>(this.OnRegister);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserLoginRequest>(this.OnLogin);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserCreateCharacterRequest>(this.OnCreateCharacter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameEnterRequest>(this.OnGameEnter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameLeaveRequest>(this.OnGameLeave);
        }

        void OnRegister(NetConnection<NetSession> sender, UserRegisterRequest request)
        {
            Log.InfoFormat("UserRegisterRequest: User:{0}  Pass:{1}", request.User, request.Passward);

            sender.Session.Response.UserRegister = new UserRegisterResponse();

            TUser existingUser = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (existingUser != null)
            {
                sender.Session.Response.UserRegister.Result = Result.Failed;
                sender.Session.Response.UserRegister.Errormsg = "用户已存在.";
            }
            else
            {
                var player = new TPlayer();
                DBService.Instance.Entities.Players.Add(player);
                var newUser = new TUser() { Username = request.User, Password = request.Passward, Player = player };
                DBService.Instance.Entities.Users.Add(newUser);
                DBService.Instance.Entities.SaveChanges();
                sender.Session.Response.UserRegister.Result = Result.Success;
                sender.Session.Response.UserRegister.Errormsg = "None";
            }

            sender.SendResponse();
        }

        void OnLogin(NetConnection<NetSession> sender, UserLoginRequest request)
        {
            Log.InfoFormat("UserLoginRequest: User:{0}  Pass:{1}", request.User, request.Passward);

            sender.Session.Response.UserLogin = new UserLoginResponse();

            TUser user = DBService.Instance.Entities.Users.Include(u => u.Player).ThenInclude(p => p.Characters).Where(u => u.Username == request.User && u.Password == request.Passward).FirstOrDefault();

            if (user == null)
            {
                sender.Session.Response.UserLogin.Result = Result.Failed;
                sender.Session.Response.UserLogin.Errormsg = "用户名或密码错误.";
            }
            else
            {
                sender.Session.Response.UserLogin.Result = Result.Success;
                sender.Session.Response.UserLogin.Errormsg = "None";
                sender.Session.Response.UserLogin.Userinfo = new NUserInfo()
                {
                    Id = (int)user.ID,
                    Player = new NPlayerInfo()
                    {
                        Id = user.Player.ID,
                    },
                };
                foreach (var @char in user.Player.Characters)
                {
                    NCharacterInfo info = new NCharacterInfo();
                    info.Id = @char.ID;
                    info.Name = @char.Name;
                    info.Class = (CharacterClass)@char.Class;
                    info.Type = CharacterType.Player;
                    info.ConfigId = @char.ID;
                    sender.Session.Response.UserLogin.Userinfo.Player.Characters.Add(info);
                }
            }

            sender.Session.User = user;


            sender.SendResponse();
        }

        void OnCreateCharacter(NetConnection<NetSession> sender, UserCreateCharacterRequest request)
        {
            Log.InfoFormat("UserCreateCharacterRequest: Name:{0}  Class:{1}", request.Name, request.Class);
            var config = DataManager.Instance.Characters[(int)request.Class];
            TCharacter character = new TCharacter()
            {
                Name = request.Name,
                Class = (int)request.Class,
                TID = (int)request.Class,
                Level = 1,
                MapID = 1,
                MapPosX = 5000,
                MapPosY = 4200,
                MapPosZ = 800,
                Gold = 1000000,
                HP = (int)config.MaxHP,
                MP = (int)config.MaxMP,
                Equips = new byte[28],
            };

            var bag = new TCharacterBag();
            bag.Items = new byte[0];
            bag.Owner = character;
            bag.Unlocked = 20;
            DBService.Instance.Entities.CharacterBags.Add(bag);
            character.Bag = bag;

            DBService.Instance.Entities.Characters.Add(character);
            sender.Session.User.Player.Characters.Add(character);
            DBService.Instance.Entities.SaveChanges();

            sender.Session.Response.CreateChar = new UserCreateCharacterResponse();

            foreach (var c in sender.Session.User.Player.Characters)
            {
                NCharacterInfo info = new NCharacterInfo();
                info.Id = c.ID;
                info.Name = c.Name;
                info.Class = (CharacterClass)c.Class;
                info.Type = CharacterType.Player;
                info.ConfigId = c.Class;
                sender.Session.Response.CreateChar.Characters.Add(info);
            }

            sender.Session.Response.CreateChar.Errormsg = "None";
            sender.Session.Response.CreateChar.Result = Result.Success;

            sender.SendResponse();
        }

        void OnGameEnter(NetConnection<NetSession> sender, UserGameEnterRequest request)
        {
            sender.Session.Response.GameEnter = new UserGameEnterResponse();

            TCharacter dbChar = DBService.Instance.Entities.Characters
            .Include(c => c.Player)
            .Include(c => c.Bag)
            .Include(c => c.Items)
            .Include(c => c.Quests)
            .Include(c => c.Friends)
            .AsNoTracking()
            .FirstOrDefault(c => c.ID == request.CharacterIdx);
            if (dbChar == null)
            {
                sender.Session.Response.GameEnter.Result = Result.Failed;
                sender.Session.Response.GameEnter.Errormsg = "角色不存在.";
                sender.SendResponse();
                return;
            }

            Log.InfoFormat("UserGameEnterRequest: Name:{0}  Class:{1}  MapId:{2}", dbChar.Name, dbChar.Class, dbChar.MapID);
            Character character = CharacterManager.Instance.AddCharacter(dbChar);
            SessionManager.Instance.AddSession(character.Id, sender);
            sender.Session.Response.GameEnter.Errormsg = "None";
            sender.Session.Response.GameEnter.Result = Result.Success;

            sender.Session.Character = character;
            sender.Session.PostResponser = character;

            sender.Session.Response.GameEnter.Character = character.Info;
            sender.SendResponse();

            MapManager.Instance[dbChar.MapID].CharacterEnter(sender, character);
        }

        void OnGameLeave(NetConnection<NetSession> sender, UserGameLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("UserGameLeaveRequest: {0}", sender.Session.Character.Info.Name);
            sender.Session.Response.GameLeave = new UserGameLeaveResponse();
            CharacterManager.Instance.RemoveCharacter(character.Id);


            sender.Session.Response.GameLeave.Errormsg = "None";
            sender.Session.Response.GameLeave.Result = Result.Success;
            CharacterLeave(character);
            sender.SendResponse();

            MapManager.Instance[sender.Session.Character.Info.MapId].CharacterLeave(sender.Session.Character);
        }

        public void CharacterLeave(Character character)
        {
            SessionManager.Instance.RemoveSession(character.Id);
            CharacterManager.Instance.RemoveCharacter(character.Id);
            character.Clear();
            MapManager.Instance[character.Info.MapId].CharacterLeave(character);
        }


    }
}
