using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using System;

public class TradingManager : MonoBehaviour
{

    private const float MALUS_AMOUNT = .85f;
    private const string EUROS_PRICE = "Euros_Price";
    private const string DOLLARS_PRICE = "Dollars_Price";
    private const string POUNDS_PRICE = "Pounds_Price";
    private const string YEN_PRICE = "Yen_Price";

    private Room myRoom;

    public Button confirmButton,handleButton;
    public GameObject[] myTitles = new GameObject[4];
    public GameObject malusObject, valueMinimumText;
    public TMP_InputField myInputField;
    public Toggle buyToggle, sellToggle;
    public TMP_Text currencyText, valueText, notEnoughText,savingsText,trendValueText,availableGoldText,availableCurrencyText;
    private int tradingMode, currencyMode; // 0 is buy 1 is sell
    public static bool eurosTrading, poundsTrading, dollarsTrading, yensTrading, BeginProcess;
    private float currencyPriceInGold;
    private double latestValue;
    private string currencyModeText;
    private float currencyToChange;
    private bool penalty;
    // Start is called before the first frame update
    void Start()
    {
        myRoom = GameManager.myRoom;

    }

    // Update is called once per frame
    void Update()
    {
     
        if (eurosTrading)
        {
            currencyToChange = MoneyManager.PLAYER_EUROS;
            eurosTrading = false;
            myTitles[1].SetActive(true);
            currencyModeText = "€";
            currencyPriceInGold = (float)myRoom.CustomProperties[EUROS_PRICE];
            currencyMode = 2;
        }
        else if (dollarsTrading)
        {
            currencyToChange = MoneyManager.PLAYER_DOLLARS;
            dollarsTrading = false;
            myTitles[0].SetActive(true);
            currencyModeText = "$";
            currencyPriceInGold = (float)myRoom.CustomProperties[DOLLARS_PRICE];
            currencyMode = 1;
        }
        else if (poundsTrading)
        {
            currencyToChange = MoneyManager.PLAYER_POUNDS;
            poundsTrading = false;
            myTitles[2].SetActive(true);
            currencyModeText = "£";
            currencyPriceInGold = (float)myRoom.CustomProperties[POUNDS_PRICE];
            currencyMode = 3;
        }
        else if (yensTrading)
        {
            currencyToChange = MoneyManager.PLAYER_YENS;
            yensTrading = false;
            myTitles[3].SetActive(true);
            currencyModeText = "¥";
            currencyPriceInGold = (float)myRoom.CustomProperties[YEN_PRICE];
            currencyMode = 4;
        }
        if (BeginProcess)
        {
            if (GameManager.malusOn)
            {

                Debug.Log("MALUS ON");
                malusObject.SetActive(true);

            }
            UiManager.tradeOn = true;
            tradingMode = 0;//set to buy intially.
            ShowAvailabilities();
            UpdateText(tradingMode);
            CheckMin();
            BeginProcess = false;
        }
    }


    public void UpdateTextValue(string value) // check here if the value is less than what's in the bank + is less than what player has. otherwise need to put less.
    {

        if (!string.IsNullOrEmpty(value))
        {
            string inputVal = value;
            int theValue = int.Parse(inputVal);
            double price;
            double malusVariation = 1;
            if(GameManager.malusOn)
            {
                malusVariation = MALUS_AMOUNT;
            }
            switch (tradingMode)
            {
                case 0://buy
                       //check player pref from lucky so that the transaction is 15% more.
                    
                    price = Math.Round((theValue * currencyPriceInGold*malusVariation), MidpointRounding.AwayFromZero);
                    latestValue = price;
                    valueText.text = price.ToString() + " " + currencyModeText;

                    //gold -> currency
                    break;
                case 1:

                    //check player pref from lucky so that the transaction brings less 15%.
                    price = Math.Round((theValue / currencyPriceInGold)*malusVariation , MidpointRounding.AwayFromZero);
                    latestValue = price;
                    valueText.text = price.ToString() + " G";
                    //currency -> gold
                    break;
            }
            CheckForMinVal(value);
            if(tradingMode ==0)
            {
                CheckForGold(value);
            }
            else if(tradingMode ==1)
            {
                CheckForCurrency(value);
            }
           
        }
        else
        {
            confirmButton.interactable = false;
        }
    }
    private void CheckForMinVal(string val)
    {
        if (Int32.Parse(val) > 0)
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
    private void CheckForGold(string val)
    {
        if (Int32.Parse(val) > MoneyManager.PLAYER_GOLD)
        {
            if (!notEnoughText.enabled)
            {
                if (confirmButton.interactable)
                {
                    confirmButton.interactable = false;
                }
                notEnoughText.enabled = true;
                notEnoughText.text=("You don't have enough Gold left!");
            }
        }
        else
        {
            if (notEnoughText.enabled)
            {
                notEnoughText.enabled = false;
            }
            confirmButton.interactable = true;
        }
    }
    private void CheckForCurrency(string val)
    {
        int theValue = Int32.Parse(val);
        bool notEnough = false;
        string theMessage="";
        switch (currencyMode)
        {
            case 1://d
                if(theValue>MoneyManager.PLAYER_DOLLARS)
                {
                    notEnough = true;
                    theMessage = "You don't have enough dollars left!";
                }
                break;
            case 2://e
                if (theValue > MoneyManager.PLAYER_EUROS)
                {
                    notEnough = true;
                    theMessage = "You don't have enough euros left!";
                }
                break;
            case 3://p
                if (theValue > MoneyManager.PLAYER_POUNDS)
                {
                    notEnough = true;
                    theMessage = "You don't have enough pounds left!";
                }
                break;
            case 4://y
                if (theValue > MoneyManager.PLAYER_YENS)
                {
                    notEnough = true;
                    theMessage = "You don't have enough yens left!";
                }
                break;
        }
        if (notEnough)
        {
            if (!notEnoughText.enabled)
            {
                if (confirmButton.interactable)
                {
                    confirmButton.interactable = false;
                }
                notEnoughText.enabled = true;
                notEnoughText.text=(theMessage);
            }
        }
        else
        {
            if (notEnoughText.enabled)
            {
                notEnoughText.enabled = false;
            }
            confirmButton.interactable = true;
        }
    }
    public void SwitchModes()
    {
        if (buyToggle.isOn)
        {
            
            tradingMode = 0;
            UpdateText(tradingMode);
            Debug.Log("Switching to buy");
        }
        else
        {
            
            tradingMode = 1;
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
    private void ShowAvailabilities()
    {
        availableGoldText.text = $"You have {MoneyManager.PLAYER_GOLD.ToString()}G";
        availableCurrencyText.text = $"You have {currencyToChange.ToString()}{currencyModeText}";
        trendValueText.text = $"1G = {currencyPriceInGold.ToString()}{currencyModeText}";
    }
    private void CheckMin()
    {
        if (MoneyManager.PLAYER_GOLD == 0)
        {
            if (MoneyManager.PLAYER_SAVINGS == 0)
            {
                SwitchModes();//since not enough gold we switch directly to sell
                handleButton.interactable = false;
                float rawValueGold = GetAmountToGiveInGold();
                if (dollarsTrading)
                {
                    if (MoneyManager.PLAYER_DOLLARS == 0)
                    {
                        //transfer 15% of highest amount currency to dollars so :
                        //      currency = 1000£ we take 150£
                        //  150£ -> to gold = x gold and x gold to dollars.
                        penalty = true;
                        float price = (float)myRoom.CustomProperties[DOLLARS_PRICE];
                        float toAdd = (float)Math.Round((rawValueGold / price), MidpointRounding.AwayFromZero);
                        MoneyManager.PLAYER_DOLLARS += toAdd;

                    }
                }
                else if (eurosTrading)
                {
                    if (MoneyManager.PLAYER_EUROS == 0)
                    {
                        penalty = true;
                        float price = (float)myRoom.CustomProperties[EUROS_PRICE];
                        float toAdd = (float)Math.Round((rawValueGold / price), MidpointRounding.AwayFromZero);
                        MoneyManager.PLAYER_EUROS += toAdd;
                    }
                }
                else if (poundsTrading)
                {
                    if (MoneyManager.PLAYER_POUNDS == 0)
                    {
                        penalty = true;
                        float price = (float)myRoom.CustomProperties[POUNDS_PRICE];
                        float toAdd = (float)Math.Round((rawValueGold / price), MidpointRounding.AwayFromZero);
                        MoneyManager.PLAYER_POUNDS += toAdd;
                    }
                }
                else if (yensTrading)
                {
                    if (MoneyManager.PLAYER_YENS == 0)
                    {
                        penalty = true;
                        float price = (float)myRoom.CustomProperties[YEN_PRICE];
                        float toAdd = (float)Math.Round((rawValueGold / price), MidpointRounding.AwayFromZero);
                        MoneyManager.PLAYER_YENS += toAdd;
                    }

                }
                if(penalty)
                {
                    penalty = false;
                    MoneyManager.PLAYER_GOLD -= 500;
                    
                }
                MoneyManager.instance.UpdateAmountText();
                MoneyManager.instance.UpdateMaxCurrency();
            }
            else
            {
                MoneyManager.PLAYER_GOLD = MoneyManager.PLAYER_SAVINGS;
                MoneyManager.PLAYER_SAVINGS = 0;
                savingsText.text=MoneyManager.PLAYER_SAVINGS.ToString() + " G";
                MoneyManager.instance.UpdateAmountText();
            }
        }

        
    }
    private float GetAmountToGiveInGold()
    {
        float amount = 0;
        float value = 0;
        float thePrice = 0; 
        switch (MoneyManager.PLAYER_HIGHEST_CURRENCY_NAME)
        {
            case "d":
                value = (float)Math.Round(MoneyManager.PLAYER_DOLLARS * .15f, MidpointRounding.AwayFromZero);
                MoneyManager.PLAYER_DOLLARS = (float)Math.Round(MoneyManager.PLAYER_DOLLARS * .85f, MidpointRounding.AwayFromZero);
                thePrice = (float)myRoom.CustomProperties[DOLLARS_PRICE];
                amount = (float)Math.Round(thePrice * value, 1);
                break;
            case "e":
                value = (float)Math.Round(MoneyManager.PLAYER_EUROS * .15f, MidpointRounding.AwayFromZero);
                MoneyManager.PLAYER_EUROS = (float)Math.Round(MoneyManager.PLAYER_EUROS * .85f, MidpointRounding.AwayFromZero);
                thePrice = (float)myRoom.CustomProperties[EUROS_PRICE];
                amount = (float)Math.Round(thePrice * value, 1);
                break;
            case "p":
                value = (float)Math.Round(MoneyManager.PLAYER_POUNDS * .15f, 1);
                MoneyManager.PLAYER_POUNDS = (float)Math.Round(MoneyManager.PLAYER_POUNDS * .85f, MidpointRounding.AwayFromZero);
                thePrice = (float)myRoom.CustomProperties[POUNDS_PRICE];
                amount = (float)Math.Round(thePrice * value, 1);
                break;
            case "y":
                value = (float)Math.Round(MoneyManager.PLAYER_YENS * .15f, 1);
                MoneyManager.PLAYER_YENS = (float)Math.Round(MoneyManager.PLAYER_YENS * .85f, MidpointRounding.AwayFromZero);
                thePrice = (float)myRoom.CustomProperties[YEN_PRICE];
                amount = (float)Math.Round(thePrice * value, 1);
                break;
        }
        return amount;
    }  
    
    public void Confirm()
    {
        if (!string.IsNullOrEmpty(myInputField.text))
        {
            int theValue = int.Parse(myInputField.text);
            if (GameManager.malusOn)
            {
                GameManager.malusOn=false;
                malusObject.SetActive(false);
            }
            float newCurr=0;
            switch (tradingMode)
            {
                case 0://buy
                    //remove thevalue of gold
                    MoneyManager.PLAYER_GOLD -= theValue;
                    newCurr = currencyToChange + (float)latestValue;
                    //increase the value of currency
                    //gold -> currency
                    break;
                case 1:
                    MoneyManager.PLAYER_GOLD += (float)latestValue;
                    newCurr = currencyToChange - theValue;
                    //remove the value of currency
                    //increase value of gold
                    //currency -> gold
                    break;
            }
            UpdateMoneyManagerAmounts(currencyMode, newCurr);
            UpdateBankings(theValue, (int)Math.Round(latestValue, MidpointRounding.AwayFromZero), currencyMode, tradingMode);
            myInputField.text = string.Empty;
            buyToggle.isOn = true;
            sellToggle.isOn = false;
            
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
    private void UpdateMoneyManagerAmounts(int mode, float value)
    {
        //1 dollars
        //2 euros
        //3 pounds
        //4 yens
        switch(mode)
        {
            case 1:
                MoneyManager.PLAYER_DOLLARS = value;
                break;
            case 2:
                MoneyManager.PLAYER_EUROS = value;
                break;
            case 3:
                MoneyManager.PLAYER_POUNDS = value;
                break;
            case 4:
                MoneyManager.PLAYER_YENS = value;
                break;
        }
        MoneyManager.instance.UpdateFortuneInGame();
    }
    private void UpdateBankings(int toAdd, int toRemove, int currency, int tradeMode)
    {
      
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
