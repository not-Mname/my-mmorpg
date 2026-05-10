using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildInfo : MonoBehaviour
{

    public Text GuildName;
    public Text GuildID;
    public Text Leader;
    public Text Notice;
    public Text MemberNumber;

    private NGuildInfo _info;
    public NGuildInfo Info
    {
        get { return _info; }
        set
        {
            _info = value;
            this.UpdareUI();
        }


    }
    void UpdareUI()
    {
        if (_info == null)
        {
            this.GuildName.text = "无";
            this.GuildID.text = "ID:0";
            this.Leader.text = "会长:无";
            this.Notice.text = "";
            this.MemberNumber.text = string.Format("成员:0/{0}", 99);

        }
        else
        {
            this.GuildName.text = _info.GuildName;
            this.GuildID.text = string.Format("ID:{0}", _info.Id);
            this.Leader.text = string.Format("会长:{0}", _info.LeaderName);
            this.Notice.text = _info.Notice;
            this.MemberNumber.text = string.Format("成员:{0}/{1}", _info.MemberCount, 99);
        }


    }
}
