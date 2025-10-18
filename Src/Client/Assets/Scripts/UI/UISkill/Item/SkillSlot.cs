using Battle;
using Common.Data;
using Managers;
using Models;
using SkillBridge.Message;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;

namespace UISkill
{
    public class SkillSlot : MonoBehaviour, IPointerClickHandler
    {
        public Image ImgIcon;

        public Image ImgMask;

        public TextMeshProUGUI TMPTimeCD;

        private SkillDefine _skillDefine;

        private Skill _skill;

        void Start()
        {
            this.ImgMask.enabled = false;
            this.TMPTimeCD.enabled = false;
        }


        void FixedUpdate()
        {
            if(this._skill.CD > 0)
            {
                if(!this.ImgMask.enabled) this.ImgMask.enabled = true;
                if(!this.TMPTimeCD.enabled) this.TMPTimeCD.enabled = true;

                int seconds = Mathf.FloorToInt(this._skill.CD);
                this.TMPTimeCD.text = seconds.ToString();
                this.ImgMask.fillAmount = this._skill.CD / this._skillDefine.CD;
            }
            else
            {
                if (this.ImgMask.enabled) this.ImgMask.enabled = false;
                if (this.TMPTimeCD.enabled) this.TMPTimeCD.enabled = false;
            }          
        }

        public void SetData(NSkillInfo skill)
        {
            int chaClass = (int)User.Instance.CurrentCharacterInfo.Class;
            this._skillDefine = DataManager.Instance.Skills[chaClass][skill.Id];
            ImgIcon.sprite = Resloader.Load<Sprite>(this._skillDefine.Icon);
            this._skill = User.Instance.CurrentCharacter.SkillManager.GetSkill(skill.Id);
            this.gameObject.SetActive(true);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SkillResult result = this._skill.CanCast(BattleManager.Instance.CurrentTarget);
            switch (result)
            {
                case SkillResult.Ok:
                    LogHelper.Log("Skill cast: " + this._skill.Define.Name);
                    BattleManager.Instance.CastSkill(this._skill);
                    return;
                case SkillResult.OutOfMp:
                    MessageBox.Show("MP不足");
                    return;
                case SkillResult.CoolDown:
                    MessageBox.Show("技能冷却中");
                    return;
                case SkillResult.InvalidTarget:
                    MessageBox.Show("无效的目标");
                    return;
            }
        }
    }
}
