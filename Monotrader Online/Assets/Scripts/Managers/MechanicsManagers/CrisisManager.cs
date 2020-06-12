using Photon.Realtime;
using System;
using UnityEngine;


//all players
public class CrisisManager : MonoBehaviour
{
    private const string EUROS_PRICE = "Euros_Price";
    private const string DOLLARS_PRICE = "Dollars_Price";
    private const string POUNDS_PRICE = "Pounds_Price";
    private const string YEN_PRICE = "Yen_Price";

    private const float CRISISAMOUNT = 1.15f;
    private string[] currencies = new string[4];
    private Room myRoom;
    // Start is called before the first frame update
    void Start()
    {
        myRoom = GameManager.myRoom;
        PopulateList();
        WhichCurrency();
    }

    // Update is called once per frame
    private void PopulateList()
    {
        currencies[0] = "d";
        currencies[1] = "e";
        currencies[2] = "p";
        currencies[3] = "y";
    }
    private void WhichCurrency()
    {
        int randomInt = UnityEngine.Random.Range(1, 5);
        Debug.Log(randomInt + " currencies will be affected");
        int randomIndex = UnityEngine.Random.Range(0, 4);
        for (int i =0;i<(randomInt-1);i++)
        {
            if(i==0)
            {
                string currency = currencies[randomIndex];
                Debug.Log("calling flucuate for : " + currency);
                //FluctuateCurrency(currency);
            }
            else
            {
                int newRandom = UnityEngine.Random.Range(0, 4);
                while(newRandom == randomIndex)
                {
                    newRandom = UnityEngine.Random.Range(0, 4);
                }
                randomIndex = newRandom;
                string currency = currencies[randomIndex];
                Debug.Log("calling flucuate for : " + currency);
                //FluctuateCurrency(currency);
            }
        }
    }

    //depending on the set properties, the corresponding price for a currency gets updated.
    private void FluctuateCurrency(string currency)
    {
        string whichPrice="";
        switch(currency)
        {
            case "e":
                whichPrice = EUROS_PRICE;
                break;
            case "d":
                whichPrice = DOLLARS_PRICE;
                break;
            case "p":
                whichPrice = POUNDS_PRICE;
                break;
            case "y":
                whichPrice = YEN_PRICE;
                break;

        }
        double newPrice = Math.Round((float)myRoom.CustomProperties[whichPrice] *CRISISAMOUNT,2);
        SetRoomProperty(whichPrice, (float)newPrice);
    }

    private void SetRoomProperty(string hashKey, float value)
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
