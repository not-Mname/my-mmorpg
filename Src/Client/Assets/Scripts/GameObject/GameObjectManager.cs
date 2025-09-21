using Entities;
using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class GameObjectManager : MonoSingleton<GameObjectManager>
{
    Dictionary<int, GameObject> characters = new Dictionary<int, GameObject>();

    protected override void OnStart()
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

    void OnCharacterEnter(Character character)
    {
        if (character != null) CreateCharacteObject(character);
    }

    void OnCharacterLeave(Character character)
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

    void CreateCharacteObject(Character character)
    {
        if (!characters.ContainsKey(character.EntityId) || characters[character.EntityId] == null)
        {
            UnityEngine.Object obj = Resloader.Load<UnityEngine.Object>(character.Define.Resource);

            if (obj == null)
            {
                Debug.LogErrorFormat("Character[{0}] Resource[{1}] not existed.", character.Define.TID, character.Define.Resource);
                return;
            }

            GameObject go = (GameObject)Instantiate(obj, this.transform);
            go.name = "Character_" + character.Id + "_" +character.Info.Name;
 
            characters[character.EntityId] = go;

            UIWouldElementManager.Instance.AddPlayerElement(go.transform, character);
        }
        this.InitGamObject(characters[character.EntityId], character);
    }

    public void InitGamObject(GameObject go, Character character)
    {        
        go.transform.position = GameObjectTool.LogicToWorld(character.Position);
        go.transform.forward = GameObjectTool.LogicToWorld(character.Direction);

        EntityController ec = go.GetComponent<EntityController>();
        if (ec != null)
        {
            ec.entity = character;
            ec.isPlayer = character.IsCurrentPlayer;
            ec.Ride(character.Info.Ride);
        }
        PlayerInputController pic = go.GetComponent<PlayerInputController>();
        if (pic != null)
        {
            if (character.IsCurrentPlayer)
            {
                User.Instance.CurrentCharacterObject = pic;
                MainPlayerCamera.Instance.player = go;
                pic.character = character;
                pic.enabled = true;
                pic.entityController = ec;
            }
            else
            {
                pic.enabled = false;
            }
        }     
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