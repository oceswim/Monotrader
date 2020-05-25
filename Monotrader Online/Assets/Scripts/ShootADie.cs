using System;
using UnityEngine;

public class ShootADie : MonoBehaviour
{
    private Rigidbody myBody;
    

    private Vector3 noVelocity = Vector3.zero;
    private bool canGuess;
    public bool roll;
    public void Start()
    {
        GameManager.instance.AddDiceInstance(this);
        roll = false;
        canGuess = false;
        myBody = transform.GetComponent<Rigidbody>();
 
    }

    private void Update()
    {
        if(myBody.velocity.Equals(noVelocity) && canGuess)
        {
            canGuess = false;
            w();
        }
        if(roll)
        {
            roll = false;
            canGuess = true;
            float dirX = UnityEngine.Random.Range(0, 500);
            float dirY = UnityEngine.Random.Range(0, 500);
            float dirZ = UnityEngine.Random.Range(0, 500);
            transform.position = new Vector3(0, 5, 0);
            transform.rotation = Quaternion.identity;
            myBody.AddForce(transform.up * 500);
            myBody.AddTorque(dirX, dirY, dirZ);
        }

    }

    private void w()
    {
      
        double xRot = Math.Round(transform.localEulerAngles.x);
        double zRot = Math.Round(transform.localEulerAngles.z);
        
        if(xRot >180)
        {
            xRot = xRot - 360;
        }
        if(zRot > 180)
        {
            zRot = zRot - 360;
        }
        Debug.Log("X : " + xRot + " Z:" + zRot);
        if (xRot == 90 && zRot == 0)
        {
            Debug.Log(transform.name+" it's a 1");
        }
        else if(xRot == 0 && zRot == 0)
        {
            Debug.Log(transform.name + " it's a 2");
        }
        else if (xRot == 0 && zRot == 90)
        {
            Debug.Log(transform.name + " it's a 3");
        }
        else if (xRot == 0 && zRot == -90)
        {
            Debug.Log(transform.name + " it's a 4");
        }
        else if (xRot == 0 && zRot == 180)
        {
            Debug.Log(transform.name + " it's a 5");
        }
        else if (xRot == -90 && zRot == 0)
        {
            Debug.Log(transform.name + " it's a 6");
        }
        
    }
}
