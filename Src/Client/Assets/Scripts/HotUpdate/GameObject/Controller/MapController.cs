using Managers;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public Collider miniMapBoundingBox;

    IEnumerator OnStart()
    {
        if (!MapService.Instance.MapSystemInitDone)
        {
            yield return null;
        }
        MiniMapManager.Instance.UpdateMiniMap(miniMapBoundingBox);
    }

    void Start()
    {
        StartCoroutine(OnStart());
    }

    void Update()
    {

    }
}
