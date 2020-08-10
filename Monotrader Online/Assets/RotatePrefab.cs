
using UnityEngine;

public class RotatePrefab : MonoBehaviour
{
   
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, -1.5f,0));
    }
}
