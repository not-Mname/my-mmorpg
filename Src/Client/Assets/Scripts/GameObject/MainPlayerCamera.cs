using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class MainPlayerCamera : MonoSingleton<MainPlayerCamera>
{
    public Camera camera;
    public Transform viewPoint;

    public GameObject player;

    void LateUpdate()
    {
        if (player == null && User.Instance.CurrentCharacterObject != null)
            player = User.Instance.CurrentCharacterObject.gameObject;
        if (player == null) return;

        this.transform.position = player.transform.position;
        this.transform.rotation = player.transform.rotation;
    }
}

