using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;

public class ShootADie : MonoBehaviourPun
{
    private Rigidbody myBody;
    private const string prefDice1 = "Dice1Val";
    private const string prefDice2 = "Dice2Val";
    private const string dicesDoneRollingHashKey = "dicesDone";
    private Room myRoom;

    private Vector3 noVelocity = Vector3.zero;
    private bool canGuess;
    public bool roll;
    public void Start()
    {
        myRoom = PhotonNetwork.CurrentRoom;
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

        if(gameObject.name.Contains("1"))
        {
            Debug.Log("Dice 1 set to :" + diceVal.ToString());
            PlayerPrefs.SetInt(prefDice1, diceVal);
        }
        else if(gameObject.name.Contains("2"))
        {
            Debug.Log("Dice 2 set to :" + diceVal.ToString());
            PlayerPrefs.SetInt(prefDice2, diceVal);
        }
        SetRoomProperty(dicesDoneRollingHashKey, 1);
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
