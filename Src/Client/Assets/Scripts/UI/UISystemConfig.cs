using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISystemConfig : UIWindow
{
    public Image MusicOff;
    public Image SoundOff;

    public Toggle musicToggle;
    public Toggle soundToggle;

    public Slider MusicSlider;
    public Slider SoundSlider;

   void Start()
    {
        this.musicToggle.isOn = Config.MusicOn;
        this.soundToggle.isOn = Config.SoundOn;

        this.MusicSlider.value = Config.MusicVolume;
        this.SoundSlider.value = Config.SoundVolume;
    }

    public override void OnYesClick()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
        PlayerPrefs.Save();
        base.OnYesClick();
    }
    public void MusicToggle(bool on)
    {
        MusicOff.enabled = !musicToggle.isOn;
        Config.MusicOn = musicToggle.isOn;
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
    }
    public void SoundToggle(bool on)
    {
        SoundOff.enabled = !soundToggle.isOn;
        Config.SoundOn = soundToggle.isOn;
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
    }
    public void MusicVolume(float vol)
    {
        Config.MusicVolume = (int)MusicSlider.value;
        PlaySound();
    }
    public void SoundVolume(float vol)
    {
        Config.SoundVolume = (int)SoundSlider.value;
        PlaySound();
    }
    float LastPlat = 0;
    private void PlaySound()
    {
        if(Time.realtimeSinceStartup - LastPlat > 0.1f)
        {
            LastPlat = Time.realtimeSinceStartup;
            SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
        }
    }
}
