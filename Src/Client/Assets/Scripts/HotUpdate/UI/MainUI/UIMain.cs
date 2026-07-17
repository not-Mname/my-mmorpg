using Entities;
using Managers;
using UIFramework;
using Utilities;

namespace UI.MainUI
{
    public class UIMain : MonoSingleton<UIMain>
    {
        /////////////////////////////// UI组件 /////////////////////////
        public UIAvatar Avatar;
        public UITeam TeamWindow;
        public UIBattleUnitInfo BattleUnitInfo;
        public UIFrame uiFrame;
        /////////////////////////////// 公有函数 ///////////////////////
        protected override void OnAwake()
        {
            uiFrame.ShowPanel("Buttons");
            uiFrame.ShowPanel("MiniMap");
            uiFrame.ShowPanel("SkillSlots");
            uiFrame.ShowPanel("UIAvatar");
            uiFrame.ShowPanel("UIChat");
            uiFrame.ShowPanel("UITargetInfo"); 
            uiFrame.ShowPanel("UITeam");
        }

        private void Start()
        {
            Avatar.Init();
            // 订阅战斗目标变化事件，当目标改变时调用OnTargetChange方法
            EVENT.Subscribe<BattleUnit>(Const.EventId.on_battle_target_change, OnTargetChange);
            MainPlayerCamera.Instance?.ApplyCursorVisibility(false);
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

        protected override void OnDestroy()
        {
            // 在对象销毁时取消订阅战斗目标变化事件，避免内存泄漏
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
            UIManager.Instance.Show<UIFriend>();
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