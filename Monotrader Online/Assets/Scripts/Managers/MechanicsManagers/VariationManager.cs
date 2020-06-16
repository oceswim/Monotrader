using Photon.Realtime;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
//all players
public class VariationManager : MonoBehaviour
{
    private const string EUROS_PRICE = "Euros_Price";
    private const string DOLLARS_PRICE = "Dollars_Price";
    private const string POUNDS_PRICE = "Pounds_Price";
    private const string YEN_PRICE = "Yen_Price";

    private const string EUROS_TREND = "Euros_Trend";
    private const string DOLLARS_TREND = "Dollars_Trend";
    private const string POUNDS_TREND = "Pounds_Trend";
    private const string YEN_TREND = "Yen_Trend";

    private const float MORE_VALUE = 1.05f;
    private const float LESS_VALUE = .95f;
    private Room myRoom;
    private float euros, dollars, pounds, yens;
    public static bool worldVar,nationalVar;
    public TMP_Text nationalText, worldText;
    public GameObject theWheel, confirmButton,panelNational,wheelPanel;
    private bool spin;
    private float rotSpeed;
    public static bool BeginProcess;
    public Button worldConfirm;
    //either one or all currencies increase or decrease their value of 5%
    void Start()
    {

        myRoom = GameManager.myRoom;
        
    }
    
    // Update is called once per frame
    void Update()
    {
        if (BeginProcess)
        {
            BeginProcess = false;
            GetRoomProperties();
        }
        if (worldVar)
        {
            worldVar = false;
            WorldVariation();
        }
        if(nationalVar)
        {
            nationalVar = false;
            spin = false;
            rotSpeed = 0;
            theWheel.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        if (spin)
        {
            if (rotSpeed <= .5f)
            {
                spin = false;
                float finalRot = theWheel.transform.localEulerAngles.z;
                DetermineCountry(finalRot);
            }
            theWheel.transform.Rotate(0, 0, rotSpeed);
            Spinning();
        }  
    }
    private void GetRoomProperties()
    {
        euros = (float)myRoom.CustomProperties[EUROS_PRICE];
        dollars = (float)myRoom.CustomProperties[DOLLARS_PRICE];
        pounds = (float)myRoom.CustomProperties[POUNDS_PRICE];
        yens = (float)myRoom.CustomProperties[YEN_PRICE];
        worldVar = false;
    }

    //World variation 
    private void WorldVariation()
    {
        float delta = 0;
        string mode = "";
        float newTrendE,newTrendD,newTrendP,newTrendY;
        float trendDelta=0;
        switch (VariationMode())
        {
            case 0://less value
                delta = LESS_VALUE;
                mode = "less";
                trendDelta = -.5f;
                break;
            case 1://more value
                delta = MORE_VALUE;
                mode = "more";
                trendDelta = .5f;
                break;
        }
        euros *= delta;
        newTrendE = (float)myRoom.CustomProperties[EUROS_TREND] + trendDelta;
        dollars *= delta;
        newTrendD = (float)myRoom.CustomProperties[DOLLARS_TREND] + trendDelta;
        pounds *= delta;
        newTrendP = (float)myRoom.CustomProperties[POUNDS_TREND] + trendDelta;
        yens *= delta;
        newTrendY = (float)myRoom.CustomProperties[YEN_TREND] + trendDelta;
        SetNewPrices(dollars, euros, pounds, yens);
        SetNewTrends(newTrendD, newTrendE, newTrendP, newTrendY);
        worldText.text = $"Each currencies became {mode} expensive.";
        worldConfirm.interactable = true;
    }

    //national variation section
    public void SpinWheel()
    {
        spin = true;
        rotSpeed = UnityEngine.Random.Range(50, 76);
    }
    private void Spinning()
    {
        float x = 1;
        while (x > .1f)
        {
            x = x - Time.deltaTime;

        }
        rotSpeed -= .5f;
    }
    private void DetermineCountry(float rotation)
    {
        Debug.Log("final rot:" + rotation);
        double value = Math.Round(rotation, 1);

        if (value >= 0 && value <= 96)
        {
            Debug.Log("america 2");
            NationalVariation(2);
            
        }
        else if (value >= 96.1 && value <= 190)
        {
            Debug.Log("france 1");
            NationalVariation(1);
           
        }
        else if (value >= 274.1 && value <= 360)
        {
            Debug.Log("japan 4");
            NationalVariation(4);
            //use player pref here.
        }
        else if (value >= 190.1 && value <= 274)
        {
            Debug.Log("england 3");
            NationalVariation(3);
        }

        wheelPanel.SetActive(false);
    }

    private void NationalVariation(int rand)
    {
        string nationality = "";
        string currency = "";
        string mode = "";
        float delta = 0;
        float trendDelta=0;
        switch (VariationMode())
        {
            case 0://less value
                delta = LESS_VALUE;
                trendDelta = -.5f;
                mode = "less";
                break;
            case 1://more value
                delta = MORE_VALUE;
                trendDelta = .5f;
                mode = "more";
                break;
        }
        float newTrend;
        switch (rand)
        {
            case 1:
                euros *= delta;
                nationality = "French";
                currency = "Euros";
                newTrend = (float)myRoom.CustomProperties[EUROS_TREND] + trendDelta;
                SetRoomProperty(EUROS_PRICE, euros);
                SetRoomProperty(EUROS_TREND, newTrend);
                break;
            case 2:
                dollars *= delta;
                nationality = "Americans";
                currency = "Dollars";
                newTrend = (float)myRoom.CustomProperties[DOLLARS_TREND] + trendDelta;
                SetRoomProperty(DOLLARS_PRICE, dollars);
                SetRoomProperty(DOLLARS_TREND, newTrend);
                break;
            case 3:
                pounds *= delta;
                nationality = "English";
                currency = "Pounds";
                newTrend = (float)myRoom.CustomProperties[POUNDS_TREND] + trendDelta;
                SetRoomProperty(POUNDS_PRICE, pounds);
                SetRoomProperty(POUNDS_TREND, newTrend);
                break;
            case 4:
                yens *= delta;
                nationality = "Japanese";
                currency = "Yens";
                newTrend = (float)myRoom.CustomProperties[YEN_TREND] + trendDelta;
                SetRoomProperty(YEN_PRICE, yens);
                SetRoomProperty(YEN_TREND, newTrend);
                break;

        }
        panelNational.SetActive(true);
        nationalText.text = $"Something went wrong with {nationality} markets. {currency} are now {mode} expensive";
        confirmButton.SetActive(true);
    }
    public void StartNational()
    {
        int rand = UnityEngine.Random.Range(1, 5);
        NationalVariation(rand);
    }


    //utilities
    private int VariationMode()
    {
        int rand = UnityEngine.Random.Range(0, 2);
        Debug.Log("VARIATION SCRIPT MODE:" + rand);
        return rand;
    }
    private void SetNewTrends(float d, float e, float p, float y)
    {
        SetRoomProperty(DOLLARS_TREND, d);
        SetRoomProperty(EUROS_TREND, e);
        SetRoomProperty(POUNDS_TREND, p);
        SetRoomProperty(YEN_TREND, y);
    }
    private void SetNewPrices(float d,float e,float p,float y)
    {
        SetRoomProperty(DOLLARS_PRICE,d);
        SetRoomProperty(EUROS_PRICE,e);
        SetRoomProperty(POUNDS_PRICE,p);
        SetRoomProperty(YEN_PRICE,y);
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
