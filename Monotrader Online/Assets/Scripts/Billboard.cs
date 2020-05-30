using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform mainCamera;
    private void Start()
    {
        mainCamera = Camera.main.transform;
    }
    private void LateUpdate()
    {
        transform.LookAt(transform.position + mainCamera.rotation * Vector3.forward,
            mainCamera.rotation * Vector3.up) ;
    }
}
