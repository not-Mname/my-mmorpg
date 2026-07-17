using Entities;
using TMPro;
using UI.Common;
using UIFramework;
using UnityEngine.UI;
using Utilities;

namespace UI.MainUI
{

    public class UIBattleUnitInfo : PanelController
    {
        /////////////////////////////// UI组件 /////////////////////////

        public TextMeshProUGUI TextName;
        public Image ImageAvatar;
        public ProgressBar HPBar;
        public ProgressBar MPBar;

        /////////////////////////////// 公有变量 ///////////////////////
        private BattleUnit _target;
        public BattleUnit Target
        {
            get
            {
                return _target;
            }
            set
            {
                _target = value;
                this.Init();
            }
        }

        /////////////////////////////// 私有变量 ///////////////////////

        /////////////////////////////// 公有函数 ///////////////////////

        private void UpdateUI()
        {
            this.HPBar.CurrentValue = this._target.Attributes.HP;
            this.MPBar.CurrentValue = this._target.Attributes.MP;
        }

        /////////////////////////////// 私有函数 ///////////////////////
        private void Init()
        {
            if (this._target != null)
            {
                this.TextName.text = this._target.Name;
                this.HPBar.SetData(this._target.Attributes.MaxHp, this._target.Attributes.HP);
                this.MPBar.SetData(this._target.Attributes.MaxMp, this._target.Attributes.MP);
            }
        }
        private void OnEnable()
        {
            EVENT.Subscribe(Const.EventId.on_battle_target_updata, this.UpdateUI);
        }

        private void OnDisable()
        {
            EVENT.Unsubscribe(Const.EventId.on_battle_target_updata);
        }
    }


}