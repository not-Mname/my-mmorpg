using Effect;
using System.Collections;
using UnityEngine;

/// <summary>
/// 通过代码控制的特效
/// </summary>
internal class EffectContoller :MonoBehaviour
{
    public float lifeTime = 2f;
    float _time = 0;

    EffectType _type;
    Transform _target;

    Vector3 _targetPos;
    Vector3 _startPos;
    Vector3 _offset;

    void OnEnable()
    {
        if (_type != EffectType.Bullet)
        {//子弹特效不延迟销毁
            StartCoroutine(Run());
        }
    }

    /// <summary>
    /// 追踪型子弹特效
    /// </summary>
    void Update()
    {
        if (_type == EffectType.Bullet)
        {
            this._time += Time.deltaTime;
            if(this._target != null)
            {
                this._targetPos = _target.position + _offset;
            }
            this.transform.LookAt(_targetPos);
            if (Vector3.Distance(transform.position, _targetPos) < 0.5f)
            {
                Destroy(gameObject);
                return;
            }
            if (this.lifeTime > 0 && _time >= lifeTime)
            {
                Destroy(gameObject);
                return;
            }
            transform.position = Vector3.Lerp(transform.position, _targetPos, Time.deltaTime / (lifeTime  - _time));
        }
    }

    IEnumerator Run() { 
        yield return new WaitForSeconds(lifeTime);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 初始化特效参数
    /// 子弹类型需要传入发射位置和目标位置，其他类型只需要传入持续时间
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="source">当type 为 bullet 时子弹发射位置</param>
    /// <param name="target">当type 为 bullet 时子弹目标位置</param>
    /// <param name="offer">目标位置偏移，以求攻击打中目标胸口位置</param>
    /// <param name="duration">持续时间</param>
    public void Init(EffectType type, Transform source, Transform target, Vector3 offer, float duration)
    {
        this._type = type;
        this._target = target;
        this.lifeTime = duration > 0 ? duration : lifeTime;
        this._time = 0;
        if(type == EffectType.Bullet)
        {
            this._startPos = transform.position;
            this._offset = offer;
            this._targetPos = target.position + offer;
        }
        else if(type == EffectType.Hit)
        {
            this.transform.position = target.position + offer;
        }
    }
}


