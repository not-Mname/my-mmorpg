using Common.Data;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterGameObject : MonoBehaviour
{
    public int id;
    Mesh mesh = null;

    void Start()
    {
        this.mesh = this.GetComponent<MeshFilter>().sharedMesh;
    }


    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        Debug.LogFormat("TeleporterGameObject : Character {0} Enter Teleporter {1}", other.name, this.id);
        PlayerController playerInputController = other.GetComponent<PlayerController>();
        if(playerInputController != null && playerInputController.isActiveAndEnabled)
        {
            TeleporterDefine td = DataManager.Instance.Teleporters[this.id];
            if( td == null)
            {
                Debug.LogFormat("TeleporterGameObject : Character {0} Enter Teleporter {1} but TeleporterDefine is null");
                return;
            }
            Debug.LogFormat("TeleporterGameObject : Character {0} Enter Teleporter {1} : {2}", playerInputController.character.Info.Name, td.ID, td.Name);
            if(td.LinkTo > 0)
            {
                if(DataManager.Instance.Teleporters.ContainsKey(td.LinkTo))
                    MapService.Instance.SendMapTeleport(this.id);
                else
                    Debug.LogFormat("Teleporter id {0} LinkTo {1} error", td.ID, td.LinkTo);
            }
        }
    }

#if UNITY_EDITOR //意思是只在编辑器下执行
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (this.mesh != null)
        {
            Gizmos.DrawWireMesh(this.mesh, this.transform.position, transform.rotation, transform.lossyScale);
        }
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.ArrowHandleCap(0, this.transform.position, this.transform.rotation, 1f, EventType.Repaint);
    }
#endif
}
