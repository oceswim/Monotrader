
using UnityEngine;

public class RotateModel : MonoBehaviour
{

    void Update()
    {
        if (!gameObject.name.Equals("Display"))
        {
        
            transform.Rotate(new Vector3(0, 0, -1.5f), Space.Self);
        }
    }
}
