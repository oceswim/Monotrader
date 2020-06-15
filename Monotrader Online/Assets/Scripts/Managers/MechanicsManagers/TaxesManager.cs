using UnityEngine;
using TMPro;
using System;
using Photon.Realtime;

//single player
public class TaxesManager : MonoBehaviour
{

    //fortune 0 - 10,000 = 5% = 1% each monney gets decreased.
    //fortune 10,001 - 20,000 = 10% = 2% each money gets decreased.
    //fortune 20,001 - 8 = 15% = 3% each money gets decreased.
    //if no more of one currency take on the gold.
    private string PLAYER_GOLD;
    private string PLAYER_DOLLARS;
    private string PLAYER_EUROS;
    private string PLAYER_YENS;
    private string PLAYER_POUNDS;
    private const int TOTAL_MONEY_TYPE = 5;
    private const string EUROS_PRICE = "Euros_Price";
    private const string DOLLARS_PRICE = "Dollars_Price";
    private const string POUNDS_PRICE = "Pounds_Price";
    private const string YEN_PRICE = "Yen_Price";

    private Room myRoom;
    private const string FORTUNE = "myFortune";
    private float generalDollars,generalEuros,generalYens,generalPounds,priceE, priceD,priceP,priceY,playerE, playerD, playerP, playerY, playerG, playerFortune,taxesPercent;
    public TMP_Text content;
    // Start is called before the first frame update
    void Start()
    {
        myRoom = GameManager.myRoom;
        InitialiseHashKeys();
        GetPrefs();
        GetPrices();
        generalEuros= GetGeneralPrices(priceE, playerE);
        generalDollars= GetGeneralPrices(priceD, playerD);
        generalPounds= GetGeneralPrices(priceP, playerP);
        generalYens= GetGeneralPrices(priceY, playerY);

        if(playerFortune<=10000)
        {
            taxesPercent = .05f;
        }
        else if(playerFortune>10000 && playerFortune<=20000)
        {
            taxesPercent = .1f;
        }
        else if(playerFortune>20000)
        {
            taxesPercent = .15f;
        }
        int percent = (int)taxesPercent * 100;
        double total = Math.Round(playerFortune * taxesPercent,2);
        content.text = $"It is time to pay your taxes! After some calculations, it was decided you had to pay {percent.ToString()}% of your fortune. The total to pay is {total.ToString()} Gold.";
    }
    private void InitialiseHashKeys()
    {
        PLAYER_GOLD = PlayerPrefs.GetString("MYGOLD");
        PLAYER_DOLLARS = PlayerPrefs.GetString("MYDOLLARS");
        PLAYER_EUROS = PlayerPrefs.GetString("MYEUROS");
        PLAYER_YENS = PlayerPrefs.GetString("MYYENS");
        PLAYER_POUNDS = PlayerPrefs.GetString("MYPOUNDS");
    }
    // Update is called once per frame
    void Update()
    {
        
    }


    //to properly substract taxes : we need to bring every currencies back to a 1 on 1 proportion.
    //once it's done, then we substract the tax. then we call update fortune on moneymanager which will update the player's fortune
    //based on the new prices set here.
    public void SubstractTaxes()
    {
        float toRemove = 1-(taxesPercent / TOTAL_MONEY_TYPE); // EITHER 1% 2% 3% per currencies.
        if (generalDollars * toRemove > 0)
        {
            generalDollars = generalDollars * toRemove;
        }
        else
        {
            Debug.Log("There was not enough dollars for taxes");
            playerG *= toRemove;
        }
        if (generalEuros * toRemove > 0)
        {
            generalEuros *= toRemove;
        }
        else
        {
            Debug.Log("There was not enough euros for taxes");
            playerG = playerG * toRemove;
        }
        if (generalPounds * toRemove > 0)
        {
            generalPounds *= toRemove;
        }
        else
        {
            Debug.Log("There was not enough pounds for taxes");
            playerG = playerG * toRemove;
        }
        if (generalYens * toRemove > 0)
        {
            Debug.Log("There was not enough yens for taxes");
            generalYens *= toRemove;
        }
        else
        {
            playerG *= toRemove;
        }
        playerG *= toRemove;

        SetPrefs(generalDollars, generalEuros, generalPounds, generalYens, playerG);
        MoneyManager.updateFortune = true;
    }

    //converting currencies back to 1 on 1 proportion.
    private float GetGeneralPrices(float price, float value)
    {
        float newVal = 0;
        if (price == 1)
        {
            newVal = value; //no change in the value.
        }
        else if(price >1)
        {
            float tempPercent = 1-(price - 1);
            newVal = value * tempPercent;
        }
        else if(price <1)
        {
            float tempPercent = 1 + (1 - price);
            newVal = value * tempPercent;
        }
        
        return newVal;
    }
    private void GetPrices()
    {
        priceE = (float)myRoom.CustomProperties[EUROS_PRICE];
        priceD = (float)myRoom.CustomProperties[DOLLARS_PRICE];
        priceP = (float)myRoom.CustomProperties[POUNDS_PRICE];
        priceY = (float)myRoom.CustomProperties[YEN_PRICE];
    }
    private void GetPrefs()
    {
        playerFortune = PlayerPrefs.GetFloat(FORTUNE);
        Debug.Log("TAXES FORTUNE : " + playerFortune);
        playerE = PlayerPrefs.GetFloat(PLAYER_EUROS);
        playerD = PlayerPrefs.GetFloat(PLAYER_DOLLARS);
        playerP = PlayerPrefs.GetFloat(PLAYER_POUNDS);
        playerY = PlayerPrefs.GetFloat(PLAYER_YENS);
        playerG = PlayerPrefs.GetFloat(PLAYER_GOLD);
    }
    private void SetPrefs(float d, float e, float p, float y,float g)
    {
        PlayerPrefs.SetFloat(PLAYER_EUROS, e);
        PlayerPrefs.SetFloat(PLAYER_DOLLARS, d);
        PlayerPrefs.SetFloat(PLAYER_YENS, y);
        PlayerPrefs.SetFloat(PLAYER_POUNDS, p);
        PlayerPrefs.SetFloat(PLAYER_GOLD, g);
    }
}
