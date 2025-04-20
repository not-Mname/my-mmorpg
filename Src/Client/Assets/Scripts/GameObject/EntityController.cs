using Entities;
using JetBrains.Annotations;
using Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class EntityController : MonoBehaviour, IEntityNotify
{
    public Animator anim;
    public Rigidbody rb;
    private AnimatorStateInfo currentBaseState;

    public Entity entity;

    public UnityEngine.Vector3 position;
    public UnityEngine.Vector3 direction;
    Quaternion rotation;

    public UnityEngine.Vector3 lastPosition;
    Quaternion lastRotation;

    public float speed;
    public float animSpeed = 1.5f;
    public float jumpPower = 3.0f;

    public bool isPlayer = false;

    public RideController rideController;

    private int currentRide = 0;

    public Transform rideBone;
    void Start()
    {
        if (this.entity != null)
        {
            EntityManager.Instance.RigisterEntityChangeNotify(entity.entityId, this);
            UpdateTransform();
        }

        if (!isPlayer) rb.useGravity = false;
    }

    void FixedUpdate()
    {
        if (this.entity == null) return;

        this.entity.OnUpdate(Time.fixedDeltaTime);

        if (!isPlayer) UpdateTransform();
    }

    void OnDestroy()
    {
        if (this.entity != null) Debug.LogFormat("{0} OnDestroy :ID:{1} POS:{2} DIR:{3} SPD:{4} ", this.name, entity.entityId, entity.position, entity.direction, entity.speed);
    }

    void UpdateTransform()
    {
        if (this.entity == null) return;

        this.position = GameObjectTool.LogicToWorld(entity.position);
        this.direction = GameObjectTool.LogicToWorld(entity.direction);

        this.rb.MovePosition(this.position);
        this.transform.forward = this.direction;
        this.lastPosition = this.position;
        this.lastRotation = this.rotation;

    }

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
        Debug.LogFormat("{0} OnEntityChange :ID:{1} POS:{2} DIR:{3} SPD:{4} ", this.name, entity.Id, entity.Entity.Position, entity.Entity.Direction, entity.Entity.Speed);
    }

    public void Ride(int rideId)
    {
        if(currentRide == rideId) return;
        currentRide = rideId;
        if(rideId > 0)
        {
            this.rideController = GameObjectManager.Instance.LoadRide(rideId, this.transform);
        }
        else
        {
            Destroy(this.rideController.gameObject);
            this.rideController = null;
        }

        if(this.rideController == null)
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