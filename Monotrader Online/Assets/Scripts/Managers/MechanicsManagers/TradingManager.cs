using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;

public class TradingManager : MonoBehaviour
{
    private const float MALUS_AMOUNT = .85f;
    private const string MALUS = "myMalus";
    private const string EUROS_PRICE = "Euros_Price";
    private const string DOLLARS_PRICE = "Dollars_Price";
    private const string POUNDS_PRICE = "Pounds_Price";
    private const string YEN_PRICE = "Yen_Price";

    private string PLAYER_GOLD;
    private string PLAYER_DOLLARS;
    private string PLAYER_EUROS;
    private string PLAYER_YENS;
    private string PLAYER_POUNDS;
    private Room myRoom;

    public Button confirmButton;
    public GameObject[] myTitles = new GameObject[4];
    public GameObject malusObject;
    public TMP_InputField myInputField;
    public Slider mySlider;
    public TMP_Text currencyText, valueText, buyText, sellText;
    private int tradingMode; // 0 is buy 1 is sell
    public static bool eurosTrading, poundsTrading, dollarsTrading, yensTrading;
    private float currencyPriceInGold;
    private double latestValue;
    private string currencyModeText, currencyToChange;
    // Start is called before the first frame update
    void Start()
    {
        myRoom = PhotonNetwork.CurrentRoom;
        tradingMode = 0;//set to buy intially.
        UpdateText(tradingMode);
        GetPrefs();
    }

    // Update is called once per frame
    void Update()
    {
        if (eurosTrading)
        {
            currencyToChange = PLAYER_EUROS;
            eurosTrading = false;
            myTitles[1].SetActive(true);
            currencyModeText = "€";
            currencyPriceInGold = (float)myRoom.CustomProperties[EUROS_PRICE];

        }
        else if (dollarsTrading)
        {
            currencyToChange = PLAYER_DOLLARS;
            dollarsTrading = false;
            myTitles[0].SetActive(true);
            currencyModeText = "$";
            currencyPriceInGold = (float)myRoom.CustomProperties[DOLLARS_PRICE];
        }
        else if (poundsTrading)
        {
            currencyToChange = PLAYER_POUNDS;
            poundsTrading = false;
            myTitles[2].SetActive(true);
            currencyModeText = "£";
            currencyPriceInGold = (float)myRoom.CustomProperties[POUNDS_PRICE];
        }
        else if (yensTrading)
        {
            currencyToChange = PLAYER_YENS;
            yensTrading = false;
            myTitles[3].SetActive(true);
            currencyModeText = "¥";
            currencyPriceInGold = (float)myRoom.CustomProperties[YEN_PRICE];
        }

    }

    private void GetPrefs()
    {
        PLAYER_GOLD = PlayerPrefs.GetString("MYGOLD");
        PLAYER_DOLLARS = PlayerPrefs.GetString("MYDOLLARS");
        PLAYER_EUROS = PlayerPrefs.GetString("MYEUROS");
        PLAYER_YENS = PlayerPrefs.GetString("MYYENS");
        PLAYER_POUNDS = PlayerPrefs.GetString("MYPOUNDS");
    }
    public void UpdateTextValue(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            string inputVal = value;
            int theValue = int.Parse(inputVal);
            double price;
            switch (tradingMode)
            {
                case 0://buy
                       //check player pref from lucky so that the transaction is 15% more.
                    price = Math.Round((theValue / currencyPriceInGold), 1);
                    if (PlayerPrefs.HasKey(MALUS))
                    {
                        if(PlayerPrefs.GetInt(MALUS)>0)
                        {
                            //we decrease the value obtained with the gold input
                            Debug.Log($"MALUS price before {price.ToString()}");
                            price *= MALUS_AMOUNT;
                            Debug.Log($"MALUS price after {price.ToString()}");
                            PlayerPrefs.SetInt(MALUS, 0);
                        }
                    }
                    latestValue = price;
                    valueText.text = price.ToString() + " " + currencyModeText;
                    //gold -> currency
                    break;
                case 1:

                    //check player pref from lucky so that the transaction brings less 15%.
                    price = Math.Round(currencyPriceInGold * theValue,1);
                    if (PlayerPrefs.HasKey(MALUS))
                    {
                        if (PlayerPrefs.GetInt(MALUS) > 0)
                        {
                            //we decrease the value obtained with the gold input
                            Debug.Log($"MALUS price before {price.ToString()}");
                            price *= MALUS_AMOUNT;
                            Debug.Log($"MALUS price after {price.ToString()}");
                            PlayerPrefs.SetInt(MALUS, 0);
                        }
                    }
                    latestValue = price;
                    valueText.text = price.ToString() + " G";
                    //currency -> gold
                    break;
            }
            confirmButton.interactable = true;
        }
        else
        {
            confirmButton.interactable = false;
        }
    }
    public void SwitchModes()
    {
        if (mySlider.value == mySlider.maxValue)
        {
            mySlider.value = mySlider.minValue;
            tradingMode = (int)mySlider.value;
            buyText.gameObject.SetActive(true);
            sellText.gameObject.SetActive(false);
            UpdateText(tradingMode);
            Debug.Log("Switching to buy");
        }
        else
        {
            mySlider.value = mySlider.maxValue;
            tradingMode = (int)mySlider.value;
            buyText.gameObject.SetActive(false);
            sellText.gameObject.SetActive(true);
            UpdateText(tradingMode);
            Debug.Log("Switching to sell");
        }
        if (!string.IsNullOrEmpty(myInputField.text))
        {
            UpdateTextValue(myInputField.text);
        }

    }
    private void UpdateText(int mode)
    {

        switch (mode)
        {
            case 0://buy
                currencyText.text = "G =";
                valueText.text = "0 " + currencyModeText;
                break;
            case 1://sell
                currencyText.text = currencyModeText + " =";
                valueText.text = "0 G";
                break;

        }

    }
    public void Confirm()
    {
        if (!string.IsNullOrEmpty(myInputField.text))
        {
            int theValue = int.Parse(myInputField.text);
            float newCurr;
            float newGold;
            switch (tradingMode)
            {
                case 0://buy
                    Debug.Log("BOUGHT with cost of " + theValue + " gold :" + latestValue + " " + currencyModeText);
                    //remove thevalue of gold
                    newGold = PlayerPrefs.GetFloat(PLAYER_GOLD) - theValue;
                    PlayerPrefs.SetFloat(PLAYER_GOLD, newGold);
                    newCurr = PlayerPrefs.GetFloat(currencyToChange) + (float)latestValue;
                    PlayerPrefs.SetFloat(currencyToChange, newCurr);
                    //increase the value of currency
                    //gold -> currency
                    break;
                case 1:
                    Debug.Log("sold with cost of " + theValue + " " + currencyModeText + ":" + latestValue + " gold");
                     newGold = PlayerPrefs.GetFloat(PLAYER_GOLD) + (float)latestValue;
                    PlayerPrefs.SetFloat(PLAYER_GOLD, newGold);
                     newCurr = PlayerPrefs.GetFloat(currencyToChange) - theValue;
                    PlayerPrefs.SetFloat(currencyToChange, newCurr);
                    //remove the value of currency
                    //increase value of gold
                    //currency -> gold
                    break;
            }
            MoneyManager.updateFortune=true;
        }
    }
}
