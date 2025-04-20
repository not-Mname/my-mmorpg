using System;
using UnityEngine;
using UnityEngine.AI;

class NavPathRenderer : MonoSingleton<NavPathRenderer>
{
    LineRenderer _pathRenderer;
    NavMeshPath _path;

    void Start()
    {
        _pathRenderer = GetComponent<LineRenderer>();
        _pathRenderer.enabled = false;
    }

    public void SetPath(NavMeshPath path, Vector3 target)
    {
        _path = path;
        if(path == null)
        {
            _pathRenderer.enabled = false;
            _pathRenderer.positionCount = 0;
        }
        else
        {
            _pathRenderer.enabled = true;
            _pathRenderer.positionCount = path.corners.Length + 1;
            _pathRenderer.SetPositions(path.corners);
            _pathRenderer.SetPosition(_pathRenderer.positionCount - 1, target);//设置终点
            for(int i = 0; i < _pathRenderer.positionCount; i++)
            {
                _pathRenderer.SetPosition(i, _pathRenderer.GetPosition(i) + Vector3.up * 0.2f);
            }
        }
    }
}

