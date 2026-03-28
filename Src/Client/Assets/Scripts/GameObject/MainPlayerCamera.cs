using Const;
using Managers;
using Models;
using System;
using UnityEngine;
using Utilities;

public class MainPlayerCamera : MonoSingleton<MainPlayerCamera>
{
    public Camera Camera;
    public Transform FollowPoint;
    public GameObject Follower;

    private Transform _cameraMainRotation;
    public bool isCursorVisible = false; // 初始隐藏
    public float targetHeight = 3f;
    public float targetSide = -0.1f;
    public float distance = 4;
    public float maxDistance = 8;
    public float minDistance = 2.2f;
    public float xSpeed = 250;
    public float ySpeed = 125;
    public float yMinLimit = -50;
    public float yMaxLimit = 72;
    public float zoomRate = 80;
    public float yaw  = 20;//偏航角（左右看）
    public float pitch  = 0;//俯仰角（上下看）
    public float targetHeightOffset = 3f;// 锁定时相机上抬的目标偏移量
    public float minPitch = -30f;   // 最多向下看 30 度（不能太低头）
    public float maxPitch = 60f;    // 最多向上看 60 度（不能太仰头）

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

    #region 私有
    void Start()
    {
        EVENT.Subscribe<string>(EventId.on_map_change, OnMapChange);
        Init();
    }

    public void Init()
    {
        if (Follower == null) return;
        this.FollowPoint = Follower.transform;
        this._cameraMainRotation = this.gameObject.transform;
        this._cameraMainRotation.forward = this.FollowPoint.forward;
    }

    void OnDestroy()
    {
        EVENT.Unsubscribe(EventId.on_map_change);
    }

    void OnMapChange(string mapName)
    {
        LogHelper.Log("OnMapChange: " + mapName);
        if (mapName == "Loading" || mapName == "CharSelect")
        {
            this.Camera.gameObject.SetActive(false);
            ApplyCursorVisibility(true);
        }
        else
        {
            this.Camera.gameObject.SetActive(true);
            ApplyCursorVisibility(false);
        }
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

    private void UpdatePositionAndDirection()
    {
        if (!isCursorVisible)
        {
            if (this._targetPoint == null)
            {
                Quaternion rotation = Quaternion.Euler(pitch , yaw , 0f);
                this._cameraMainRotation.rotation = rotation;
            }
            else
            {
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

                Quaternion finalRotation = yawRotation * Quaternion.Euler(pitchAngle, 0, 0);
                this._cameraMainRotation.rotation = finalRotation;
            }
        }

        transform.position = FollowPoint.position - this._cameraMainRotation.rotation * Vector3.forward * distance + new Vector3(targetSide, targetHeight, 0);
    }

    private void UpdateInput()
    {
        if (!isCursorVisible)
        {
            yaw  += InputManager.Instance.MouseValueX * xSpeed * Time.deltaTime;
            pitch  -= InputManager.Instance.MouseValueY * ySpeed * Time.deltaTime;
            pitch  = Mathf.Clamp(pitch , yMinLimit, yMaxLimit);
        }
    }

    private bool CheckAndInit()
    {
        if (InputManager.Instance.AltPressed)
        {
            isCursorVisible = !isCursorVisible; // 切换状态
            ApplyCursorVisibility(isCursorVisible);
        }

        if (Follower == null && User.Instance.CurrentCharacterObject != null)
        {
            Follower = User.Instance.CurrentCharacterObject.gameObject;
            Init();
        }

        if (FollowPoint == null && Follower != null)
        {
            this.FollowPoint = Follower.transform;
        }

        if (Follower == null || Camera == null || FollowPoint == null)
        {
            return false;
        }

        return true;
    }
    #endregion

    #region 公开
    public void ApplyCursorVisibility(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        this.isCursorVisible = visible;
    }

    #endregion
}

