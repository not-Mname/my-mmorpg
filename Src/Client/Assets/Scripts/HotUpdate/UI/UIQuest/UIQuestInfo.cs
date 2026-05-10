using Assets.Scripts.Models;
using Managers;
using Models;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestInfo : MonoBehaviour
{
    public Text title;
    public Text description;
    public UIIconItem[] reward;
    public Text rewardGold;
    public Text rewardExp;
    //public Text overView;

    public bool isDiolog = false;

    public Button NavButton;
    private int npc = 0;

    public void SetQuestInfo(Quest quest)
    {
        title.text = quest.define.Name;
        //if(this.overView!= null) this.overView.text = quest.define.Overview;
        if(this.description!= null)
        {
            if (isDiolog)
            {
                description.text = quest.define.Dialog;
            }
            else
            {
                description.text = quest.define.Overview;
            }
        }
        
        rewardGold.text = quest.define.RewardGold.ToString();
        rewardExp.text = quest.define.RewardExp.ToString();

        if(quest.info == null)
            this.npc = quest.define.AcceptNPC;
        else
            this.npc = quest.define.SubmitNPC;

        if(this.NavButton != null)
            this.NavButton.gameObject.SetActive(this.npc > 0);
    }

    public void OnClickAbandon()
    {

    }

    public void OnClickNav()
    {
        Vector3 pos = NPCManager.Instance.GetNPCPostion(this.npc);
        User.Instance.CurrentCharacterObject.StartNav(pos);
        UIManager.Instance.Close<UIQuest>();
    }
}
