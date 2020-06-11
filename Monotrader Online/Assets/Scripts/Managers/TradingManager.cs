using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;

public class TradingManager : MonoBehaviour
{
    private const string EUROS_PRICE = "Euros_Price";
    private const string DOLLARS_PRICE = "Dollars_Price";
    private const string POUNDS_PRICE = "Pounds_Price";
    private const string YEN_PRICE = "Yen_Price";
    private Room myRoom;

    public Button confirmButton;
    public GameObject[] myTitles = new GameObject[4];
    public TMP_InputField myInputField;
    public Slider mySlider;
    public TMP_Text currencyText, valueText, buyText, sellText;
    private int tradingMode; // 0 is buy 1 is sell
    public static bool eurosTrading, poundsTrading, dollarsTrading, yensTrading;
    private float currencyPriceInGold;
    private double latestValue;
    private string currencyModeText;
    // Start is called before the first frame update
    void Start()
    {
        myRoom = PhotonNetwork.CurrentRoom;
        tradingMode = 0;//set to buy intially.
        UpdateText(tradingMode);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (eurosTrading)
        {
            eurosTrading = false;
            myTitles[1].SetActive(true);
            currencyModeText = "€";
            currencyPriceInGold = (float)myRoom.CustomProperties[EUROS_PRICE];

        }
        else if (dollarsTrading)
        {
            dollarsTrading = false;
            myTitles[0].SetActive(true);
            currencyModeText = "$";
            currencyPriceInGold = (float)myRoom.CustomProperties[DOLLARS_PRICE];
        }
        else if (poundsTrading)
        {
            poundsTrading = false;
            myTitles[2].SetActive(true);
            currencyModeText = "£";
            currencyPriceInGold = (float)myRoom.CustomProperties[POUNDS_PRICE];
        }
        else if (yensTrading)
        {
            yensTrading = false;
            myTitles[3].SetActive(true);
            currencyModeText = "¥";
            currencyPriceInGold = (float)myRoom.CustomProperties[YEN_PRICE];
        }

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
                    price = Math.Round((theValue / currencyPriceInGold), 2);
                    latestValue = price;
                    valueText.text = price.ToString() + " " + currencyModeText;
                    //gold -> currency
                    break;
                case 1:
                    price = Math.Round(currencyPriceInGold * theValue);
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
            switch (tradingMode)
            {
                case 0://buy
                    Debug.Log("BOUGHT with cost of " + theValue + " gold :" + latestValue + " " + currencyModeText);
                    //gold -> currency
                    break;
                case 1:
                    Debug.Log("sold with cost of " + theValue + " " + currencyModeText + ":" + latestValue + " gold");
                    //currency -> gold
                    break;
            }
        }
    }
}
