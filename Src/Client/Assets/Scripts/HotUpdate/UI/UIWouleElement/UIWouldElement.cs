using UnityEngine;

public class UIWouldElement : MonoBehaviour
{
    public Transform owner;

    public float height = 0.5f;

    private float controllerHeight = 0;

    void Start()
    {
        controllerHeight = owner.gameObject.GetComponent<CharacterController>().height;
    }

    void Update()
    {
        if(owner != null)
        {
            this.transform.position = (owner.position + Vector3.up * (controllerHeight + height));
            Transform cameraTransform = MainPlayerCamera.Instance.transform;
            this.transform.forward = cameraTransform.forward;
        }
    }
}
