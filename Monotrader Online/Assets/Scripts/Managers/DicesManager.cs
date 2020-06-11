using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;

public class DicesManager : MonoBehaviourPun
{
    private Rigidbody myBody;
    private const string dicesDoneRollingHashKey = "dicesDone";
    private Room myRoom;

    private int myIndex;
    private Vector3 noVelocity = Vector3.zero;
    private bool canGuess;
    public bool roll;
    public void Start()
    {
        myRoom = PhotonNetwork.CurrentRoom;
        roll = false;
        canGuess = false;
        myBody = transform.GetComponent<Rigidbody>();
        if(gameObject.name.EndsWith("1"))
        {
            myIndex = 1;
        }
        else
        {
            myIndex = 2;
        }
    }

    private void Update()
    {
        if(myBody.velocity.Equals(noVelocity) && canGuess)
        {
            canGuess = false;
            WhichDiceValue();
        }
        if(roll)
        {
            float dirX = UnityEngine.Random.Range(0, 500);
            float dirY = UnityEngine.Random.Range(0, 500);
            float dirZ = UnityEngine.Random.Range(0, 500);
            transform.position = new Vector3(transform.position.x, 5, transform.position.z);
            transform.rotation = Quaternion.identity;
            myBody.AddForce(transform.up * 500);
            myBody.AddTorque(dirX, dirY, dirZ);
            WaitOut(1);
            roll = false;
            canGuess = true;
        }

    }
    private void WaitOut(float seconds)
    {
        float start = 0;
        while (start < seconds)
        {
            //Debug.Log("Waiting " + start);
            start += Time.deltaTime;
        }
    }

    private void WhichDiceValue()
    {
      
        double xRot = Math.Round(transform.localEulerAngles.x);
        double zRot = Math.Round(transform.localEulerAngles.z);
        int diceVal = 0;
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
            diceVal = 1;
        }
        else if(xRot == 0 && zRot == 0)
        {
            Debug.Log(transform.name + " it's a 2");
            diceVal = 2;
        }
        else if (xRot == 0 && zRot == 90)
        {
            Debug.Log(transform.name + " it's a 3");
            diceVal = 3;
        }
        else if (xRot == 0 && zRot == -90)
        {
            Debug.Log(transform.name + " it's a 4");
            diceVal = 4;
        }
        else if (xRot == 0 && zRot == 180)
        {
            Debug.Log(transform.name + " it's a 5");
            diceVal = 5;
        }
        else if (xRot == -90 && zRot == 0)
        {
            Debug.Log(transform.name + " it's a 6");
            diceVal = 6;
        }

        Debug.Log("Dice"+this.gameObject.name+" set to :" + diceVal.ToString());
        GameManager.instance.SetDicePrefs(diceVal);

    }
    private void SetRoomProperty(string hashKey, int value)//general room properties
    {

        if (myRoom.CustomProperties[hashKey] == null)
        {
            myRoom.CustomProperties.Add(hashKey, value);
        }
        else if (myRoom.CustomProperties[hashKey] != null)
        {
            int newValue = (int)myRoom.CustomProperties[hashKey];
           switch(newValue)
            {
                case 0:
                    myRoom.CustomProperties[hashKey] = 1;
                    break;
                case 1:
                    myRoom.CustomProperties[hashKey] = 2;
                    break;
            }

        }

        myRoom.SetCustomProperties(myRoom.CustomProperties);
    }
}
