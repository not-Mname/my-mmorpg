using Models;
using System.Collections.Generic;
using TMPro;
using UI.UISkill.Item;
using UISkill;
using UnityEngine;

namespace UI.UISkill
{
    public class UISkill : UIWindow
    {
        public GameObject SkillPrefab;
        public Transform SkillContainer;
        public TextMeshProUGUI SkillDesc;
        private List<UISkillItem> _skillList;
        private ListView _listView;

        void Start()
        {

        }

        private void Refresh()
        {

        }

        private void Init()
        {
            this._skillList = new List<UISkillItem>();
            foreach (var slot in User.Instance.CurrentCharacterInfo.Skills)
            {
                var slotObj = Instantiate(this.SkillPrefab, this.transform);
                var item = slotObj.GetComponent<UISkillItem>();
                this._skillList.Add(slotObj.GetComponent<UISkillItem>());
                item.SetData(slot);
            }
        }

        private void Clear()
        {
            foreach (var item in this._skillList)
            {
                Destroy(item.gameObject);
            }
        }
    }
}
