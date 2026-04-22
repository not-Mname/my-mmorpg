using Effect;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 特效管理器
/// 所有不依赖于角色的特效都应该在这里创建
/// </summary>
internal class FXManager : MonoSingleton<FXManager>
{
    public List<GameObject> Prefabs;
    private Dictionary<string, GameObject> _effects = new();

    protected override void OnAwake()
    {
        base.OnAwake();
        for(int i = 0; i < Prefabs.Count; i++)
        {
            Prefabs[i].SetActive(false);
            _effects.Add(Prefabs[i].name, Prefabs[i]);
        }
    }

    /// <summary>
    /// 在世界中创建一个特效
    /// </summary>
    /// <param name="name">特效名</param>
    /// <param name="position">位置</param>
    /// <returns></returns>
    private EffectContoller CreateEffect(string name, Vector3 position)
    {
        if(this._effects.TryGetValue(name, out GameObject effectPrefab))
        {
            GameObject effect = GameObject.Instantiate(effectPrefab, this.transform, true);
            effect.transform.position = position;
            return effect.GetComponent<EffectContoller>();
        }
        return null;
    }

    public void PlayEffect(EffectType type, string name, Transform target, Vector3 position, float duration)
    {
        var effect = CreateEffect(name, position);
        if(effect != null)
        {
            effect.Init(type, transform, target, position, duration);
            effect.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError($"特效 {name} 不存在");
        }
    }
}

