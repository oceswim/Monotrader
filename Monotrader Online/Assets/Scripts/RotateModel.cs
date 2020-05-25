
using UnityEngine;

public class RotateModel : MonoBehaviour
{

    void Update()
    {

        transform.Rotate(new Vector3(0, .5f, 0), Space.Self);
    }
}
