using Const;
using Models;
using UnityEngine;
using Utilities;


public class MainPlayerCamera : MonoSingleton<MainPlayerCamera>
{
    public Camera Camera;
    public Transform ViewPoint;
    public GameObject Player;

    void Start()
    {
        EVENT.Subscribe<string>(EventId.on_map_change, OnMapChange);
    }

    void OnMapChange(string mapName)
    {
        LogHelper.Log("MainPlayerCamera OnMapChange: " + mapName);
        if(mapName == "Loading" || mapName == "CharSelect")
        {
            this.Camera.gameObject.SetActive(false);
        }
        else
        {
            this.Camera.gameObject.SetActive(true);
        }
            
    }

    void LateUpdate()
    {
        if (Player == null && User.Instance.CurrentCharacterObject != null)
            Player = User.Instance.CurrentCharacterObject.gameObject;
        if (Player == null) return;

        this.transform.position = Player.transform.position;
        this.transform.rotation = Player.transform.rotation;
    }
}

