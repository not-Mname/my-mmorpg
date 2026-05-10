using Effect;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityEffectManager : MonoBehaviour
{
    public Transform Root;
    public Transform[] Props;

    public Dictionary<string, GameObject> Effects = new ();
    /// <summary>
    /// 贴身技能特效播放
    /// </summary>
    /// <param name="name"></param>
    internal void PlayEffect(string name)
    {
        if (Effects.ContainsKey(name))
        {
            this.Effects[name].SetActive(true);
        }
    }

    /// <summary>
    /// 子弹特效播放
    /// </summary>
    /// <param name="bullet"></param>
    /// <param name="bulletResource"></param>
    /// <param name="targetTrans"></param>
    /// <param name="duration"></param>
    internal void PlayEffect(EffectType type, string name, Transform target, Vector3 position, float duration)
    {
        if (type == EffectType.Bullet)
        {
            EffectContoller effect = InstantiateEffect(name);
            effect.Init(type, this.transform, target, position, duration);
            effect.gameObject.SetActive(true);
        }
        else
        {
            PlayEffect(name);
        }
    }

    private EffectContoller InstantiateEffect(string name)
    {
        if (this.Effects.TryGetValue(name, out GameObject prefab))
        {
            
            GameObject go = Instantiate(prefab, GameObjectManager.Instance.transform, true);
            go.transform.position = prefab.transform.position;
            go.transform.rotation = prefab.transform.rotation;
            return go.GetComponent<EffectContoller>();
        }
        return null;
    }

    void Start()
    {
        Effects.Clear();
        int count = Root.childCount;
        if (count == 0 || !Root)
        {
            return;
        }
        for (int i = 0; i < count; ++i)
        {
            var effect = Root.GetChild(i);
            Effects.Add(effect.name, effect.gameObject);
        }

        if (Props != null)
        {
            foreach (var prop in Props)
            {
                this.Effects.Add(prop.name, prop.gameObject);
            }
        }
    }

}
