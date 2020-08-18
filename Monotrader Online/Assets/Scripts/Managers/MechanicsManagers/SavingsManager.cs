using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

//focused on 1 player
public class SavingsManager : MonoBehaviour
{
    private double percentage;
    private float myGold,myFortune;
    public TMP_Text savingsText,totalSavings;
    public Button yesButton, noButton;
    public static bool BeginProcess,updateSavings;
    public GameObject savingsObject;
    // Start is called before the first frame update
  
    private void Update()
    {
        if(BeginProcess)
        {
   
            myGold = MoneyManager.PLAYER_GOLD;
            myFortune = MoneyManager.PLAYER_FORTUNE;
            SetSavingsText();
            BeginProcess = false;
        }
        if(updateSavings)
        {
            updateSavings = false;
            totalSavings.text = MoneyManager.PLAYER_SAVINGS.ToString() + " G";
        }

        

    }
    private void SetSavingsText()
    {
        percentage = Math.Round(myGold * .15f, MidpointRounding.AwayFromZero);
        string text = $"You can place 15% of your gold to your savings." +
            $"It represents {percentage.ToString()} Gold."+
            $"Do you want to ?";
        savingsText.text = text;
        yesButton.interactable = true;
        noButton.interactable = true;
    }
    public void Accept()
    {
        
        if (MoneyManager.PLAYER_SAVINGS==0)
        {
          float oldPercentage = MoneyManager.PLAYER_SAVINGS;
          float newPercentage = oldPercentage + (float)percentage;
          MoneyManager.PLAYER_SAVINGS =newPercentage;
          
        }
        else
        {
            MoneyManager.PLAYER_SAVINGS=(float)percentage;
           
        }
        totalSavings.text = MoneyManager.PLAYER_SAVINGS.ToString()+ " G";
        float newGold = myGold - (float)percentage;
        MoneyManager.PLAYER_GOLD= newGold;
        MoneyManager.instance.UpdateFortuneInGame();
        
    }
    public void Done()
    {
        savingsObject.SetActive(true);
        BoardManager.NextTurn();
    }
}
