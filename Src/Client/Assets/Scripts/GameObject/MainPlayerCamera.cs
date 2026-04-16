using Const;
using Managers;
using Models;
using System;
using UnityEngine;
using Utilities;

public class MainPlayerCamera : MonoSingleton<MainPlayerCamera>
{
    [SerializeField]
    private Camera _camera;
    public Transform FollowPoint;
    public GameObject Follower;

    [Header("Lazy Follow Settings")]
    public float baseDistance = 4f;       // 理想距离
    public float pullStrength = 0.3f;     // 回拉强度

    private Transform _cameraTransform;
    public bool isCursorVisible = false; // 初始隐藏
    public float height = 1.5f;
    public float xSpeed = 250;
    public float ySpeed = 125;
    public float yMinLimit = -50;
    public float yMaxLimit = 72;
    public float yaw = 20;//偏航角（左右看）
    public float pitch = 0;//俯仰角（上下看）
    public float targetHeightOffset = 3f;// 锁定时相机上抬的目标偏移量
    public float minPitch = -30f;   // 最多向下看 30 度（不能太低头）
    public float maxPitch = 60f;    // 最多向上看 60 度（不能太仰头）

    public float normalDistance = 4;
    public float minCameraDistance = 1f;
    public float maxCameraDistance = 10f;
    public float currentDistance;
    public float followSpeed = 5;

    #region getters and setters
    private Transform _targetPoint;

    private GameObject _target;
    public GameObject Target
    {
        get
        {
            return _target;
        }
        set
        {

            _target = value ? value : null;
            _targetPoint = value ? value.transform : null;
        }
    }

  

    #endregion

    #region 生命周期
    protected override void OnAwake()
    {
        this._camera = this.GetComponentInChildren<Camera>();
        currentDistance = normalDistance;

    }
    void OnEnable()
    {
        EVENT.Subscribe<string>(EventId.on_map_change, OnMapChange);
    }

    private void OnDisable()
    {
        EVENT.Unsubscribe(EventId.on_map_change);
    }
    void LateUpdate()
    {
        if (!CheckAndInit())
        {
            return;
        }
        UpdateInput();
        UpdatePositionAndDirection();

    }
    #endregion

    #region 私有
    public void Init()
    {
        if (Follower == null) return;
        this.FollowPoint = Follower.transform;
        this._cameraTransform = this.gameObject.transform;
        this._cameraTransform.forward = this.FollowPoint.forward;
        currentDistance = normalDistance;
    }

    /// <summary>
    /// 在登录和角色选择界面，禁用主摄像机并释放鼠标
    /// </summary>
    /// <param name="mapName">地图名</param>
    void OnMapChange(string mapName)
    {
        LogHelper.Log("OnMapChange: " + mapName);
        if (mapName == "Loading" || mapName == "CharSelect")
        {
            this.gameObject.SetActive(false);
            ApplyCursorVisibility(true);
        }
        else
        {
            this.gameObject.SetActive(true);
            ApplyCursorVisibility(false);
        }
    }

    /// <summary>
    /// 核心方法，提供锁定敌人和跟随玩家两种模式
    /// </summary>
    private void UpdatePositionAndDirection()
    {
        if (!isCursorVisible)
        {
            Quaternion rotation = this._cameraTransform.rotation;

            if (this._targetPoint == null)
            {//如果未锁定，根据鼠标输入构建旋转
                rotation = Quaternion.Euler(pitch, yaw, 0f);
            }
            else
            {//如果锁定敌人
                //float distance = (this.FollowPoint.position - this.Target.transform.position).magnitude;
                //if (distance > 20f)
                //{
                //    EVENT.Fire(EventId.on_player_lock_target, null);
                //}
                //设置目标瞄准点（可加上高度偏移）
                Vector3 targetAimPoint = _targetPoint.transform.position + Vector3.up * targetHeightOffset;

                //计算从相机到目标的方向
                Vector3 dir = targetAimPoint - this.transform.position;

                //只使用水平方向来计算 Yaw（绕 Y 轴旋转）
                Vector3 horizontalDir = dir;
                horizontalDir.y = 0; // 忽略 Y 分量，只保留水平方向

                float distanceToTarget = dir.magnitude;

                if (horizontalDir.sqrMagnitude < 0.01f || distanceToTarget < 0.1f)
                    return;

                //左右旋转
                Quaternion yawRotation = Quaternion.LookRotation(horizontalDir, Vector3.up);

                //使用 atan2 计算摄像机指向目标向量与水平地面之间的夹角
                float pitchAngle = Mathf.Atan2(dir.y, new Vector2(dir.x, dir.z).magnitude) * Mathf.Rad2Deg;
                pitchAngle = Mathf.Clamp(pitchAngle, minPitch, maxPitch);

                rotation = yawRotation * Quaternion.Euler(pitchAngle, 0, 0);
            }

            this._cameraTransform.rotation = rotation;
            currentDistance = (FollowPoint.position - this._cameraTransform.position).magnitude;
            currentDistance = Mathf.Clamp(currentDistance, minCameraDistance, maxCameraDistance);
            //Debug.Log($"UpdatePositionAndDirection : {currentDistance}");
            Vector3 targetPosition = FollowPoint.position - rotation * Vector3.forward * normalDistance;
            targetPosition.y += height;
            Vector3 finnalPosition = Vector3.Lerp(this._cameraTransform.position, targetPosition, Time.deltaTime * followSpeed);
            transform.position = finnalPosition;
        }
    }

    private void UpdateInput()
    {
        if (!isCursorVisible)
        {
            yaw += InputManager.Instance.MouseValueX * xSpeed * Time.deltaTime;
            pitch -= InputManager.Instance.MouseValueY * ySpeed * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, yMinLimit, yMaxLimit);
        }
    }

    /// <summary>
    /// 检测是否需要更新位置和旋转
    /// </summary>
    /// <returns></returns>
    private bool CheckAndInit()
    {
        if (InputManager.Instance.AltPressed)
        {//如果按下alt切换指针锁定状态
            isCursorVisible = !isCursorVisible;
            ApplyCursorVisibility(isCursorVisible);
        }

        if (Follower == null && User.Instance.CurrentCharacterObject != null)
        {// 如果跟随者为空 当前角色不为空 尝试赋值
            Follower = User.Instance.CurrentCharacterObject.gameObject;
            Init();
        }

        if (FollowPoint == null && Follower != null)
        {//如果跟随点为空尝试赋值
            this.FollowPoint = Follower.transform;
        }

        if (Follower == null || _camera == null || FollowPoint == null)
        {//所有尝试失败 说明角色未创建
            return false;
        }

        return true;
    }
    #endregion

    #region 公开

    /// <summary>
    /// 锁定或释放指针
    /// </summary>
    /// <param name="visible"></param>
    public void ApplyCursorVisibility(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        this.isCursorVisible = visible;
    }

    #endregion
}

