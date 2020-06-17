using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using TMPro;
using Photon.Realtime;
using System;

public class BankManager : MonoBehaviour
{
    //private variable
    private const int INITIAL_GOLD = 5000;
    private const int INITIAL_CURRENCIES = 2500;
    private GameObject GoldBank, DollarsBank, EurosBank, PoundsBank, YensBank;
    private BankScript goldB, dollarsB, eurosB, poundsB, yensB;
    private Room myRoom;
    private int playerCount;
    private const string EUROS_SYMBOL = "€";
    private const string DOLARS_SYMBOL = "$";
    private const string POUNDS_SYMBOL = "P";
    private const string YENS_SYMBOL = "Y";
    private const string GOLD_SYMBOL = "G";

    //public variables
    public const string GOLD_BANK = "GoldBank";
    public const string DOLLARS_BANK = "DollarsBank";
    public const string EUROS_BANK = "EurosBank";
    public const string POUNDS_BANK = "PoundsBank";
    public const string YENS_BANK = "YensBank";
    public TMP_Text goldText, dollarsText, poundsText, yenText, eurosText;


    public static BankManager instance = null;
   
    private void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

    }
    // Start is called before the first frame update
    void Start()
    {

        myRoom = GameManager.myRoom;
        playerCount = myRoom.PlayerCount;
        Debug.Log("PLAYER COUNT " + playerCount);
        GoldBank = new GameObject(GOLD_BANK);
        GoldBank.AddComponent<BankScript>();
        
        EurosBank = new GameObject(EUROS_BANK);
        EurosBank.AddComponent<BankScript>();
        
        DollarsBank = new GameObject(DOLLARS_BANK);
        DollarsBank.AddComponent<BankScript>();
        
        PoundsBank = new GameObject(POUNDS_BANK);
        PoundsBank.AddComponent<BankScript>();
        
        YensBank = new GameObject(YENS_BANK);
        YensBank.AddComponent<BankScript>();

        InitialiseBanks(GoldBank, DollarsBank, EurosBank, PoundsBank, YensBank);
        
    }
    private void InitialiseBanks(GameObject g,GameObject d, GameObject e,GameObject p,GameObject y)
    {
        goldB= g.GetComponent<BankScript>();
        dollarsB = d.GetComponent<BankScript>();
        eurosB = e.GetComponent<BankScript>();
        poundsB = p.GetComponent<BankScript>();
        yensB = y.GetComponent<BankScript>();
        SetInitialAmount(goldB, dollarsB, eurosB, poundsB, yensB);

    }
    private void SetInitialAmount(BankScript g, BankScript d, BankScript e, BankScript p, BankScript y)
    {
        g.amount = INITIAL_GOLD * playerCount;
        d.amount = e.amount = p.amount = y.amount = INITIAL_CURRENCIES * playerCount;
        SetText(g.amount, d.amount, e.amount, p.amount, y.amount);
    }
    // Update is called once per frame

    private void SetText(int g,int d,int e, int p,int y)
    {
        dollarsText.text = d.ToString() + DOLARS_SYMBOL;
        goldText.text = g.ToString() + GOLD_SYMBOL;
        eurosText.text = e.ToString() + EUROS_SYMBOL;
        yenText.text = y.ToString() + YENS_SYMBOL;
        poundsText.text = p.ToString() + POUNDS_SYMBOL;
            
    }
    public void UpdateGold(int val)
    {
        goldB.UpdateAmount(val);
        SetRoomProperty(GOLD_BANK, goldB.amount);
        goldText.text = goldB.amount.ToString() + GOLD_SYMBOL;
 
    }
    public void UpdateDollars(int val)
    {
        dollarsB.UpdateAmount(val);
        SetRoomProperty(DOLLARS_BANK, dollarsB.amount);
        dollarsText.text = dollarsB.amount.ToString() + DOLARS_SYMBOL;
    }
    public void UpdateEuros(int val)
    {

        eurosB.UpdateAmount(val);
        SetRoomProperty(EUROS_BANK, eurosB.amount);
        eurosText.text = eurosB.amount.ToString() + EUROS_SYMBOL;
    }
    public void UpdatePounds(int val)
    {
        poundsB.UpdateAmount(val);
        SetRoomProperty(POUNDS_BANK, poundsB.amount);
        poundsText.text = poundsB.amount.ToString() + POUNDS_SYMBOL;
    }

    public void UpdateYens(int val)
    {
        yensB.UpdateAmount(val);
        SetRoomProperty(YENS_BANK, yensB.amount); 
        yenText.text = yensB.amount.ToString() + YENS_SYMBOL;
    }


    public void TaxIncome(float gold, float dollars,float euros,float pounds, float yens)
    {
        Debug.Log($"After taxes the bank gets : {gold}G {dollars}D {euros}E {pounds}P {yens}Y");
        Debug.Log($"converted to int : {(int)Math.Round(gold, 0)}G {(int)Math.Round(dollars, 0)}D {(int)Math.Round(euros, 0)}E {(int)Math.Round(pounds, 0)}P {(int)Math.Round(yens, 0)}Y");
        if(gold>0)
        {
            UpdateGold((int)Math.Round(gold, 0));
        }
        if(dollars>0)
        {
            UpdateDollars((int)Math.Round(dollars, 0));
        }
        if(euros>0)
        {
            UpdateEuros((int)Math.Round(euros, 0));
        }
        if(pounds>0)
        {
            UpdateEuros((int)Math.Round(pounds, 0));
        }
        if(yens>0)
        {
            UpdateYens((int)Math.Round(yens, 0));
        }
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
