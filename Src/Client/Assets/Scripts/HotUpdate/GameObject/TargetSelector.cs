using Managers;
using System;
using UnityEngine;

internal class TargetSelector : MonoSingleton<TargetSelector>
{
    Projector _projector;
    bool _active = false;
    Vector3 _center;
    float _range;
    float _size;
    Vector3 offset = new Vector3(0, 2f, 0);

    protected Action<Vector3> selectPoint;
    protected override void OnAwake()
    {
        base.OnAwake();
        if (!_projector)
        {
            _projector = GetComponentInChildren<Projector>();
            this._projector.gameObject.SetActive(_active);
        }
    }

    public void Active(bool active)
    {
        _active = active;
        if(!_projector){ return; }
        _projector.gameObject.SetActive(active);
        _projector.orthographicSize = _size * 0.5f;
    }

    void Update()
    {
        if (!_active) { return; }
        if(!_projector){ return; }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Terrain")))
        {
            Vector3 hitPoint = hit.point;
            Vector3 distance = hitPoint - _center;
            
            if (distance.magnitude >= _range)
            {
                hitPoint = _center + (distance.normalized * _range);
            }
            _projector.transform.position = hitPoint + offset;
            if (InputManager.Instance.MouseLeftPressed)
            {
                this.selectPoint?.Invoke(hitPoint);
                this.Active(false);
            }
        }
        if (InputManager.Instance.MouseRightPressed)
        {
            this.Active(false);
        }
    }

    public static void ShowSelector(Vector3Int center, int range, int size, Action<Vector3> onPointSelected)
    {
        if (!Instance) { return; }
        Instance.selectPoint = onPointSelected;
        Instance._center = GameObjectTool.LogicToWorld(center);
        Instance._range = GameObjectTool.LogicToWorld(range);
        Instance._size = GameObjectTool.LogicToWorld(size);
        Instance.Active(true);
    }
}

