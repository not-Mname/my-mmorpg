using Services;
using SkillBridge.Message;
using System;
using UI.UIArena;
namespace Managers
{
    public class ArenaManager : Singleton<ArenaManager>
    {
        public NArenaInfo ArenaInfo { get; set; }
        public int Round { get; private set; }

        public void EnterArena(NArenaInfo arenaInfo)
        {
            ArenaInfo = arenaInfo;
        }

        public void ExitArena(NArenaInfo arenaInfo)
        {
            ArenaInfo = arenaInfo;
        }

        public void SendReady()
        {
            ArenaService.Instance.SendArenaReadyRequest(this.ArenaInfo.ArenaId);
        }

        public void OnReady(int round, NArenaInfo arenaInfo)
        {
            this.Round = round;
            if(UIArena.Instance != null)
            {
                UIArena.Instance.ShowCountDown();
            }
        }

        internal void OnRoundStart(int round, NArenaInfo arenaInfo)
        {
            if (UIArena.Instance != null) { 
                UIArena.Instance.ShowRoundStart(round, arenaInfo);
            }
        }

        internal void OnRoundEnd(int round, NArenaInfo arenaInfo)
        {
            if (UIArena.Instance != null) { 
                UIArena.Instance.ShowRoundResult(round, arenaInfo);
            }
        }
    }
}
