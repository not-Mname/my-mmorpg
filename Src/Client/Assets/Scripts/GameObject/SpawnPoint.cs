using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public int id;
    Mesh mesh = null;

    void Start()
    {
        mesh = this.GetComponent<MeshFilter>().sharedMesh;
    }
#if UNITY_EDITOR

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 vec = this.transform.position + Vector3.up * .5f * this.transform.localScale.y;
        if(mesh != null)
        {
            //(绘制的网格， 绘制的位置， 绘制的朝向， 绘制的缩放)
            Gizmos.DrawWireMesh(mesh, vec, this.transform.rotation, this.transform.localScale);
        }
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.ArrowHandleCap(0, vec, this.transform.rotation, 1f, EventType.Repaint);
        UnityEditor.Handles.Label(vec, "SpawnPoint " + id);
    }

#endif
}

