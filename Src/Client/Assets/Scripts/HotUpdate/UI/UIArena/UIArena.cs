using Managers;
using SkillBridge.Message;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace UI.UIArena
{
    internal class UIArena : MonoSingleton<UIArena>
    {
        public TextMeshProUGUI RoundText;
        public TextMeshProUGUI CountDownText;
        private void Start()
        {
            RoundText.enabled = false;
            CountDownText.enabled = false;
            ArenaManager.Instance.SendReady();
        }

        internal void ShowCountDown()
        {
            StartCoroutine(CountDown(10));
        }

        private IEnumerator CountDown(int seconds)
        {
            int total = seconds;
            var wait = new WaitForSeconds(1);
            this.RoundText.enabled = true;
            this.CountDownText.enabled = true;
            while (total > 0)
            {
                SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_CountDown);
                this.CountDownText.text = total.ToString();
                yield return wait;
                total--;
            }
            RoundText.text = "READY";
        }

        internal void ShowRoundResult(int round, NArenaInfo arenaInfo)
        {
            CountDownText.text = "FIGHT";
            // todo: 显示结果
        }

        internal void ShowRoundStart(int round, NArenaInfo arenaInfo)
        {
            this.CountDownText.enabled = true;
            CountDownText.text = "YOU WIN";
        }
    }
}
