using Battle;
using Common.Battle;
using Common.Data;
using Models;
using SkillBridge.Message;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UISkill
{
    public class SkillSlot : MonoBehaviour, IPointerClickHandler
    {
        public Image ImgIcon;

        public Image ImgMask;

        public TextMeshProUGUI TMPTimeCD;

        private SkillDefine _skillDefine;

        private Skill _skill;

        private float targetTime = 0;

        // 每帧更新
        void Update()
        {
            float remaining = targetTime - Time.time;
            this.ImgMask.gameObject.SetActive(remaining > 0);
            this.TMPTimeCD.gameObject.SetActive(remaining > 0);
            if (remaining <= 0)
            {
                return;
            }
            int seconds = Mathf.FloorToInt(remaining);
            this.TMPTimeCD.text = seconds.ToString();
            this.ImgMask.fillAmount = remaining / this._skillDefine.CD;
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
            SkillResult result = this._skill.CanCast();
            switch (result)
            {
                case SkillResult.OK:
                    this._skill.Cast();
                    targetTime = Time.time + this._skillDefine.CD;
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
