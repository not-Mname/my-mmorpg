using Const;
using UnityEngine;
using Utilities;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class PlayerEnemyCheck : CharacterEnemyCheckBase
{
    public float MaxDistanceToEnemy;// 超出这个距离就不再检测
    public float MaxAngleToEnemy;// 超出这个角度就不再检测

    protected override void Awake()
    {
        base.Awake();
        
    }

    protected override void Start()
    {
        base.Start();

    }

    public override void Initialize()
    {
        base.Initialize();

        base.Radius = 10f;
        MaxDistanceToEnemy = 10f;
        MaxAngleToEnemy = 10f;

        if (DetectionLayerMask == -1)// 如果没有设置LayerMask，则使用默认的'Enemy'层
        {
            Debug.LogWarning($"{gameObject.name}: LayerMask not set, using default 'Enemy' layer");
            DetectionLayerMask = LayerMask.GetMask("Enemy");
        }
    }

    /// <summary>
    /// 检查是否有敌人, 并锁定目标
    /// 优先判断角度，如果角度小于MaxAngleToEnemy，则锁定距离最近的敌人
    /// 如果角度大于MaxngleToEnemy，则锁定距离最近的敌人
    /// 执行结束会抛出on_player_lock_target事件
    /// </summary>
    public override void Check()
    {
        base.Check();
        GameObject enemy = null;
        if (Count > 0 && CentralPoint != null)
        {
            var angleCollider = CaculateMinAngleCollider();
            var distanceCollider = CaculateMinDistanceCollider();

            if (angleCollider.Item1 != null && angleCollider.Item2 < MaxAngleToEnemy)
            {
                enemy = angleCollider.Item1.gameObject;
            }
            else if( distanceCollider.Item1!= null && distanceCollider.Item2 < MaxDistanceToEnemy)
            {
                enemy = distanceCollider.Item1.gameObject;
            }
        }
        EVENT.Fire(EventId.on_player_lock_target, enemy);
    }

    /// <summary>
    /// 计算屏幕中心到最近的敌人距离的碰撞体
    /// </summary>
    /// <returns>(找到的碰撞体, 最小的角度) 没找到的话Collider为null float为float.MaxValue</returns>
    public (Collider, float) CaculateMinAngleCollider()
    {
        Collider minCollider = null;
        float minAngle = float.MaxValue;

        for (int i = 0; i < Count; i++)
        {
            Vector3 dir = (Buffer[i].transform.position - transform.position).normalized;
            float angle = Vector3.Angle(dir, CentralPoint.forward);
            if (angle < minAngle)
            {
                minAngle = angle;
                minCollider = Buffer[i];
            }

        }

        return (minCollider, minAngle);
    }
}
