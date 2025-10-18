using Entities;
using Managers;
using Models;
using System;
using UnityEngine.UI;
using Utilities;

namespace UI.MainUI
{
    public class UIMain : MonoSingleton<UIMain>, IDisposable
    {
        /////////////////////////////// UI组件 /////////////////////////
        public Text AvatarName;
        public Text AvatarLevel;
        public UITeam TeamWindow;
        public UIBattleUnitInfo BattleUnitInfo;

        

        /////////////////////////////// 公有函数 ///////////////////////
        protected override void OnStart()
        {
            AvatarName.text = User.Instance.CurrentCharacterInfo.Name;
            AvatarLevel.text = User.Instance.CurrentCharacterInfo.Level.ToString();
            EVENT.Subscribe<BattleUnit>(Const.EventId.on_battle_target_change, OnTargetChange);
            MainPlayerCamera.Instance.ApplyCursorVisibility(false);
        }

        public void OnTargetChange(BattleUnit target)
        {
            if (target != null)
            {
                if (!this.BattleUnitInfo.isActiveAndEnabled)
                {
                    this.BattleUnitInfo.gameObject.SetActive(true);
                }
                BattleUnitInfo.Target = target;
            }
            else
            {
                this.BattleUnitInfo.gameObject.SetActive(false);
            }
        }

        public void Dispose()
        {
            EVENT.Unsubscribe(Const.EventId.on_battle_target_change);
        }

        public void OnClickBag()
        {
            UIManager.Instance.Show<UIBag>();
        }

        public void OnClickEquip()
        {
            UIManager.Instance.Show<UIEquip>();
        }

        public void OnClickQuest()
        {
            UIManager.Instance.Show<UIQuest>();
        }

        public void OnClickFriend()
        {
            UIManager.Instance.Show<UIFriends>();
        }

        public void ShowTeamUI(bool show)
        {
            TeamWindow.ShowTeam(show);
        }

        public void OnCliceGuild()
        {
            GuildManager.Instance.ShowGuild();
        }

        public void OnClickRide()
        {
            UIManager.Instance.Show<UIRide>();
        }

        public void OnClickSetting()
        {
            UIManager.Instance.Show<UISetting>();
        }

        public void OnClickSkill()
        {

        }
    }

}