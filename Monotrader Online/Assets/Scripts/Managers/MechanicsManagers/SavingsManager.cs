using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

//focused on 1 player
public class SavingsManager : MonoBehaviour
{
    private const string FORTUNE = "myFortune";
    private const string MY_SAVINGS = "mySavings";
    private double percentage;
    private float myGold,myFortune;
    public TMP_Text savingsText,totalSavings;
    private string PLAYER_GOLD;
    public Button yesButton, noButton;
    public static bool BeginProcess;
    // Start is called before the first frame update
    void Start()
    {
        PLAYER_GOLD = PlayerPrefs.GetString("MYGOLD"); 
        
    }
    private void Update()
    {
        if(BeginProcess)
        {
            myGold = PlayerPrefs.GetFloat(PLAYER_GOLD);
            myFortune = PlayerPrefs.GetFloat(FORTUNE);
            SetSavingsText();
            BeginProcess = false;
        }
    }
    private void SetSavingsText()
    {
        percentage = Math.Round(myGold * .15f, 1);
        string text = $"You can place 15% of your gold to your savings." +
            $"It represents {percentage.ToString()} Gold."+
            $"Do you want to ?";
        savingsText.text = text;
        yesButton.interactable = true;
        noButton.interactable = true;
    }
    public void Accept()
    {
        if (PlayerPrefs.HasKey(MY_SAVINGS))
        {
          float oldPercentage =  PlayerPrefs.GetFloat(MY_SAVINGS);
          float newPercentage = oldPercentage + (float)percentage;
          PlayerPrefs.SetFloat(MY_SAVINGS, newPercentage);
          Debug.Log($"SAVINGS before :{oldPercentage.ToString()}, savings now : {newPercentage.ToString()}");
        }
        else
        {
            PlayerPrefs.SetFloat(MY_SAVINGS, (float)percentage);
            Debug.Log($"SAVINGS set :{PlayerPrefs.GetFloat(MY_SAVINGS).ToString()}");
        }
        totalSavings.text = PlayerPrefs.GetFloat(MY_SAVINGS).ToString()+ " G";
        float newGold = myGold - (float)percentage;
        PlayerPrefs.SetFloat(PLAYER_GOLD, newGold);
        MoneyManager.updateFortune = true;
    }
}
