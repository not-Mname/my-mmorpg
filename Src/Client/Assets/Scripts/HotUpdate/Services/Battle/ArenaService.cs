using GameInterFace;
using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using UnityEngine.Scripting;

namespace Services
{
    [Preserve]
    class ArenaService : Singleton<ArenaService>, IDisposable, IInitializable
    {
        public ArenaService()
        {
            MessageDistributer.Instance.Subscribe<ArenaChallengeRequest>(this.OnArenaChallengeRequest);
            MessageDistributer.Instance.Subscribe<ArenaChallengeResponse>(this.OnArenaChallengeResponse);
            MessageDistributer.Instance.Subscribe<ArenaBeginResponse>(this.OnArenaBeginResponse);
            MessageDistributer.Instance.Subscribe<ArenaEndResponse>(this.OnArenaEndResponse);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ArenaChallengeRequest>(this.OnArenaChallengeRequest);
            MessageDistributer.Instance.Unsubscribe<ArenaChallengeResponse>(this.OnArenaChallengeResponse);
            MessageDistributer.Instance.Unsubscribe<ArenaBeginResponse>(this.OnArenaBeginResponse);
            MessageDistributer.Instance.Unsubscribe<ArenaEndResponse>(this.OnArenaEndResponse);
        }

        public void Init()
        {

        }

        /// <summary>
        /// 发起竞技场挑战请求，发起挑战的玩家是红方，被挑战者是蓝方。
        /// 红方向蓝方发送竞技场挑战请求，蓝方收到后可以选择接受或者拒绝。
        /// </summary>
        /// <param name="targetId">目标entityId</param>
        /// <param name="name">名字</param>
        internal void SendArenaChallengeRequest(int targetId, string name)
        {
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.ArenaChallengeReq = new ArenaChallengeRequest();
            message.Request.ArenaChallengeReq.Info = new NArenaInfo()
            {
                Red = new ArenaPlayer()
                {
                    EntityId = User.Instance.CurrentCharacterInfo.EntityId,
                    Name = User.Instance.CurrentCharacterInfo.Name,
                },
                Blue = new ArenaPlayer()
                {
                    EntityId = targetId,
                    Name = name,
                }

            };
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 回应来自红方的竞技场挑战请求，可以选择接受或者拒绝。
        /// </summary>
        /// <param name="accept"></param>
        /// <param name="request"></param>
        internal void SendArenaChallengeResponse(bool accept, ArenaChallengeRequest request)
        {
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.ArenaChallengeRes = new ArenaChallengeResponse()
            {
                Result = accept ? Result.Success : Result.Failed,
                Errormsg = accept ? "对方接受挑战" : "对方拒绝挑战",
                ArenaInfo = request.Info
            };
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 竞技场结束
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnArenaEndResponse(object sender, ArenaEndResponse message)
        {
            ArenaManager.Instance.ExitArena(message.ArenaInfo);
        }

        /// <summary>
        /// 竞技场开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnArenaBeginResponse(object sender, ArenaBeginResponse message)
        {
            ArenaManager.Instance.EnterArena(message.ArenaInfo);
        }

        /// <summary>
        /// 红方收到蓝方的竞技场挑战回应，这里只处理拒绝的情况。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnArenaChallengeResponse(object sender, ArenaChallengeResponse message)
        {
            if (message.Result == Result.Failed)
            {
                MessageBox.Show(message.Errormsg, "对方拒绝挑战");
            }
        }
        /// <summary>
        /// 蓝方回应红方的竞技场挑战请求，可以选择接受或者拒绝。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnArenaChallengeRequest(object sender, ArenaChallengeRequest message)
        {
            var confirm = MessageBox.Show($"{message.Info.Red.Name}向你发起竞技场挑战请求",
                "竞技场挑战请求", MessageBoxType.Confirm, "接受", "拒绝"
                );
            confirm.OnYes += () => {
                this.SendArenaChallengeResponse(true, message) ;
            };
            confirm.OnNo += () => {
                this.SendArenaChallengeResponse(false, message);
            };
        }


    }
}