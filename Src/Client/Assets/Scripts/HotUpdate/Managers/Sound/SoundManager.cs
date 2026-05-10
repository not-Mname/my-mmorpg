using Asset;
using AssetBundleFramework;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoSingleton<SoundManager>
{
    public AudioMixer AudioMixer;
    public AudioSource MusicAudioSource;
    public AudioSource SoundAudioSource;

    const string MusicPath = "Assets/AssetBundle/Music/";
    const string SoundPath = "Assets/AssetBundle/Sound/";

    public bool MusicOn
    {
        get
        {
            return _musicOn;
        }
        set
        {
            _musicOn = value;
            this.MusicMute(!_musicOn);
        }
    }
    private bool _musicOn;

    public bool SoundOn
    {
        get
        {
            return _soundOn;
        }
        set
        {
            _soundOn = value;
            this.SoundMute(!_soundOn);
        }
    }
    private bool _soundOn;

    public int SoundVolume
    {
        get
        {
            return _soundVolume;
        }
        set
        {
            _soundVolume = value;
            this.SetVolume("SoundVolume", value);
        }
    }
    private int _soundVolume;

    public int MusicVolume
    {
        get
        {
            return _musicVolume;
        }
        set
        {
            _musicVolume = value;
            this.SetVolume("MusicVolume", value);
        }
    }
    private int _musicVolume;

    public void MusicMute(bool mute)
    {
        this.SetVolume("MusicVolume", mute ? 0 : _musicVolume);
    }
    public void SoundMute(bool mute)
    {
        this.SetVolume("SoundVolume", mute ? 0 : _soundVolume);
    }
    private void SetVolume(string name, int value)
    {
        float volume = value * 0.5f - 50;//如果value是0，则volume为-50分贝相当于静音，如果value是100，则volume为0
        this.AudioMixer.SetFloat(name, volume);
    }

    public void PlaySound(string path)
    {
        IResource res = Resloader.Instance.LoadAssetSync(SoundPath + path);
        AudioClip clip = res.GetAsset<AudioClip>();
        if (clip == null)
        {
            Debug.LogWarningFormat("AudioClip {0} not found", SoundPath + path);
            return;
        }

        SoundAudioSource.PlayOneShot(clip);
    }

    public void PlayMusic(string path)
    {
        IResource res = Resloader.Instance.LoadAssetSync(MusicPath + path);
        AudioClip clip = res.GetAsset<AudioClip>();
        if (clip == null)
        {
            Debug.LogWarningFormat("AudioClip {0} not found", MusicPath + path);
            return;
        }

        if (MusicAudioSource.isPlaying)
        {
            MusicAudioSource.Stop();
        }
        MusicAudioSource.clip = clip;
        MusicAudioSource.Play();
    }
}
