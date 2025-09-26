using System.Collections.Generic;
using UnityEngine;

namespace UISkill
{
    public class SkillSlots : MonoBehaviour
    {
        public GameObject SkillSlotPrefab;
        private List<SkillSlot> Slots;

        public void Start()
        {
            Slots = new List<SkillSlot>();
        }

        private void Refresh()
        {
            foreach (SkillSlot slot in Slots)
            {
                ;
            }
        }

    }
}
