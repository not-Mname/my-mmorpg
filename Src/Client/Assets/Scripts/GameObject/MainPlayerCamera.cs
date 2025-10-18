using Const;
using Managers;
using Models;
using UnityEngine;
using Utilities;

public class MainPlayerCamera : MonoSingleton<MainPlayerCamera>
{
    public Camera Camera;
    public Transform ViewPoint;
    public Transform TargetPoint;
    public GameObject Player;

    private Transform _cameraMainRotation;
    private bool isCursorVisible = false; // 初始隐藏
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
    public float x = 20;
    public float y = 0;


    void Start()
    {
        EVENT.Subscribe<string>(EventId.on_map_change, OnMapChange);
        this.ViewPoint = Player.transform;
        this._cameraMainRotation = this.gameObject.transform;
        this._cameraMainRotation.forward = this.ViewPoint.forward;
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

        if (InputManager.Instance.AltPressed)
        {
            isCursorVisible = !isCursorVisible; // 切换状态
            ApplyCursorVisibility(isCursorVisible);
        }

        if (!isCursorVisible)
        {
            x += InputManager.Instance.MouseValueX * xSpeed * Time.deltaTime;
            y -= InputManager.Instance.MouseValueY * ySpeed * Time.deltaTime;
            y = Mathf.Clamp(y, yMinLimit, yMaxLimit);
            if (this.TargetPoint == null)
            {
                Quaternion rotation = Quaternion.Euler(y, x, 0f);
                this._cameraMainRotation.rotation = rotation;
                transform.position = ViewPoint.position - rotation * Vector3.forward * distance + new Vector3(targetSide, targetHeight, 0);
            }
            else
            {
                this._cameraMainRotation.LookAt(this.TargetPoint);
                transform.position = ViewPoint.position - Vector3.forward * distance + new Vector3(targetSide + 5f, targetHeight, 0);
            }


        }
    }

    private bool CheckAndInit()
    {
        if (Player == null && User.Instance.CurrentCharacterObject != null)
        {
            Player = User.Instance.CurrentCharacterObject.gameObject;
            this.ViewPoint = Player.transform;
        }

        if (ViewPoint == null)
        {
            this.ViewPoint = Player.transform;
        }

        if (Player == null || Camera == null || ViewPoint == null)
        {
            return false;
        }

        return true;
    }

    public void ApplyCursorVisibility(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        this.isCursorVisible = visible;
    }
}

