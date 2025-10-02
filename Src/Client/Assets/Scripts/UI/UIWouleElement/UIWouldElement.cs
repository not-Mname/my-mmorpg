using UnityEngine;

public class UIWouldElement : MonoBehaviour
{
    public Transform owner;

    public float height = 2;

    void Update()
    {
        if(owner != null)
        {
            this.transform.position = (owner.position + Vector3.up * height);
        }
        Transform cameraTransform = MainPlayerCamera.Instance.Camera.transform;
        this.transform.forward = cameraTransform.forward;
    }
}
