using Models;
using System.Collections.Generic;
using UnityEngine;

namespace UISkill
{
    public class SkillSlots : MonoBehaviour
    {
        public GameObject SkillSlotPrefab;
        private List<SkillSlot> _slots;

        public void Start()
        {
            this._slots = new List<SkillSlot>();
            this.SetData();
        }

        private void SetData()
        {
            foreach (var slot in User.Instance.CurrentCharacterInfo.Skills)
            {
                var slotObj = Instantiate(this.SkillSlotPrefab, this.transform);
                var item = slotObj.GetComponent<SkillSlot>();
                this._slots.Add(slotObj.GetComponent<SkillSlot>());
                item.SetData(slot);
            }
        }

    }
}
