using Entities;
using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectManager : MonoSingleton<GameObjectManager>
{
    Dictionary<int, GameObject> characters = new Dictionary<int, GameObject>();

    protected override void OnAwake()
    {
        StartCoroutine(InitGameObjects());
        CharacterManager.Instance.OnCharacterEnter += OnCharacterEnter;
        CharacterManager.Instance.OnCharacterLeave += OnCharacterLeave;
    }

    void Update()
    {

    }

    void OnDestroy()
    {
        CharacterManager.Instance.OnCharacterEnter -= OnCharacterEnter;
        CharacterManager.Instance.OnCharacterLeave -= OnCharacterLeave;
    }

    void OnCharacterEnter(BattleUnit character)
    {
        if (character != null) CreateCharacteObject(character);
    }

    void OnCharacterLeave(BattleUnit character)
    {
        if (character != null) DestroyCharacterObject(character.EntityId);
    }

    IEnumerator InitGameObjects()
    {
        foreach (var character in CharacterManager.Instance.Characters.Values)
        {
            CreateCharacteObject(character);
            yield return null;
        }
    }

    void CreateCharacteObject(BattleUnit character)
    {
        if (!characters.ContainsKey(character.EntityId) || characters[character.EntityId] == null)
        {
            StartCoroutine(CreateCharacterRoutine(character));
        }
        else
        {
            this.InitGamObject(characters[character.EntityId], character);
        }
            

    }

    IEnumerator CreateCharacterRoutine(BattleUnit character)
    {
        UnityEngine.Object obj = Resloader.Load<UnityEngine.Object>(character.Define.Resource);

        if (obj == null)
        {
            Debug.LogErrorFormat("Character[{0}] Resource[{1}] not existed.", character.Define.TID, character.Define.Resource);
            yield break;
        }

        // 实例化对象
        GameObject go = (GameObject)Instantiate(obj);
        go.name = "Character_" + character.Id + "_" + character.Info.Name;
        characters[character.EntityId] = go;

        // 设置父物体
        go.transform.SetParent(this.transform, true);

        // 等待一帧确保组件初始化
        yield return null;

        UIWouldElementManager.Instance.AddPlayerElement(go.transform, character);
        this.InitGamObject(go, character);
    }

    public void InitGamObject(GameObject go, BattleUnit character)
    {        
        EntityController ec = go.GetComponent<EntityController>();
        if (ec != null)
        {
            ec.entity = character;
            ec.isPlayer = character.IsCurrentPlayer;
            character.Controller = ec;
            ec.Ride(character.Info.Ride);
            ec.Init();
        }
        PlayerController pic = go.GetComponent<PlayerController>();
        if (pic != null)
        {
            if (character.IsCurrentPlayer)
            {
                User.Instance.CurrentCharacterObject = pic;
                MainPlayerCamera.Instance?.Init();
                pic.character = character;
                pic.enabled = true;
                pic.entityController = ec;
            }
            else
            {
                pic.enabled = false;
            }
        }
        go.transform.position = GameObjectTool.LogicToWorld(character.Position);
        go.transform.forward = GameObjectTool.LogicToWorld(character.Direction);
        
        go.transform.SetParent(this.transform, true);
    }

    public RideController LoadRide(int rideId, Transform parent)
    {
        var rideDefine = DataManager.Instance.Rides[rideId];
        UnityEngine.Object obj = Resloader.Load<UnityEngine.Object>(rideDefine.Resource);
        if (obj == null)
        {
            Debug.LogErrorFormat("Ride[{0}] Resource[{1}] not existed.", rideId, rideDefine.Resource);
            return null;
        }
        GameObject go = (GameObject)Instantiate(obj, parent);
        go.name = "Ride_" + rideId + "_" + rideDefine.Name;
        return go.GetComponent<RideController>();
    }

    public void DestroyCharacterObject(int characterId)
    {
        if (characters.ContainsKey(characterId) && characters[characterId]!= null)
        {
            Destroy(characters[characterId]);
            characters.Remove(characterId);
        }
    }
}