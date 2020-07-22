using UnityEngine;
using TMPro;
using System;
using Photon.Realtime;
using System.Collections.Generic;

//single player
public class TaxesManager : MonoBehaviour
{
    
    //fortune 0 - 10,000 = 5% = 1% each monney gets decreased.
    //fortune 10,001 - 20,000 = 10% = 2% each money gets decreased.
    //fortune 20,001 - 8 = 15% = 3% each money gets decreased.
    //if no more of one currency take on the gold.
    
    private const int TOTAL_MONEY_TYPE = 5;
    private const string EUROS_PRICE = "Euros_Price";
    private const string DOLLARS_PRICE = "Dollars_Price";
    private const string POUNDS_PRICE = "Pounds_Price";
    private const string YEN_PRICE = "Yen_Price";
    private const string PREFDICE = "DiceVal";
    private const string POSITION_INDEX_PREF_KEY = "myPositionIndex";
    private const int BACK_POS_1 = -4;
    private const int BACK_POS_2 = -6;

    private Room myRoom;
    private const string FORTUNE = "myFortune";
    private float generalDollars,generalEuros,generalYens,generalPounds,priceE, priceD,priceP,priceY,playerE, playerD, playerP, playerY, playerG, playerFortune,taxesPercent;
    public TMP_Text content,buttonContent;
    public static bool BeginProcess;
    public static int status;
    private int dice1, dice2;
    public static List<int> values = new List<int>();
    public GameObject subPanel;
    // Start is called before the first frame update
    void Start()
    {
        myRoom = GameManager.myRoom;
        dice1 = dice2 = 0;
    }
    private void Update()
    {
        if(BeginProcess)
        {
            BeginProcess = false;
            Initialise();
        }
        if(status==2)
        {
            
            status = 0;
            dice1 = values[0];
            dice2 = values[1];
         
            if (dice1 != dice2)
            {
                MoveBack();
                dice1 = 0;
                dice2 = 0;
            }
            else
            {
                Done();//double done so can keep playing.
                //add some effects here.
            }
            
        }
    }
    private void Initialise()
    {
        GetPrefs();
        GetPrices();
        generalEuros = GetGeneralPrices(priceE, playerE);
        generalDollars = GetGeneralPrices(priceD, playerD);
        generalPounds = GetGeneralPrices(priceP, playerP);
        generalYens = GetGeneralPrices(priceY, playerY);

        if (playerFortune <= 10000)
        {
            taxesPercent = .05f;
        }
        else if (playerFortune > 10000 && playerFortune <= 20000)
        {
            taxesPercent = .1f;
        }
        else if (playerFortune > 20000)
        {
            taxesPercent = .15f;
        }
        float percent = taxesPercent * 100;
     
        double total = Math.Round(playerFortune * taxesPercent, 2);
        content.text = $"The Police is here! You haven't paid your taxes in a long time. " +
            $"Pay {total.ToString()} Gold or roll the dices to get a chance to escape. " +
            $"If you miss you go backwards!";
        buttonContent.text = $"Pay {total.ToString()}";

    }


    //to properly substract taxes : we need to bring every currencies back to a 1 on 1 proportion.
    //once it's done, then we substract the tax. then we call update fortune on moneymanager which will update the player's fortune
    //based on the new prices set here.
    public void SubstractTaxes()
    {
        float toRemove = 1-(taxesPercent / TOTAL_MONEY_TYPE); // EITHER 1% 2% 3% per currencies.
    
        float oldD = generalDollars;
        float oldE = generalEuros;
        float oldG = playerG;
        float oldP = generalPounds;
        float oldY = generalYens;

        float removedG, removedD, removedE, removedP, removedY;

        if (generalDollars * toRemove > 0)
        {
            generalDollars = generalDollars * toRemove;
            removedD = oldD - generalDollars;

        }
        else
        {
   
            removedD = 0;
            playerG *= toRemove;
        }
        if (generalEuros * toRemove > 0)
        {

            generalEuros *= toRemove;
            removedE = oldE - generalEuros;
   
        }
        else
        {
   
            removedE = 0;
            playerG = playerG * toRemove;
        }
        if (generalPounds * toRemove > 0)
        {
            generalPounds *= toRemove;
            removedP = oldP - generalPounds;
    
        }
        else
        {
        
            removedP = 0;
            playerG = playerG * toRemove;
        }
        if (generalYens * toRemove > 0)
        {
           
            generalYens *= toRemove;
            removedY = oldY - generalYens;
      
        }
        else
        {
      
            removedY = 0;
            playerG *= toRemove;
        }
        playerG *= toRemove;
        
        removedG = oldG - playerG;
        

        generalDollars = (float)(Math.Round(generalDollars,1));
        generalEuros= (float)(Math.Round(generalEuros, 1));
        generalPounds = (float)(Math.Round(generalPounds, 1));
        generalYens = (float)(Math.Round(generalYens, 1));
        playerG = (float)(Math.Round(playerG, 1));

        SetPrefs(generalDollars, generalEuros, generalPounds, generalYens, playerG);
        MoneyManager.instance.UpdateFortuneInGame();
        BankManager.instance.TaxIncome(removedG, removedD, removedE, removedP, removedY);
        

    }

    //here we call gamemanager roll with taxes roll set to true
    public void MoveBack()
    {
        Debug.Log("moving back: " + PlayerPrefs.GetInt(POSITION_INDEX_PREF_KEY));

            //set pref dice to minus
            if(PlayerPrefs.GetInt(POSITION_INDEX_PREF_KEY)==18)
            {
                PlayerPrefs.SetInt(PREFDICE, BACK_POS_1);
           
                MovementManager.backWards = true;
                MovementManager.moveMe = true;
            }
            else if(PlayerPrefs.GetInt(POSITION_INDEX_PREF_KEY)==27)
            {
                PlayerPrefs.SetInt(PREFDICE, BACK_POS_2);
          
                MovementManager.backWards = true;
                MovementManager.moveMe = true;
            }
        //go backwards
        //if index == 27 -> go to 21
        //if index == 18 -> go to 14
        Done();
    }
    public void Done()
    {
       
        BoardManager.NextTurn();
        gameObject.SetActive(false);
        subPanel.SetActive(true);
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
        playerFortune = MoneyManager.PLAYER_FORTUNE;
    
        playerE = MoneyManager.PLAYER_EUROS;
        playerD = MoneyManager.PLAYER_DOLLARS;
        playerP = MoneyManager.PLAYER_POUNDS;
        playerY = MoneyManager.PLAYER_YENS;
        playerG = MoneyManager.PLAYER_GOLD;
    }
    private void SetPrefs(float d, float e, float p, float y,float g)
    {
        Debug.Log($"TO SET : {d},{e},{p},{g}");
        MoneyManager.PLAYER_EUROS= e;
        MoneyManager.PLAYER_DOLLARS= d;
        MoneyManager.PLAYER_YENS= y;
        MoneyManager.PLAYER_POUNDS= p;
        MoneyManager.PLAYER_GOLD= g;
    }
 
    
}
