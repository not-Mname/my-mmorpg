using Managers;
using Models;
using UIFramework;
using UnityEngine;
using UnityEngine.UI;


public class UIMiniMap : PanelController
{
    public Collider miniMapBoudingBox;
    public Text mapName;
    public Image Map;
    public Image arrow;
    private Transform playerTransform;

    public void UpdateMap()
    {
        mapName.text = DataManager.Instance.Maps[User.Instance.CurrentMapData.ID].Name;
        this.Map.overrideSprite = MiniMapManager.Instance.LodeCuurentMap();
        Map.SetNativeSize();
        Map.transform.localPosition = new Vector3(0, 0, 0);
        miniMapBoudingBox = MiniMapManager.Instance.MiniMapCollider;
        playerTransform = null;
    }

    public void SetMiniMapPosition()
    {
        
        if (playerTransform == null)
        {
            if (User.Instance.CurrentCharacterObject == null) return;
            playerTransform = User.Instance.CurrentCharacterObject.transform;
        }
        if (playerTransform == null || miniMapBoudingBox == null) return;

        float x = miniMapBoudingBox.bounds.size.x;
        float y = miniMapBoudingBox.bounds.size.z;

        float realX = playerTransform.position.x - miniMapBoudingBox.bounds.min.x;
        float realY = playerTransform.position.z - miniMapBoudingBox.bounds.min.z;

        float pivotX = realX / x;
        float pivotY = realY / y;

        Map.rectTransform.pivot = new Vector2(pivotX, pivotY);
        Map.rectTransform.localPosition = new Vector2(0, 0);
        arrow.transform.eulerAngles = new Vector3(0, 0, -playerTransform.transform.eulerAngles.y);
    }

    void Start()
    {
        MiniMapManager.Instance.miniMap = this;
    }

    void Update()
    {
        this.SetMiniMapPosition();
    }
}
