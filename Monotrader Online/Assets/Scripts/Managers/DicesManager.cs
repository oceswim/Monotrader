using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;

public class DicesManager : MonoBehaviourPunCallBacks
{
    private Rigidbody myBody;
    private Room myRoom;

    private int myIndex, temp;
    private Vector3 noVelocity = Vector3.zero;
    private bool canGuess;
    public bool roll,goodToRoll;
    public bool taxesRoll,switchOwner;
    private const string POSITION_INDEX_PREF_KEY = "myPositionIndex";
    private static float dirX, dirY, dirZ;
    public void Start()
    {
        dirX = dirY = dirZ =temp= 0;
        myRoom = PhotonNetwork.CurrentRoom;
        roll = canGuess = taxesRoll =goodToRoll= false;
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
      
        if (roll)
        {
             dirX = UnityEngine.Random.Range(0, 500);
             dirY = UnityEngine.Random.Range(0, 500);
             dirZ = UnityEngine.Random.Range(0, 500);
            goodToRoll = true;
            roll = false;
        }
        else if(goodToRoll)
        {
            if (!canGuess)
            {
                transform.position = new Vector3(transform.position.x, 5, transform.position.z);
                transform.rotation = Quaternion.identity;
                int force = UnityEngine.Random.Range(250, 351);
                myBody.AddForce(transform.up * force);
                myBody.AddTorque(dirX, dirY, dirZ, ForceMode.VelocityChange);
 
            }
            if (!myBody.velocity.Equals(noVelocity))//allows to make sure the dice roll before a value is guessed
            {
                canGuess = true;
                goodToRoll = false;
            }
        }
        else if (myBody.velocity.Equals(noVelocity) && canGuess)
        {
            canGuess = false;
            WhichDiceValue();
        }
        if(switchOwner)
        {
            switchOwner = false;
            SwitchViewOwner();
        }

    }
    private void WaitOut(float seconds)
    {
        float start = 0;
        while (start < seconds)
        {

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

        if (xRot == 90 && zRot == 0)
        {

            diceVal = 1;
        }
        else if(xRot == 0 && zRot == 0)
        {
    
            diceVal = 2;
        }
        else if (xRot == 0 && zRot == 90)
        {

            diceVal = 3;
        }
        else if (xRot == 0 && zRot == -90)
        {
  
            diceVal = 4;
        }
        else if (xRot == 0 && zRot == 180)
        {
 
            diceVal = 5;
        }
        else if (xRot == -90 && zRot == 0)
        {

            diceVal = 6;
        }

        if (!taxesRoll)
        {
            
            GameManager.instance.SetDicePrefs(diceVal);
            
        }
        else
        {
            taxesRoll = false;
            TaxesManager.values.Add(diceVal);
            TaxesManager.status++;
        }

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
   
    private void SwitchViewOwner()
    {
        if (photonView.IsMine)
        {
            photonView.TransferOwnership(PhotonNetwork.PlayerListOthers[0]);
           
        }
    }
}
