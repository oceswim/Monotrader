
using UnityEngine;
using TMPro;
using Photon.Realtime;
using System;
using Photon.Pun;

public class BankManager : MonoBehaviourPun
{
    //private variable
    private const int INITIAL_GOLD = 5000;
    private const int INITIAL_CURRENCIES = 2500;
    private GameObject GoldBank, DollarsBank, EurosBank, PoundsBank, YensBank;
    private BankScript goldB, dollarsB, eurosB, poundsB, yensB;
    private Room myRoom;
    private Player myPlayer;
    private int playerCount;
    private const string EUROS_SYMBOL = "€";
    private const string DOLARS_SYMBOL = "$";
    private const string POUNDS_SYMBOL = "P";
    private const string YENS_SYMBOL = "Y";
    private const string GOLD_SYMBOL = "G";

    private bool UpdateOnce;

    public static bool Trigger;
    //public variables
    public const string GOLD_BANK = "GoldBank";
    public const string DOLLARS_BANK = "DollarsBank";
    public const string EUROS_BANK = "EurosBank";
    public const string POUNDS_BANK = "PoundsBank";
    public const string YENS_BANK = "YensBank";

    public const string GOLD_UPDATE = "GoldUpdate";
    public const string DOLLARS_UPDATE = "DollarsUpdate";
    public const string EUROS_UPDATE = "EurosUpdate";
    public const string POUNDS_UPDATE = "PoundsUpdate";
    public const string YENS_UPDATE = "YensUpdate";
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

        myPlayer = PhotonNetwork.LocalPlayer;
        myRoom = GameManager.myRoom;
        
        playerCount = myRoom.PlayerCount;
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

    private void Update()
    {
        if (Trigger)
        {
            Trigger = false;

            int g = CheckValue(GOLD_UPDATE);
            int d = CheckValue(DOLLARS_UPDATE);
            int e = CheckValue(EUROS_UPDATE);
            int p = CheckValue(POUNDS_UPDATE);
            int y = CheckValue(YENS_UPDATE);
            UpdateMyGUI(g, d, e, p, y);
         
            BoardManager.NextTurn();
            
                
        }
           
  
        
    }
    private int CheckValue(string key)
    {
        int val;
        if (myRoom.CustomProperties[key] != null)
        {
            if ((int)myRoom.CustomProperties[key] == 1)
            {
                val = (int)myRoom.CustomProperties[key];
            }
            else
            {
                val = 0;
            }
        }
        else
        {
            val = 0;
        }
        return val;
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
        SetTextAndInitialProperty(g.amount, d.amount, e.amount, p.amount, y.amount,myPlayer.IsMasterClient);
    }
    // Update is called once per frame

    private void SetTextAndInitialProperty(int g,int d,int e, int p,int y,bool masterClient)
    {

        dollarsText.text = d.ToString() + DOLARS_SYMBOL;

        goldText.text = g.ToString() + GOLD_SYMBOL;

        eurosText.text = e.ToString() + EUROS_SYMBOL;

        yenText.text = y.ToString() + YENS_SYMBOL;

        poundsText.text = p.ToString() + POUNDS_SYMBOL;
        if(masterClient)
        {        
            SetRoomProperty(EUROS_BANK, e);
            SetRoomProperty(POUNDS_BANK, p);
            SetRoomProperty(YENS_BANK, y);
            SetRoomProperty(GOLD_BANK, g);        
            SetRoomProperty(DOLLARS_BANK, d);
        }
            
    }
    public void UpdateGold(int val)
    {
        //if another playe rupdate it we need to synchronise the amount before update.
        if (myRoom.CustomProperties[GOLD_BANK] != null)
        {
            if (goldB.amount != (int)myRoom.CustomProperties[GOLD_BANK])
            {
                goldB.amount = (int)myRoom.CustomProperties[GOLD_BANK];
            }
        }
        goldB.UpdateAmount(val);
        SetRoomProperty(GOLD_BANK, goldB.amount);
        SetRoomProperty(GOLD_UPDATE, 1);
        goldText.text = goldB.amount.ToString() + GOLD_SYMBOL;
 
    }
    public void UpdateDollars(int val)
    {
        if (myRoom.CustomProperties[DOLLARS_BANK] != null)
        {
            if (dollarsB.amount != (int)myRoom.CustomProperties[DOLLARS_BANK])
            {
                dollarsB.amount = (int)myRoom.CustomProperties[DOLLARS_BANK];
            }
        }
        dollarsB.UpdateAmount(val);
        SetRoomProperty(DOLLARS_BANK, dollarsB.amount);
        SetRoomProperty(DOLLARS_UPDATE,1);
        dollarsText.text = dollarsB.amount.ToString() + DOLARS_SYMBOL;
    }
    public void UpdateEuros(int val)
    {
        if (myRoom.CustomProperties[EUROS_BANK] != null)
        {
            if (eurosB.amount != (int)myRoom.CustomProperties[EUROS_BANK])
            {
                eurosB.amount = (int)myRoom.CustomProperties[EUROS_BANK];
            }
        }
        eurosB.UpdateAmount(val);
        SetRoomProperty(EUROS_BANK, eurosB.amount);
        SetRoomProperty(EUROS_UPDATE, 1);
        eurosText.text = eurosB.amount.ToString() + EUROS_SYMBOL;
    }
    public void UpdatePounds(int val)
    {
        if (myRoom.CustomProperties[POUNDS_BANK] != null)
        {
            if (poundsB.amount != (int)myRoom.CustomProperties[POUNDS_BANK])
            {
                poundsB.amount = (int)myRoom.CustomProperties[POUNDS_BANK];
            }
        }
        poundsB.UpdateAmount(val);
        SetRoomProperty(POUNDS_BANK, poundsB.amount);
        SetRoomProperty(POUNDS_UPDATE,1);
        poundsText.text = poundsB.amount.ToString() + POUNDS_SYMBOL;
    }

    public void UpdateYens(int val)
    {
        if (myRoom.CustomProperties[YENS_BANK] != null)
        {
            if (yensB.amount != (int)myRoom.CustomProperties[YENS_BANK])
            {
                yensB.amount = (int)myRoom.CustomProperties[YENS_BANK];
            }
        }
        yensB.UpdateAmount(val);
        SetRoomProperty(YENS_BANK, yensB.amount);
        SetRoomProperty(YENS_UPDATE, 1);
        yenText.text = yensB.amount.ToString() + YENS_SYMBOL;
    }

    //if the int are set to 1 then we update the text. otherwise it was unchanged so we leave as it is
    private void UpdateMyGUI(int gold,int dollars,int euros,int pounds,int yens)
    {
        string g, d, e, p, y;
        if (gold == 1)
        {
             g = myRoom.CustomProperties[GOLD_BANK].ToString() + GOLD_SYMBOL;
            SetRoomProperty(GOLD_UPDATE, 0);
        }
        else
        {
            g = "";
        }
        if (dollars == 1)
        {
             d = myRoom.CustomProperties[DOLLARS_BANK].ToString() + DOLARS_SYMBOL;
            SetRoomProperty(DOLLARS_UPDATE, 0);
        }
        else
        {
            d = "";
        }
        if (euros == 1)
        {
             e = myRoom.CustomProperties[EUROS_BANK].ToString() + EUROS_SYMBOL;
            SetRoomProperty(EUROS_UPDATE, 0);
        }
        else
        {
            e = "";
        }
        if (pounds == 1)
        {
             p = myRoom.CustomProperties[POUNDS_BANK].ToString() + POUNDS_SYMBOL;
            SetRoomProperty(POUNDS_UPDATE, 0);
        }
        else
        {
            p = "";
        }
        if (yens == 1)
        {
             y = myRoom.CustomProperties[YENS_BANK].ToString() + YENS_SYMBOL;
            SetRoomProperty(YENS_UPDATE, 0);
        }
        else
        {
            y = "";
        }
  
        if (!photonView.IsMine)
        {
            photonView.TransferOwnership(myPlayer);
        
        }
        photonView.RPC("SetBankText", RpcTarget.AllBuffered, g,d,e,p,y);
    }

    public void TaxIncome(float gold, float dollars,float euros,float pounds, float yens)
    {

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
    private void SetCustomsPPT(string hashKeyIndex, int ind)
    {
        int playerIndex = ind;
        if (GameManager._myCustomProperty[hashKeyIndex] != null)
        {
            GameManager._myCustomProperty[hashKeyIndex] = playerIndex;
        }
        else
        {
            GameManager._myCustomProperty.Add(hashKeyIndex, playerIndex);
        }
        PhotonNetwork.LocalPlayer.CustomProperties = GameManager._myCustomProperty;
        PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);

    }

    [PunRPC]
    private void SetBankText(string goldT,string dollarsT,string eurosT,string poundsT,string yensT)
    {

        if (!string.IsNullOrEmpty(goldT))
        {
            goldText.text = goldT;
        }
        if(!string.IsNullOrEmpty(dollarsT))
        {
 
            dollarsText.text = dollarsT;
        }
        if(!string.IsNullOrEmpty(eurosT))
        {
            eurosText.text = eurosT;
        }
        if(!string.IsNullOrEmpty(poundsT))
        {
            poundsText.text = poundsT;
        }
        if(!string.IsNullOrEmpty(yensT))
        {
            yenText.text = yensT;
        }
       
        
        

    }
}
