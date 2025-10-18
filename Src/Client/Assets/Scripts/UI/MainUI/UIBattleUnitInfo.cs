using Entities;
using TMPro;
using UI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainUI
{

    public class UIBattleUnitInfo : MonoBehaviour
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
                this.UpdateUI();
            }
        }

        /////////////////////////////// 私有变量 ///////////////////////

        /////////////////////////////// 公有函数 ///////////////////////
        public void UpdateUI()
        {
            if (this._target != null)
            {
                this.TextName.text = this._target.Name;
                this.HPBar.SetData(this._target.Attributes.MaxHp, this._target.Attributes.HP);
                this.MPBar.SetData(this._target.Attributes.MaxMp, this._target.Attributes.MP);
            }
        }

        /////////////////////////////// 私有函数 ///////////////////////

        private void Start()
        {

        }
    }


}