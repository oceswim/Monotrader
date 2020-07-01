using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using System;

public class TradingManager : MonoBehaviour
{

    private const float MALUS_AMOUNT = 1.15f;
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
    public GameObject malusObject, valueMinimumText;
    public TMP_InputField myInputField;
    public Slider mySlider;
    public TMP_Text currencyText, valueText, buyText, sellText;
    private int tradingMode, currencyMode; // 0 is buy 1 is sell
    public static bool eurosTrading, poundsTrading, dollarsTrading, yensTrading;
    private float currencyPriceInGold;
    private double latestValue;
    private string currencyModeText, currencyToChange;
    public static bool BeginProcess;
    // Start is called before the first frame update
    void Start()
    {
        myRoom = GameManager.myRoom;

    }

    // Update is called once per frame
    void Update()
    {
        if (BeginProcess)
        {
            tradingMode = 0;//set to buy intially.
            UpdateText(tradingMode);
            GetPrefs();
            BeginProcess = false;
        }
        if (eurosTrading)
        {
            currencyToChange = PLAYER_EUROS;
            eurosTrading = false;
            myTitles[1].SetActive(true);
            currencyModeText = "€";
            currencyPriceInGold = (float)myRoom.CustomProperties[EUROS_PRICE];
            currencyMode = 2;
        }
        else if (dollarsTrading)
        {
            currencyToChange = PLAYER_DOLLARS;
            dollarsTrading = false;
            myTitles[0].SetActive(true);
            currencyModeText = "$";
            currencyPriceInGold = (float)myRoom.CustomProperties[DOLLARS_PRICE];
            currencyMode = 1;
        }
        else if (poundsTrading)
        {
            currencyToChange = PLAYER_POUNDS;
            poundsTrading = false;
            myTitles[2].SetActive(true);
            currencyModeText = "£";
            currencyPriceInGold = (float)myRoom.CustomProperties[POUNDS_PRICE];
            currencyMode = 3;
        }
        else if (yensTrading)
        {
            currencyToChange = PLAYER_YENS;
            yensTrading = false;
            myTitles[3].SetActive(true);
            currencyModeText = "¥";
            currencyPriceInGold = (float)myRoom.CustomProperties[YEN_PRICE];
            currencyMode = 4;
        }
  
    }

    private void GetPrefs()
    {
        PLAYER_GOLD = PlayerPrefs.GetString("MYGOLD");
        PLAYER_DOLLARS = PlayerPrefs.GetString("MYDOLLARS");
        PLAYER_EUROS = PlayerPrefs.GetString("MYEUROS");
        PLAYER_YENS = PlayerPrefs.GetString("MYYENS");
        PLAYER_POUNDS = PlayerPrefs.GetString("MYPOUNDS");
        Debug.Log("malus value :" + PlayerPrefs.GetInt(MALUS));
    }
    public void UpdateTextValue(string value) // check here if the value is less than what's in the bank + is less than what player has. otherwise need to put less.
    {
        if (PlayerPrefs.HasKey(MALUS))
        {
            if (PlayerPrefs.GetInt(MALUS) > 0)
            {
                Debug.Log("MALUS ON");
                malusObject.SetActive(true);
            }
        }
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
                    latestValue = price;
                    valueText.text = price.ToString() + " " + currencyModeText;

                    //gold -> currency
                    break;
                case 1:

                    //check player pref from lucky so that the transaction brings less 15%.
                    price = Math.Round(currencyPriceInGold * theValue, 1);
                    latestValue = price;
                    valueText.text = price.ToString() + " G";
                    //currency -> gold
                    break;
            }
            if (Int32.Parse(value) >= 250)
            {
                if (valueMinimumText.activeSelf)
                {
                    valueMinimumText.SetActive(false);
                }
                confirmButton.interactable = true;
            }
            else
            {
                if (!valueMinimumText.activeSelf)
                {
                    if (confirmButton.interactable)
                    {
                        confirmButton.interactable = false;
                    }
                    valueMinimumText.SetActive(true);

                }
            }
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
            if (PlayerPrefs.HasKey(MALUS))
            {
                if (PlayerPrefs.GetInt(MALUS) > 0)
                {
                    //we decrease the value obtained with the gold input
                    Debug.Log("price before malus :" + theValue);
                    double rounded =Math.Round(theValue * MALUS_AMOUNT, 1);
                    theValue = (int)rounded;
                    Debug.Log("price with malus :" + theValue);
                    PlayerPrefs.SetInt(MALUS, 0);
                }
            }
            float newCurr;
            float newGold;
            switch (tradingMode)
            {
                case 0://buy
                    //remove thevalue of gold
                    newGold = PlayerPrefs.GetFloat(PLAYER_GOLD) - theValue;
                    Debug.Log("BEFORE : " + PlayerPrefs.GetFloat(currencyToChange));
                    newCurr = PlayerPrefs.GetFloat(currencyToChange) + (float)latestValue;
                    PlayerPrefs.SetFloat(PLAYER_GOLD, newGold);
                    PlayerPrefs.SetFloat(currencyToChange, newCurr);
                    Debug.Log("AFTER : " + PlayerPrefs.GetFloat(currencyToChange));
                    MoneyManager.instance.UpdateFortuneInGame();
                    //increase the value of currency
                    //gold -> currency
                    break;
                case 1:
                    newGold = PlayerPrefs.GetFloat(PLAYER_GOLD) + (float)latestValue;
                    Debug.Log("BEFORE : " + PlayerPrefs.GetFloat(currencyToChange));
                    newCurr = PlayerPrefs.GetFloat(currencyToChange) - theValue;
                    PlayerPrefs.SetFloat(PLAYER_GOLD, newGold);
                    PlayerPrefs.SetFloat(currencyToChange, newCurr);
                    Debug.Log("After : " + PlayerPrefs.GetFloat(currencyToChange));
                    MoneyManager.instance.UpdateFortuneInGame();
                    //remove the value of currency
                    //increase value of gold
                    //currency -> gold
                    break;
            }
           
            UpdateBankings(theValue, (int)Math.Round(latestValue, 0), currencyMode, tradingMode);
            myInputField.text = string.Empty;
            mySlider.value = mySlider.minValue;
            malusObject.SetActive(false);
            if (myTitles[0].activeSelf)
            {
                myTitles[0].SetActive(false);
            }
            if (myTitles[1].activeSelf)
            {
                myTitles[1].SetActive(false);
            }
            if (myTitles[2].activeSelf)
            {
                myTitles[2].SetActive(false);
            }
            if (myTitles[3].activeSelf)
            {
                myTitles[3].SetActive(false);
            }
        }
    }
    private void UpdateBankings(int toAdd, int toRemove, int currency, int tradeMode)
    {
        Debug.Log("TO ADD CURR :" + toAdd + " " + currency + " to remove :" + toRemove);
        switch (currency)
        {
            case 1://dollars
                switch (tradeMode)
                {
                    case 0://buy
                        BankManager.instance.UpdateGold(toAdd);
                        BankManager.instance.UpdateDollars(-toRemove);
                        break;
                    case 1:

                        BankManager.instance.UpdateGold(-toRemove);
                        BankManager.instance.UpdateDollars(toAdd);
                        break;
                }
                break;
            case 2://euros
                switch (tradeMode)
                {
                    case 0://buy

                        BankManager.instance.UpdateGold(toAdd);
                        BankManager.instance.UpdateEuros(-toRemove);
                        break;

                    case 1:

                        BankManager.instance.UpdateGold(-toRemove);
                        BankManager.instance.UpdateEuros(toAdd);
                        break;
                }
                break;
            case 3://pounds
                switch (tradeMode)
                {
                    case 0://buy
                        BankManager.instance.UpdateGold(toAdd);
                        BankManager.instance.UpdatePounds(-toRemove);
                        break;
                    case 1:
                        BankManager.instance.UpdateGold(-toRemove);
                        BankManager.instance.UpdatePounds(toAdd);
                        break;
                }
                break;
            case 4://yens
                switch (tradeMode)
                {
                    case 0://buy
                        BankManager.instance.UpdateGold(toAdd);
                        BankManager.instance.UpdateYens(-toRemove);
                        break;
                    case 1:
                        BankManager.instance.UpdateGold(-toRemove);
                        BankManager.instance.UpdateYens(toAdd);
                        break;
                }
                break;
        }
        //SetRoomProperty(TRIGGER_UPDATE, 1);
        BankManager.Trigger = true;

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
