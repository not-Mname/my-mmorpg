using Effect;
using Entities;
using Managers;
using SkillBridge.Message;
using UnityEngine;
using Utilities;


public class EntityController : MonoBehaviour, IEntityNotify, IEntityController
{
    public Animator anim;
    public CharacterController characterController;
    private AnimatorStateInfo currentBaseState;

    public Entity entity;

    public Vector3 position;
    public Vector3 direction;
    Quaternion rotation;

    public Vector3 lastPosition;
    Quaternion lastRotation;

    public float speed;
    public float animSpeed = 1.5f;
    public float jumpPower = 3.0f;

    public bool isPlayer = false;

    public RideController rideController;
    public EntityEffectManager EffectManager;

    private int currentRide = 0;

    public Transform rideBone;

    #region Unity 事件
    void Update()
    {
        if (this.entity == null) { return; }

        // 只有玩家角色需要客户端预测更新
        // AI角色完全由服务器同步，不需要客户端预测
        if (isPlayer)
        {
            this.entity.OnUpdate(Time.deltaTime);
        }
        else
        {
            // 对于AI角色，直接同步位置
            SyncEntityTransform();
        }
    }

    void OnDestroy()
    {
        if (this.entity != null) Debug.LogFormat("{0} OnDestroy :ID:{1} POS:{2} DIR:{3} SPD:{4} ", this.name, entity.EntityId, entity.Position, entity.Direction, entity.Speed);
    }

    void OnMouseDown()
    {
        BattleUnit unit = this.entity as BattleUnit;
        if (unit.IsCurrentPlayer) return;
        BattleManager.Instance.CurrentTarget = unit;
    }

    #endregion


    #region IEntityNotify 方法
    public void OnEntityEvent(EntityEvent entityEvent, int param)
    {
        switch (entityEvent)
        {
            case EntityEvent.Idle:
                anim.SetBool("Move", false);
                anim.SetTrigger("Idle");
                break;
            case EntityEvent.MoveFwd:
                anim.SetBool("Move", true);
                break;
            case EntityEvent.MoveBack:
                anim.SetBool("Move", true);
                break;
            case EntityEvent.Jump:
                anim.SetBool("Jump", true);
                break;
            case EntityEvent.Ride:
                this.Ride(param);
                break;
        }
        if (rideController != null) this.rideController.OnEntityEvent(entityEvent, param);
    }

    public void OnEntityRemoved()
    {
        if (UIWouldElementManager.Instance != null)
            UIWouldElementManager.Instance.RemoveElement(this.transform);
        Destroy(this.gameObject);
    }

    public void OnEntityChange(NEntitySync entity)
    {
        //Debug.LogFormat("{0} OnEntityChange :ID:{1} POS:{2} DIR:{3} SPD:{4} ", this.name, entity.Id, entity.Entity.Position, entity.Entity.Direction, entity.Entity.Speed);
    }
    #endregion


    #region IEntityController 方法
    public void PlayAnim(string animName)
    {
        this.anim.SetTrigger(animName);
    }

    public void SetStandby(bool standby)
    {
        this.anim.SetBool("Standby", standby);
    }

    public void UpdateDirection()
    {
        this.direction = GameObjectTool.LogicToWorld(entity.Direction);
        this.transform.forward = this.direction;
        this.lastRotation = this.rotation;
    }

    public void PlayEffect(EffectType type, string name, BattleUnit target, float duration)
    {
        Transform targetTrans = target.Controller.GetTransform();
        if (type == EffectType.Position || type == EffectType.Hit)
        {
            FXManager.Instance.PlayEffect(type, name, targetTrans, target.GetHitOffset(), duration);
        }
        else
        {
            this.EffectManager.PlayEffect(type, name, targetTrans, target.GetHitOffset(), duration);
        }
    }

    public void PlayEffect(EffectType type, string name, NVector3 targetPosition, float duration)
    {
        if (type == EffectType.Position || type == EffectType.Hit)
        {
            FXManager.Instance.PlayEffect(type, name, null, GameObjectTool.LogicToWorld(targetPosition), duration);
        }
        else
        {
            this.EffectManager.PlayEffect(type, name, null, GameObjectTool.LogicToWorld(targetPosition), duration);
        }
    }

    public Transform GetTransform()
    {
        return this.transform;
    }
    #endregion

    public void UpdateTransform()
    {
        this.position = GameObjectTool.LogicToWorld(entity.Position);
        this.transform.position = this.position;
        this.lastPosition = this.position;
        UpdateDirection();
    }

    public void Init()
    {
        if (this.entity != null)
        {
            EntityManager.Instance.RigisterEntityChangeNotify(entity.EntityId, this);
            SyncEntityTransform();
        }
    }

    /// <summary>
    /// 同步实体位置信息到客户端，并应用到当前对象上。
    /// 用于非当前玩家角色的位置同步。
    /// </summary>
    void SyncEntityTransform()
    {
        if (this.entity == null) { return; }

        this.position = GameObjectTool.LogicToWorld(entity.Position);
        this.direction = GameObjectTool.LogicToWorld(entity.Direction);
        this.speed = GameObjectTool.LogicToWorld(entity.Speed);


        Vector3 dir = this.position - transform.position;
        float distance = dir.magnitude;
        if (distance > 0.01f)
        {
            // speed 接近 0 时无法通过 Move 消除误差，直接 snap
            if (speed < 0.01f || distance > 5f)
            {
                this.transform.position = this.position;
                if (distance > 5f)
                    LogHelper.Log($"[{this.gameObject.name}] 瞬移");
            }
            else
            {
                dir.Normalize();
                this.characterController.Move(dir * speed * Time.deltaTime);
                LogHelper.Log($"[{this.gameObject.name}] 移动 dir : {dir }speed : {speed} distance : {dir * speed * Time.deltaTime}");
            }
        }
        this.transform.forward = this.direction;
        this.lastPosition = this.position;
        this.lastRotation = this.rotation;
    }

    public void Ride(int rideId)
    {
        if (currentRide == rideId) return;
        currentRide = rideId;
        if (rideId > 0)
        {
            this.rideController = GameObjectManager.Instance.LoadRide(rideId, this.transform);
        }
        else
        {
            Destroy(this.rideController.gameObject);
            this.rideController = null;
        }

        if (this.rideController == null)
        {
            this.anim.transform.localPosition = Vector3.zero;
            this.anim.SetLayerWeight(1, 0);//设置权重为0，使动画停止播放
        }
        else
        {
            this.rideController.SetRider(this);
            this.anim.SetLayerWeight(1, 1);
        }
    }

    public void SetRidePosition(Vector3 position)
    {
        this.anim.transform.position = position + (this.anim.transform.position - this.rideBone.position);
    }
}