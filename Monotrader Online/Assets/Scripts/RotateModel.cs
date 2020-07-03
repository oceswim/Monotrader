
using UnityEngine;

public class RotateModel : MonoBehaviour
{

    void Update()
    {
        if (gameObject.name.Equals("Display"))
        {
            transform.Rotate(new Vector3(0, .5f, 0), Space.Self);
        }
        else
        {
            transform.Rotate(new Vector3(0, 0, -1.5f), Space.Self);
        }
    }
}
