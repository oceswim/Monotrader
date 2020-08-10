using Photon.Realtime;
using System;
using UnityEngine;

public class SpinTheWheel : MonoBehaviour
{
    public const string TRIGGER_UPDATE = "UpdateActivated";
    public const string GOLD_UPDATE = "GoldUpdate";
    public const string DOLLARS_UPDATE = "DollarsUpdate";
    public const string EUROS_UPDATE = "EurosUpdate";
    public const string POUNDS_UPDATE = "PoundsUpdate";
    public const string YENS_UPDATE = "YensUpdate";

    public GameObject theWheel,confirmButton;
    private bool spin;
    private float rotSpeed;
    public AudioSource spinningSound;
    public static bool BeginProcess;
    private bool mode1;
    private Room myRoom;
    // Start is called before the first frame update
    void Start()
    {
        myRoom = GameManager.myRoom;
        spin = false;
        rotSpeed = 0;
        theWheel.transform.eulerAngles = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(BeginProcess)
        {
            rotSpeed = 0;
            theWheel.transform.eulerAngles = new Vector3(0, 0, 0);
            BeginProcess = false;
        }
        if(spin)
        {
            if(rotSpeed<=.5f)
            {
                spin = false;
                float finalRot =theWheel.transform.localEulerAngles.z;
                DeterminePerk(finalRot);

            }
            theWheel.transform.Rotate(0, 0, rotSpeed);
            Spinning();
        }
    }
    public void Confirm()
    {
        Debug.Log(mode1 + " mode1");
        if (mode1)
        {
            mode1 = false;
            BankManager.Trigger = true;
            MoneyManager.instance.UpdateFortuneInGame();
        }
        else
        {
            BoardManager.NextTurn();
        }
    }
    public void Spin()
    {
        spinningSound.Play();
        spin = true;
        rotSpeed = UnityEngine.Random.Range(50, 76);
    }

    private void Spinning()
    {
        float x = 1;
        while(x>.1f)
        {
            x = x - Time.deltaTime;

        }
        rotSpeed -= .5f;
    }

    private void DeterminePerk(float rotation)
    {

        double value = Math.Round(rotation, 1);

         if(value>=0 && value<=96)
         {
             mode1 = true;
             MoneyManager.PLAYER_GOLD += 500;
             BankManager.instance.UpdateGold(-500);

         }
         else if(value >= 96.1 && value <=190)
         {
             mode1 = true;
             MoneyManager.PLAYER_GOLD -= 250;
             BankManager.instance.UpdateGold(250);

         }
         else if(value >= 274.1 && value <= 360)
         {

             //use player pref here.
             GameManager.malusOn = true;

         }
         else if(value >= 190.1 && value <= 274)
         {

             mode1 = true;

             int randomInt = UnityEngine.Random.Range(0, 4);
             switch(randomInt)
             {
                 case 0:
                     MoneyManager.PLAYER_DOLLARS+=500;
                     BankManager.instance.UpdateDollars(-500);

                     //dollars
                     break;
                 case 1:
                     MoneyManager.PLAYER_EUROS+=500;
                     BankManager.instance.UpdateEuros(-500);

                     //euros
                     break;
                 case 2:
                     MoneyManager.PLAYER_POUNDS+=500;
                     BankManager.instance.UpdatePounds(-500);

                     //pounds
                     break;
                 case 3:
                     MoneyManager.PLAYER_YENS+=500;
                     BankManager.instance.UpdateYens(-500);

                     //yen
                     break;

             }

         }


        confirmButton.SetActive(true);
    }
    private void SetRoomProperty(string hashKey, int value)
    {
        if (myRoom.CustomProperties[hashKey] == null)
        {
            myRoom.CustomProperties.Add(hashKey, value);
        }
        else
        {

            myRoom.CustomProperties[hashKey] = value;

        }
        myRoom.SetCustomProperties(myRoom.CustomProperties);

    }
}
