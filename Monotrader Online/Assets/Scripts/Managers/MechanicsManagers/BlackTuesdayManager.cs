using Photon.Realtime;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

//all players
public class BlackTuesdayManager : MonoBehaviour
{
    private const string EUROS_PRICE = "Euros_Price";
    private const string DOLLARS_PRICE = "Dollars_Price";
    private const string POUNDS_PRICE = "Pounds_Price";
    private const string YEN_PRICE = "Yen_Price";

    private const string EUROS_TREND = "Euros_Trend";
    private const string DOLLARS_TREND = "Dollars_Trend";
    private const string POUNDS_TREND = "Pounds_Trend";
    private const string YEN_TREND = "Yen_Trend";

    private double increase,DELTA;
    private Room myRoom;
    private float euros, dollars, pounds, yens;
    public TMP_Text blackTuesdayText;
    public static bool BeginProcess;
    public Button confirm;
    public GameObject increaseObject;
    public Animator historyAnimator;
    // Start is called before the first frame update
    void Start()
    {
        myRoom = GameManager.myRoom;
        
    }

    private void Update()
    {
        if(BeginProcess)
        {
            SetDelta();
            euros = (float)myRoom.CustomProperties[EUROS_PRICE];
            dollars = (float)myRoom.CustomProperties[DOLLARS_PRICE];
            pounds = (float)myRoom.CustomProperties[POUNDS_PRICE];
            yens = (float)myRoom.CustomProperties[YEN_PRICE];
            WorldVariation();
            BeginProcess = false;
        }
        if(increaseObject.activeSelf)
        {
            if (!historyAnimator.GetBool("BlinkH"))
            {
                historyAnimator.SetBool("BlinkH", true);
            }
        }
        else
        {
            if(historyAnimator.GetBool("BlinkH"))
            {
                historyAnimator.SetBool("BlinkH", false);
            }
        }
    }
    // Update is called once per frame
    private void SetDelta()
    {
        increase =Math.Round(UnityEngine.Random.Range(.05f, .21f),2);
        DELTA = Math.Round(1 + increase, 2);
        
        SetNewTrends((float)increase);
    }
    private void WorldVariation()
    {
       
        euros *= (float)DELTA;
        dollars *= (float)DELTA;
        pounds *= (float)DELTA;
        yens *= (float)DELTA;
        double roundEuros = Math.Round(euros, 2);
        double roundDollars = Math.Round(dollars, 2);
        double roundPounds = Math.Round(pounds, 2);
        double roundYens = Math.Round(yens, 2);
     
        SetNewPrices((float)roundDollars, (float)roundEuros, (float)roundPounds, (float)roundYens);
        blackTuesdayText.text = $"The trading market collapsed. Every currencies increased of {increase*100}%";
        confirm.interactable = true;
    }
    private void SetNewTrends(float val)
    {
       
        double d = Math.Round((float)myRoom.CustomProperties[DOLLARS_TREND] + val,2);
        double e = Math.Round((float)myRoom.CustomProperties[EUROS_TREND] + val, 2);
        double p = Math.Round((float)myRoom.CustomProperties[POUNDS_TREND] + val, 2);
        double y = Math.Round((float)myRoom.CustomProperties[YEN_TREND] + val, 2);
      
        SetRoomProperty(DOLLARS_TREND, (float)d);
        SetRoomProperty(EUROS_TREND, (float)e);
        SetRoomProperty(POUNDS_TREND, (float)p);
        SetRoomProperty(YEN_TREND, (float)y);
    }
    private void SetNewPrices(float d, float e, float p, float y)
    {
        SetRoomProperty(DOLLARS_PRICE, d);
        SetRoomProperty(EUROS_PRICE, e);
        SetRoomProperty(POUNDS_PRICE, p);
        SetRoomProperty(YEN_PRICE, y);

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
    public void Done()
    {
        increaseObject.SetActive(true);
        BoardManager.NextTurn();
    }
}
