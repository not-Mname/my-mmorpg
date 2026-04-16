using UnityEngine;
using Utilities;


/// <summary>
/// 通过关键帧播放特效和声音
/// </summary>
internal class AnimationEventController : MonoBehaviour
{
    public EntityEffectManager EffectManager;

    void PlayEffect(string name)
    {
        EffectManager.PlayEffect(name);
        LogHelper.Log($"AnimationEventController:PlayEffect {name} : {this.name}");
    }

    void PlaySound(string name)
    {
        LogHelper.Log($"AnimationEventController:PlaySound {name} : {this.name}");
    }
}

