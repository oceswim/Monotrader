using System;
using UnityEngine;

public class SpinTheWheel : MonoBehaviour
{
    private string PLAYER_GOLD;
    private string PLAYER_DOLLARS;
    private string PLAYER_EUROS;
    private string PLAYER_YENS;
    private string PLAYER_POUNDS;
    private const string MALUS = "myMalus";
    public GameObject theWheel,confirmButton;
    private bool spin;
    private float rotSpeed;
    public static bool BeginProcess;
    private bool mode1;
    // Start is called before the first frame update
    void Start()
    {
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
            GetPrefs();
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
        if(mode1)
        {
            mode1 = false;
            MoneyManager.updateFortune = true;
        }
        BoardManager.NextTurn();
    }
    public void Spin()
    {        
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
        Debug.Log("final rot:" + rotation);
        double value = Math.Round(rotation, 1);

        if(value>=0 && value<=96)
        {
            Debug.Log("+500");
            float newGold = PlayerPrefs.GetFloat(PLAYER_GOLD) + 500;
            BankManager.instance.UpdateGold(-500);
            PlayerPrefs.SetFloat(PLAYER_GOLD, newGold);
            mode1 = true;
        }
        else if(value >= 96.1 && value <=190)
        {
            Debug.Log("-250");
            mode1 = true;
            float newGold = PlayerPrefs.GetFloat(PLAYER_GOLD) - 250;
            BankManager.instance.UpdateGold(250);
            PlayerPrefs.SetFloat(PLAYER_GOLD, newGold);
        }
        else if(value >= 274.1 && value <= 360)
        {
            Debug.Log("Malus applied.");
            //use player pref here.
            PlayerPrefs.SetInt(MALUS, 1);
        }
        else if(value >= 190.1 && value <= 274)
        {
            Debug.Log("+500$");
            mode1 = true;
            string key = "";

            int randomInt = UnityEngine.Random.Range(0, 4);
            switch(randomInt)
            {
                case 0:
                    key= PLAYER_DOLLARS;
                    BankManager.instance.UpdateDollars(-500);
                    //dollars
                    break;
                case 1:
                    key =PLAYER_EUROS;
                    BankManager.instance.UpdateEuros(-500);
                    //euros
                    break;
                case 2:
                    key=PLAYER_POUNDS;
                    BankManager.instance.UpdatePounds(-500);
                    //pounds
                    break;
                case 3:
                    key=PLAYER_YENS;
                    BankManager.instance.UpdateYens(-500);
                    //yen
                    break;

            }
            float newFortune = PlayerPrefs.GetFloat(key) + 500;
            PlayerPrefs.SetFloat(key, newFortune);
        }
        
        confirmButton.SetActive(true);
    }

    private void GetPrefs()
    {
      
      PLAYER_GOLD =   PlayerPrefs.GetString("MYGOLD");
      PLAYER_DOLLARS =   PlayerPrefs.GetString("MYDOLLARS");
      PLAYER_EUROS =  PlayerPrefs.GetString("MYEUROS");
      PLAYER_YENS =  PlayerPrefs.GetString("MYYENS");
      PLAYER_POUNDS =  PlayerPrefs.GetString("MYPOUNDS");
        
    }
}
