
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

        public void SetData(int skillID)
        {
            //DataManager.Instance
            ImgIcon.sprite = Resloader.Load<Sprite>("skillicon/" + skillID);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            
        }
    }
}
