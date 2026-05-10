using Battle;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBuffItem : MonoBehaviour
{
    public Image BuffIcon;
    public TextMeshProUGUI Time;
    public Image Mask;
    private Buff _buff;


    internal void Init(Buff buff)
    {
        _buff = buff;
    }
    void Update()
    {
        if (this._buff.Time > 0)
        {
            if (!this.Mask.enabled) this.Mask.enabled = true;
            if (!this.Time.enabled) this.Time.enabled = true;

            int seconds = Mathf.FloorToInt(this._buff.BuffDefine.Duration - this._buff.Time);
            this.Time.text = seconds.ToString();
            this.Mask.fillAmount = seconds / this._buff.BuffDefine.Duration;
        }
        else
        {
            if (this.Mask.enabled) this.Mask.enabled = false;
            if (this.Time.enabled) this.Time.enabled = false;
        }
    }
}
