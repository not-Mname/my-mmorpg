using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWouldElement : MonoBehaviour
{
    public Transform owner;

    public float height = 2;

    void Start()
    {

    }

    void Update()
    {
        if(owner != null)
        {
            this.transform.position = (owner.position + Vector3.up * height);
        }
        Transform cameraTransform = MainPlayerCamera.Instance.camera.transform;
        this.transform.forward = cameraTransform.forward;
        /*this.transform.LookAt(cameraTransform.position, Vector3.up);
        transform.Rotate(0, 180, 0);*/
    }
}
