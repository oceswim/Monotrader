using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

//all players
public class CrisisManager : MonoBehaviour
{
    private const string EUROS_PRICE = "Euros_Price";
    private const string DOLLARS_PRICE = "Dollars_Price";
    private const string POUNDS_PRICE = "Pounds_Price";
    private const string YEN_PRICE = "Yen_Price";

    private const string EUROS_TREND = "Euros_Trend";
    private const string DOLLARS_TREND = "Dollars_Trend";
    private const string POUNDS_TREND = "Pounds_Trend";
    private const string YEN_TREND = "Yen_Trend";

    private const float CRISISAMOUNT = 1.15f;
    private const float TRENDAMOUNT = .15f;
    private string[] currencies = new string[4];
    private List<string> affectedCurrencies;
    private Room myRoom;
    public Button confirmButton;
    public TMP_Text crisisText;
    public static bool BeginProcess;
    // Start is called before the first frame update
    void Start()
    {
        myRoom = GameManager.myRoom;
        PopulateList();

    }
    private void Update()
    {
        if(BeginProcess)
        {
            WhichCurrency();
            BeginProcess = false;
        }
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
        affectedCurrencies = new List<string>();
        
        int randomInt = UnityEngine.Random.Range(1, 5);//1 to 4 
    
        if (randomInt < 4 && randomInt >1)
        {
            var rng = new System.Random();
            var values = Enumerable.Range(0, 4).OrderBy(x => rng.Next()).ToArray();//the random currency affected
            if (randomInt == 2)
            { 
               
                int first = values[0];
                int second = values[1];

                CallFluctuate(first);
                CallFluctuate(second);

            }
            else if (randomInt == 3)
            {
                
                int first = values[0];
                int second = values[1];
                int third = values[2];
                CallFluctuate(first);
                CallFluctuate(second);
                CallFluctuate(third);

            }
 
        }
        else if(randomInt ==1)
        {
            int randomIndex = UnityEngine.Random.Range(0, 4);//0 to 3 the random currency affected initially
            string currency = currencies[randomIndex];
            FluctuateCurrency(currency);
        
        }
        else
        {
            foreach(string s in currencies)
            {
            
                FluctuateCurrency(s);
            }
        }
        SetText();
    }
    private void CallFluctuate(int v)
    {
        string currency = currencies[v];
        
        FluctuateCurrency(currency);
    }
    //depending on the set properties, the corresponding price for a currency gets updated.
    private void FluctuateCurrency(string currency)
    {
        float newTrend=0;
        string whichPrice="";
        string whichTrend = "";
        switch(currency)
        {
            case "e":
                affectedCurrencies.Add("Euros");
                whichPrice = EUROS_PRICE;
                newTrend = (float)myRoom.CustomProperties[EUROS_TREND] + TRENDAMOUNT;
                whichTrend = EUROS_TREND;
                break;
            case "d":
                affectedCurrencies.Add("Dollars");
                whichPrice = DOLLARS_PRICE;
                whichTrend = DOLLARS_TREND;
                newTrend = (float)myRoom.CustomProperties[DOLLARS_TREND]+ TRENDAMOUNT;
                break;
            case "p":
                affectedCurrencies.Add("Pounds");
                whichPrice = POUNDS_PRICE;
                whichTrend = POUNDS_TREND;
                newTrend = (float)myRoom.CustomProperties[POUNDS_TREND] + TRENDAMOUNT;
                break;
            case "y":
                affectedCurrencies.Add("Yens");
                whichPrice = YEN_PRICE;
                whichTrend = YEN_TREND;
                newTrend = (float)myRoom.CustomProperties[YEN_TREND] + TRENDAMOUNT;
                break;

        }
        Debug.Log($"The which trend {whichTrend} and new trend {newTrend}");
        SetRoomProperty(whichTrend, newTrend);
        double newPrice = Math.Round((float)myRoom.CustomProperties[whichPrice] * CRISISAMOUNT,2);
        SetRoomProperty(whichPrice, (float)newPrice);
        
    }
    private void SetText()
    {
        string text = "A crisis just hit the trading market. The ";
        foreach(string s in affectedCurrencies)
        {
            
            if (affectedCurrencies.IndexOf(s) == (affectedCurrencies.Count - 1))
            {
                if(affectedCurrencies.Count>1)
                {
                    text += s + " are affected.";
                }
                else
                {
                    text += s + " is affected.";
                }
                
            }
            else if(affectedCurrencies.IndexOf(s) == (affectedCurrencies.Count - 2))
            {
                text += s + " and ";
            }
            else
            {
                text += s + ", ";
            }
        }
        crisisText.text = text;

        confirmButton.interactable = true;
    }

    public void Done()
    {

        BoardManager.NextTurn();
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
