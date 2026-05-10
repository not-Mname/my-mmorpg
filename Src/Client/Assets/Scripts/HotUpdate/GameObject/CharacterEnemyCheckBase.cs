using System;
using UnityEngine;
using Utilities;


public class CharacterEnemyCheckBase : MonoBehaviour
{
    [Header("检测设置")]
    public Transform CentralPoint; // 被检测单位的中心点
    public float Radius = 5f; // 默认半径
    public LayerMask DetectionLayerMask = -1; // 明确使用LayerMask类型

    [Header("运行时数据")]
    public Collider[] Buffer = new Collider[32]; // 存储检测到的所有碰撞体
    public int Count; // 检测到的碰撞体数量

    // 是否已经初始化
    public bool IsInitialized { get; private set; }

    protected virtual void Awake()
    {
        Initialize();
    }

    protected virtual void Start()
    {
        // 确保已经初始化
        if (!IsInitialized)
            Initialize();
    }

    protected virtual void OnDestroy()
    {
        Buffer = null;
    }

    /// <summary>
    /// 初始化检测系统
    /// </summary>
    public virtual void Initialize()
    {
        if (CentralPoint == null)
        {
            CentralPoint = transform;
        }

        IsInitialized = true;
    }

    /// <summary>
    /// 设置检测的层级
    /// </summary>
    public virtual void SetDetectionLayer(string layerName)
    {
        DetectionLayerMask = LayerMask.GetMask(layerName);
        IsInitialized = true;
    }

    /// <summary>
    /// 设置检测的层级（直接使用LayerMask）
    /// </summary>
    public virtual void SetDetectionLayer(LayerMask layerMask)
    {
        DetectionLayerMask = layerMask;
        IsInitialized = true;
    }

    public virtual void Check()
    {
        if (!IsInitialized)
        {
            Debug.LogError("CharacterEnemyCheck not initialized! Call Initialize() first.");
            return;
        }

        Count = Physics.OverlapSphereNonAlloc(CentralPoint.position, Radius, Buffer, DetectionLayerMask);
    }

    /// <summary>
    /// 计算最近的碰撞体及其距离
    /// </summary>
    /// <returns>返回符合条件的碰撞体和距离</returns>
    public (Collider, float) CaculateMinDistanceCollider()
    {
        Collider minCollider = null;
        float minDistance = float.MaxValue;
        for (int i = 0; i < Count; i++)
        {
            if (Buffer[i].transform == transform) { continue; }

            float distance = Vector3.Distance(CentralPoint.position, Buffer[i].transform.position);
            if (distance < minDistance)
            {
                minCollider = Buffer[i];
                minDistance = distance;
            }
        }
        return (minCollider, minDistance);
    }

}

