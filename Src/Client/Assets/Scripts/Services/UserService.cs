using System;
using Common;
using Network;
using UnityEngine;
using SkillBridge.Message;
using Models;
using Managers;

namespace Services
{
    class UserService : Singleton<UserService>, IDisposable
    {
        public UnityEngine.Events.UnityAction<Result, string> OnRegister;
        public UnityEngine.Events.UnityAction<Result, string> OnLogin;
        public UnityEngine.Events.UnityAction<Result, string> OnCreatCharacter;
        public UnityEngine.Events.UnityAction<Result, string> OnGameEnter;
        NetMessage pendingMessage = null;
        bool connected = false;
        bool IsQuit = false;

        public UserService()
        {
            NetClient.Instance.OnConnect += OnGameServerConnect;
            NetClient.Instance.OnDisconnect += OnGameServerDisconnect;
            MessageDistributer.Instance.Subscribe<UserRegisterResponse>(this.OnUserRegister);
            MessageDistributer.Instance.Subscribe<UserLoginResponse>(this.OnUserLogin);
            MessageDistributer.Instance.Subscribe<UserCreateCharacterResponse>(this.OnUserCreatCharacter);
            MessageDistributer.Instance.Subscribe<UserGameEnterResponse>(this.OnUserGameEnter);
            MessageDistributer.Instance.Subscribe<UserGameLeaveResponse>(this.OnUserGameLeave);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<UserRegisterResponse>(this.OnUserRegister);
            MessageDistributer.Instance.Unsubscribe<UserLoginResponse>(this.OnUserLogin);
            MessageDistributer.Instance.Unsubscribe<UserCreateCharacterResponse>(this.OnUserCreatCharacter);
            MessageDistributer.Instance.Unsubscribe<UserGameEnterResponse>(this.OnUserGameEnter);
            MessageDistributer.Instance.Unsubscribe<UserGameLeaveResponse>(this.OnUserGameLeave);
            NetClient.Instance.OnConnect -= OnGameServerConnect;
            NetClient.Instance.OnDisconnect -= OnGameServerDisconnect;
        }

        public void Init()
        {

        }

        public void ConnectToServer()
        {
            Debug.Log("ConnectToServer() Start ");
            //NetClient.Instance.CryptKey = this.SessionId;
            NetClient.Instance.Init("127.0.0.1", 8000);
            NetClient.Instance.Connect();
        }


        void OnGameServerConnect(int result, string reason)
        {
            Log.InfoFormat("LoadingMesager::OnGameServerConnect :{0} reason:{1}", result, reason);
            if (NetClient.Instance.Connected)
            {
                this.connected = true;
                if(this.pendingMessage!=null)
                {
                    NetClient.Instance.SendMessage(this.pendingMessage);
                    this.pendingMessage = null;
                }
            }
            else
            {
                if (!this.DisconnectNotify(result, reason))
                {
                    MessageBox.Show(string.Format("网络错误，无法连接到服务器！\n RESULT:{0} ERROR:{1}", result, reason), "错误", MessageBoxType.Error);
                }
            }
        }

        public void OnGameServerDisconnect(int result, string reason)
        {
            this.DisconnectNotify(result, reason);
            return;
        }

        bool DisconnectNotify(int result,string reason)
        {
            if (this.pendingMessage != null)
            {
                if (this.pendingMessage.Request.userRegister!=null)
                {
                    if (this.OnRegister != null)
                    {
                        this.OnRegister(Result.Failed, string.Format("服务器断开！\n RESULT:{0} ERROR:{1}", result, reason));
                    }
                }
                return true;
            }
            return false;
        }

        #region 发送消息
        public void SendRegister(string user, string psw)
        {
            Debug.LogFormat("UserRegisterRequest::user :{0} psw:{1}", user, psw);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.userRegister = new UserRegisterRequest();
            message.Request.userRegister.User = user;
            message.Request.userRegister.Passward = psw;

            if (this.connected && NetClient.Instance.Connected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }
        }

        public void SendLogin(string user, string psw)
        {
            Debug.LogFormat("UserLoginRequest::user :{0} psw:{1}", user, psw);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.userLogin = new UserLoginRequest();
            message.Request.userLogin.User = user;
            message.Request.userLogin.Passward = psw;

            if(this.connected && NetClient.Instance.Connected)
            {
                pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }
        }

        public void SendCreateCharacter(string name, CharacterClass Class)
        {
            NetMessage message = new NetMessage();
            message.Request = new SkillBridge.Message.NetMessageRequest();
            message.Request.createChar = new UserCreateCharacterRequest();
            message.Request.createChar.Name = name;
            message.Request.createChar.Class = Class;

            if(this.connected && NetClient.Instance.Connected)
            {
                pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }
        }

        public void SendGameEnter(int CharecterId)
        {
            ChatManager.Instance.Init();
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.gameEnter = new UserGameEnterRequest();
            message.Request.gameEnter.characterIdx = CharecterId;
            if (this.connected && NetClient.Instance.Connected)
            {
                pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }
        }

        public void SendGameLeave(bool isQuitGame = false)
        {
            this.IsQuit = isQuitGame;
            MapService.Instance.CurrentMapId = 0;
            NetMessage meg = new NetMessage();
            meg.Request = new NetMessageRequest();
            meg.Request.gameLeave = new UserGameLeaveRequest();

            if(this.connected && NetClient.Instance.Connected)
            {
                pendingMessage = null;
                NetClient.Instance.SendMessage(meg);
            }
            else
            {
                this.pendingMessage = meg;
                this.ConnectToServer();
            }
                
        }
        #endregion

        #region 接受服务器的消息
        void OnUserRegister(object sender, UserRegisterResponse response)
        {
            Debug.LogFormat("OnUserRegister:{0} [{1}]", response.Result, response.Errormsg);

            if (this.OnRegister != null)
            {
                this.OnRegister(response.Result, response.Errormsg);
            }
        }

        void OnUserLogin(object sender, UserLoginResponse response)
        {
            Debug.LogFormat("OnUserLogin:{0} [{1}]", response.Result, response.Errormsg);

            if (this.OnLogin != null)
            {
                this.OnLogin(response.Result, response.Errormsg);
            }

            User.Instance.SetupUserInfo(response.Userinfo);
        }

        void OnUserCreatCharacter(object sender, UserCreateCharacterResponse response)
        {
            Debug.LogFormat("OnUserCreatCharacter:{0} [{1}]", response.Result, response.Errormsg);

            if (this.OnCreatCharacter != null)
            {
                Models.User.Instance.Info.Player.Characters.Clear();
                Models.User.Instance.Info.Player.Characters.AddRange(response.Characters);
                this.OnCreatCharacter(response.Result, response.Errormsg);
            }
        }

        void OnUserGameEnter(object sender, UserGameEnterResponse response)
        {
            Debug.LogFormat("OnUserGameEnter:{0} [{1}]", response.Result, response.Errormsg);

            if(this.OnGameEnter != null)
            {
                this.OnGameEnter(response.Result, response.Errormsg);
                if(response.Character != null)
                {
                    User.Instance.CurrentCharacter = response.Character;
                    ItemManager.Instance.Init(response.Character.Items);
                    BagManager.Instance.Init(response.Character.Bag);
                    EquipManager.Instance.Init(response.Character.Equips);
                    QuestManager.Instance.Init(response.Character.Quests);
                    FriendManager.Instance.Init(response.Character.Friends);
                    GuildManager.Instance.Init(response.Character.Guild);
                   
                }
            }
        }

        void OnUserGameLeave(object sender, UserGameLeaveResponse response)
        {
            Debug.LogFormat("OnUserGameLeave:{0} [{1}]", response.Result, response.Errormsg);
            MapService.Instance.CurrentMapId = 0;
            User.Instance.CurrentCharacter = null;
            if (this.IsQuit)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
               

            }
        }
#endregion
    }
}
