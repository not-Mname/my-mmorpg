using Asset;
using AssetBundleFramework;
using Entities;
using Managers;
using MMO;
using Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;

public class GameObjectManager : MonoSingleton<GameObjectManager>
{
    private Dictionary<int, GameObject> _characters = new Dictionary<int, GameObject>();
    public Queue<GameObject> ReadyObjects = new();

    protected override void OnAwake()
    {
        StartCoroutine(InitGameObjects());
        CharacterManager.Instance.OnCharacterEnter += OnCharacterEnter;
        CharacterManager.Instance.OnCharacterLeave += OnCharacterLeave;
    }

    void Update()
    {

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
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
        if (!_characters.ContainsKey(character.EntityId) || _characters[character.EntityId] == null)
        {
            StartCoroutine(CreateCharacterRoutine(character));
        }
        else
        {
            this.InitGamObject(_characters[character.EntityId], character);
        }


    }

    IEnumerator CreateCharacterRoutine(BattleUnit character)
    {
        IResource res = Resloader.Instance.LoadAssetAsync(character.Define.Resource);
        yield return res;
        if (res == null)
        {
            Debug.LogErrorFormat("Character[{0}] Resource[{1}] not existed.", character.Define.TID, character.Define.Resource);
            yield break;
        }

        // 实例化对象
        GameObject go = res.Instantiate(this.transform, true, true);
        go.SetActive(false);
        go.name = "Character_" + character.Id + "_" + character.Info.Name;
        _characters[character.EntityId] = go;

        // 设置父物体
        //go.transform.SetParent(this.transform, true);

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
                pic.character = character;
                pic.entityController = ec;
            }
        }
        go.transform.position = GameObjectTool.LogicToWorld(character.Position);
        go.transform.forward = GameObjectTool.LogicToWorld(character.Direction);
        if (!SceneManager.Instance.IsLoading)
        {
            go.SetActive(true);
        }
        else
        {
            go.SetActive(false);
            ReadyObjects.Enqueue(go);
        }

        ec.enabled = true;
        if (pic) { pic.enabled = true; }
        go.transform.SetParent(this.transform, true);
    }

    public RideController LoadRide(int rideId, Transform parent)
    {
        var rideDefine = DataManager.Instance.Rides[rideId];
        IResource res = Resloader.Instance.LoadAssetSync(rideDefine.Resource);
        if (res == null)
        {
            Debug.LogErrorFormat("Ride[{0}] Resource[{1}] not existed.", rideId, rideDefine.Resource);
            return null;
        }
        GameObject go = res.Instantiate(true);
        go.name = "Ride_" + rideId + "_" + rideDefine.Name;
        return go.GetComponent<RideController>();
    }

    public void DestroyCharacterObject(int characterId)
    {
        if (_characters.ContainsKey(characterId) && _characters[characterId] != null)
        {
            Destroy(_characters[characterId]);
            _characters.Remove(characterId);
        }
    }
}