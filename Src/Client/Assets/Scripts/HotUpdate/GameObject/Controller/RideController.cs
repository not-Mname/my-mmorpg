using Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class RideController : MonoBehaviour
{
    public Transform MountPoint;
    public EntityController Rider;
    public Vector3 Offset;
    private Animator _anim;

    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    void Update()
    {
        //如果没有骑乘点或者没有乘客，则不进行任何操作
        if (MountPoint == null || Rider == null) return;
        //设置骑乘点的位置
        //TransformDirection()方法可以将一个向量从本地坐标系转换到世界坐标系
        this.Rider.SetRidePosition(this.MountPoint.position + this.MountPoint.TransformDirection(this.Offset));
    }
    public void OnEntityEvent(EntityEvent @event, int param)
    {
        switch (@event)
        {
            case EntityEvent.Idle:
                _anim.SetBool("Move", false);
                break;
            case EntityEvent.MoveFwd:
                _anim.SetBool("Move", true);
                break;
            case EntityEvent.MoveBack:
                _anim.SetBool("Move", true);
                break;
            case EntityEvent.Jump:
                _anim.SetTrigger("Jump");
                break;

        }
    }

    public void SetRider(EntityController entityController)
    {
        this.Rider = entityController;
    }
}

