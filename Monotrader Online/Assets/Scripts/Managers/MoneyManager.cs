using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class MoneyManager : MonoBehaviourPunCallBacks
{

    private const float INITIAL_GOLD = 5000;
    private const float INITIAL_CURRENCIES = 2500;


    private const string EUROS_PRICE = "Euros_Price";
    private const string DOLLARS_PRICE = "Dollars_Price";
    private const string POUNDS_PRICE = "Pounds_Price";
    private const string YEN_PRICE = "Yen_Price";

    private const string TRENDUPDATECOUNTER = "trendCount";
    private const string HISTORY_UPDATE = "History_Update";
    private const string UPDATE_DONE = "UpdateDone";
    private const string HISTORY_STATUS = "History_Status";
    private const string PLAYER_STATE = "My_State";
    private const string CHAR_SELECTION = "CharSelected";

    private const string EUROS_TREND = "Euros_Trend";
    private const string DOLLARS_TREND = "Dollars_Trend";
    private const string POUNDS_TREND = "Pounds_Trend";
    private const string YEN_TREND = "Yen_Trend";

    private const string EUROS_TREND_IND = "Euros_Trend_Ind";
    private const string DOLLARS_TREND_IND = "Dollars_Trend_Ind";
    private const string POUNDS_TREND_IND = "Pounds_Trend_Ind";
    private const string YEN_TREND_IND = "Yen_Trend_Ind";

    private float[] TRENDS_VALUES;

    private const string GAME_STATE_HASHKEY = "gameState";


    private const string HISTORY_TURN_ACTUAL = "History_Turn_Actual";
    private string HISTORY_TURN_M1 = "History_Turn_Minus1";
    private string HISTORY_TURN_M2 = "History_Turn_Minus2";
    private string HISTORY_TURN_M3 = "History_Turn_Minus3";
    private int trendUpdateActivator = 0;
    private float myGold, myDollars, myEuros, myYens, myPounds;

    private Player myPlayer;
    private Room myRoom;
    private bool playersReady,updateOnceFortune, updateOnceGUI,setUpOnceTrend;

    private List<Player> notReadyPlayers = new List<Player>();

    public TMP_Text goldAmount, dollarsAmount, eurosAmount, poundsAmount, yenAmount, waitingForText, totalFortuneText;
    public GameObject waitingForObject,sectionsManagerObject,resetObject;
    public Animator TrendAnimator;
    public TMP_Text[] dollarsTrendInGame, eurosTrendInGame, poundsTrendInGame, yensTrendInGame, dHistory, eHistory, pHistory, yHistory;

    public static float PLAYER_GOLD;
    public static float PLAYER_DOLLARS;
    public static float PLAYER_EUROS;
    public static float PLAYER_YENS;
    public static float PLAYER_POUNDS;
    public static float PLAYER_FORTUNE;
    public static float PLAYER_SAVINGS;
    public static string PLAYER_HIGHEST_CURRENCY_NAME;
    public static float PLAYER_HIGHEST_CURRENCY_VALUE;


    public static bool newTurnFortune;
    public static MoneyManager instance = null;
    // Start is called before the first frame update
    private void Awake()
    {
        myPlayer = PhotonNetwork.LocalPlayer;
      
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        
    }
    void Start()
    {

        PopulateTrendsList();
        playersReady =setUpOnceTrend= false;
        myRoom = GameManager.myRoom;
        InitialiseHistoryPrefs();

    }

    // Update is called once per frame
    void Update()
    {
        //if all the players are in the room
        if (PhotonNetwork.PlayerList != null && PhotonNetwork.CurrentRoom !=null)
        {
            if (PhotonNetwork.PlayerList.Length == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                if (myRoom.CustomProperties[GAME_STATE_HASHKEY] == null)
                {
                    string waitingText = "Waiting for ";
                    foreach (Player p in PhotonNetwork.PlayerListOthers)
                    {

                        if (p.CustomProperties[CHAR_SELECTION].Equals("-1"))
                        {

                            if (!notReadyPlayers.Contains(p))
                            {
                                notReadyPlayers.Add(p);

                            }
                            if (notReadyPlayers.IndexOf(p).Equals(notReadyPlayers.Count - 1))
                            {
                                waitingText += p.NickName + "....";
                            }
                            else
                            {
                                waitingText += p.NickName + ", ";
                            }
                        }
                        else
                        {
                            if (notReadyPlayers.Contains(p))
                            {
                                notReadyPlayers.Remove(p);
                            }

                        }
                    }
                    if (!waitingForText.text.Equals(waitingText))
                    {
                        waitingForText.text = waitingText;
                    }
                    if(notReadyPlayers.Count<1)
                    {
                        waitingForObject.SetActive(false);
                    }

                }
                else if (myRoom.CustomProperties[GAME_STATE_HASHKEY] != null && !playersReady)
                {
                    if ((int)myRoom.CustomProperties[GAME_STATE_HASHKEY] == 1)
                    {
                        playersReady = true;
                        waitingForObject.SetActive(false);
                        SetInitialAmounts(myPlayer.IsMasterClient);
                    
                        sectionsManagerObject.SetActive(true);

                    }
                }

                if (myRoom.CustomProperties[HISTORY_STATUS] != null)
                {

                    if ((int)myRoom.CustomProperties[HISTORY_STATUS] == 1)
                    {
                        if ((int)myPlayer.CustomProperties[PLAYER_STATE] == 0 && !setUpOnceTrend)
                        {
                            setUpOnceTrend = true;
                            SetCustomsPPT(PLAYER_STATE, 1);
                            Wait(2);
                            if (updateOnceFortune)
                            {

                                UpdateFortune();
                                updateOnceFortune = false;

                            }
                            

                            UpdateTrendDisplay();
                            UpdateHistoryGUI();


                        }
                        if ((int)myPlayer.CustomProperties[PLAYER_STATE] == 1 && setUpOnceTrend)
                        {

                            if (myPlayer.IsMasterClient)
                            {
                                bool allReady = true;
                                foreach (Player p in PhotonNetwork.PlayerListOthers)
                                {
                                    if ((int)p.CustomProperties[PLAYER_STATE] != 1)
                                    {
                                        allReady = false;
                                    }
                                }
                                if (allReady)
                                {
                                    setUpOnceTrend = false;
                                    SetRoomHistoryStatus(0);
                                    SetCustomsPPT(PLAYER_STATE, 0);
                                    foreach (Player p in PhotonNetwork.PlayerListOthers)
                                    {
                                        SetCustomsPPT(PLAYER_STATE, 0, p);
                                    }

                                }
                            }


                        }
                    }


                }


                if (newTurnFortune)
                {
                    newTurnFortune = false;
                    UpdateFortune();

                }

                if (myRoom.CustomProperties[HISTORY_UPDATE] != null)
                {
                    if ((int)myRoom.CustomProperties[HISTORY_UPDATE] == 1 && !updateOnceGUI)
                    {
                        if (!updateOnceGUI)
                        {
                            SetCustomsPPT(UPDATE_DONE, 1);
                            UpdateHistoryGUI();
                            updateOnceGUI = true;
                        }
                        if (myPlayer.IsMasterClient)
                        {
                            bool done = true;
                            foreach (Player p in PhotonNetwork.PlayerList)
                            {
                                if (p.CustomProperties[UPDATE_DONE] != null)
                                {
                                    if ((int)p.CustomProperties[UPDATE_DONE] != 1)
                                    {
                                        done = false;
                                    }
                                }
                                else
                                {
                                    done = false;
                                }
                            }
                            if (done)
                            {
                                SetRoomProperty(HISTORY_UPDATE, 0);
                            }
                        }


                    }
                    else
                    {
                        if (updateOnceGUI)
                        {

                            SetCustomsPPT(UPDATE_DONE, 0);
                            updateOnceGUI = false;
                        }
                    }
                }

            }

            
        }
        if (resetObject.activeSelf)
        {
            Debug.Log("Blink t true");
            TrendAnimator.SetBool("BlinkT", true);

        }
        else
        {
            if (TrendAnimator.GetBool("BlinkT"))
            {
                TrendAnimator.SetBool("BlinkT", false);
            }
        }

    }

    //the trends list for each currency, from .9 to 1.1 value
    private void PopulateTrendsList()
    {
        TRENDS_VALUES = new float[21];
        double i = .9;
        int index = 0;
        while(i<1.1)
        {
            TRENDS_VALUES[index] = (float)(Math.Round(i,2));

            i += .01f;
            index++;
            
        }
      
    }

    private void InitialiseHistoryPrefs()
    {
        HISTORY_TURN_M3= "D/1/0_E/1/0_P/1/0_Y/1/0";
        HISTORY_TURN_M2= "D/1/0_E/1/0_P/1/0_Y/1/0";
        HISTORY_TURN_M1= "D/1/0_E/1/0_P/1/0_Y/1/0";
    }
    //the initial prices of each currency at the beginning of the game
    private void SetInitialAmounts(bool masterClient)
    {
        PLAYER_SAVINGS = 0;//TO PUT TO 0 !
        
        myGold = INITIAL_GOLD;
 
        myEuros = myDollars = myPounds = myYens = INITIAL_CURRENCIES;
        


        PLAYER_GOLD = myGold;
        PLAYER_DOLLARS= myDollars;
        PLAYER_EUROS= myEuros;
        PLAYER_POUNDS= myPounds;
        PLAYER_YENS= myYens;

        PLAYER_FORTUNE = PLAYER_GOLD + PLAYER_DOLLARS + PLAYER_EUROS + PLAYER_POUNDS + PLAYER_YENS;

        float initD;
        float initE;
        float initP;
        float initY;
        initD = initE = initP = initY = 1;

        if (masterClient)
        {

            SetRoomPrices(initD, initE, initP, initY);
            PrepareTrends(false);//sets up the price trends

        }
        updateOnceFortune = true;
    }

    //the room history allows to set if the history was updated for all player (1) if not (0)
    private void SetRoomHistoryStatus(int ind)
    {
        SetRoomProperty(HISTORY_STATUS, ind);
    }


    //allows to set each currencies prices to the room properties
    private void SetRoomPrices(float dol, float eur, float pou, float yen)
    {

        SetRoomProperty(EUROS_PRICE, eur);
        SetRoomProperty(DOLLARS_PRICE, dol);
        SetRoomProperty(POUNDS_PRICE, pou);
        SetRoomProperty(YEN_PRICE, yen);
    }

    //allows to set each currencies trends variation to the room properties
    private void SetRoomTrends(float d, float e, float p, float y)
    {

        SetRoomProperty(DOLLARS_TREND, d);
        SetRoomProperty(EUROS_TREND, e);
        SetRoomProperty(POUNDS_TREND, p);
        SetRoomProperty(YEN_TREND, y);
    }

    //allows to update the UI for each player
    private void UpdateTrendDisplay()
    {

        int d = (int)myRoom.CustomProperties[DOLLARS_TREND_IND];
        int e = (int)myRoom.CustomProperties[EUROS_TREND_IND];
        int p = (int)myRoom.CustomProperties[POUNDS_TREND_IND];
        int y = (int)myRoom.CustomProperties[YEN_TREND_IND];

        //change when board gets reset. HEEERRE
        int newCount = 0;
        if(myRoom.CustomProperties[TRENDUPDATECOUNTER]!=null)
        {
            newCount = (int)myRoom.CustomProperties[TRENDUPDATECOUNTER]+1;
            SetRoomProperty(TRENDUPDATECOUNTER, newCount);
        }
        else
        {
            newCount++;
            SetRoomProperty(TRENDUPDATECOUNTER, newCount);
        }
  
        //if more than 6 trends are off we reset the board
        if (newCount > 6)
        {

            SetRoomProperty(TRENDUPDATECOUNTER, 0);
            for (int i = 0; i < dollarsTrendInGame.Length; i++)
            {
                
               dollarsTrendInGame[i].enabled = true;
               eurosTrendInGame[i].enabled = true;
               poundsTrendInGame[i].enabled = true;
               yensTrendInGame[i].enabled = true;
                
            }
            
             resetObject.SetActive(true);
                
            
        }

        dollarsTrendInGame[d].enabled = false;
        eurosTrendInGame[e].enabled = false;
        poundsTrendInGame[p].enabled = false;
        yensTrendInGame[y].enabled = false;
    
    }
    //allows to set each currencies variation indexes to the room properties
    private void SetTrendIndexes(int d, int e, int p, int y)
    {
        SetRoomProperty(DOLLARS_TREND_IND, d);
        SetRoomProperty(EUROS_TREND_IND, e);
        SetRoomProperty(POUNDS_TREND_IND, p);
        SetRoomProperty(YEN_TREND_IND, y);
    }

    //allows to set up the trends variations at each new turn
    private void PrepareTrends(bool rollVariation)
    {
        int randIndDol = Random.Range(0, 21);
        int randIndEur = Random.Range(0, 21);
        int randIndPou = Random.Range(0, 21);
        int randIndYen = Random.Range(0, 21);
        SetTrendIndexes(randIndDol, randIndEur, randIndPou, randIndYen);
        if (rollVariation)
        {

            float dolTrend = (float)myRoom.CustomProperties[DOLLARS_TREND];
            float eurTrend = (float)myRoom.CustomProperties[EUROS_TREND];
            float pouTrend = (float)myRoom.CustomProperties[POUNDS_TREND];
            float yenTrend = (float)myRoom.CustomProperties[YEN_TREND];
            if (!photonView.IsMine)
            {
                photonView.TransferOwnership(myPlayer);

            }
            photonView.RPC("TrendListUpdate", RpcTarget.AllBuffered, dolTrend, eurTrend, pouTrend, yenTrend);//we update current price with new variation
        }
        else
        {
            UpdateTrendList();
            SetTrends();
            //we need to update the prefs set in set trends for each player to have them
            if (!photonView.IsMine)
            {
                photonView.TransferOwnership(myPlayer);

            }
            photonView.RPC("SetTheHistoryTrends", RpcTarget.AllBuffered, 
                HISTORY_TURN_M3,
                HISTORY_TURN_M2,
                HISTORY_TURN_M1);//we update current price with new variation

        }


    }


    //allows to set the trends based on the newly created indexes in preparetrends()
    private void SetTrends()
    {
        //we have values from -.1 to +.1 = a variation in %
        //if -10% : price * .9f
        //if +10% : price * 1.1f

        float dolTrend = (float)myRoom.CustomProperties[DOLLARS_TREND];
        float eurTrend = (float)myRoom.CustomProperties[EUROS_TREND];
        float pouTrend = (float)myRoom.CustomProperties[POUNDS_TREND];
        float yenTrend = (float)myRoom.CustomProperties[YEN_TREND];


        SetRoomPrices((float)dolTrend, (float)eurTrend, (float)pouTrend, (float)yenTrend);
        UpdateHistory((float)dolTrend, (float)eurTrend, (float)pouTrend, (float)yenTrend,false);

    } 

    public void UpdatePricesOnHistory()
    {
        float dolPrice = (float)myRoom.CustomProperties[DOLLARS_PRICE];
        float eurPrice = (float)myRoom.CustomProperties[EUROS_PRICE];
        float pouPrice = (float)myRoom.CustomProperties[POUNDS_PRICE];
        float yenPrice = (float)myRoom.CustomProperties[YEN_PRICE];

        UpdateHistory(dolPrice,eurPrice, pouPrice, yenPrice,true);
        SetRoomProperty(HISTORY_UPDATE, 1);
        
    }
    private double CalculateNewPrice(float oldPrice, float trend)
    {
        trend = 1 + (trend / 10);
        double newPrice = oldPrice * trend;
        return Math.Round(newPrice, 2);

    }

    //allows to set the new trends based on the new indexes
    private void UpdateTrendList()
    {
        int randIndDol = (int)myRoom.CustomProperties[DOLLARS_TREND_IND];
        int randIndEur = (int)myRoom.CustomProperties[EUROS_TREND_IND];
        int randIndPou = (int)myRoom.CustomProperties[POUNDS_TREND_IND];
        int randIndYen = (int)myRoom.CustomProperties[YEN_TREND_IND];

        float dolTrend = TRENDS_VALUES[randIndDol];
        float eurTrend = TRENDS_VALUES[randIndEur];
        float pouTrend = TRENDS_VALUES[randIndPou];
        float yenTrend = TRENDS_VALUES[randIndYen];

        SetRoomTrends(dolTrend, eurTrend, pouTrend, yenTrend);


    }
    private void UpdateTrendListWithDice(float dollars, float euros, float pound, float yen)
    {

        int randIndDol = (int)myRoom.CustomProperties[DOLLARS_TREND_IND];
        int randIndEur = (int)myRoom.CustomProperties[EUROS_TREND_IND];
        int randIndPou = (int)myRoom.CustomProperties[POUNDS_TREND_IND];
        int randIndYen = (int)myRoom.CustomProperties[YEN_TREND_IND];

        float dolTrend = (float)TRENDS_VALUES[randIndDol];//the final trends
        float eurTrend = (float)TRENDS_VALUES[randIndEur];
        float pouTrend = (float)TRENDS_VALUES[randIndPou];
        float yenTrend = (float)TRENDS_VALUES[randIndYen];

        SetRoomTrends(dolTrend, eurTrend, pouTrend, yenTrend);

        SetRoomPrices(dolTrend, eurTrend, pouTrend, yenTrend);
        UpdateHistory(dolTrend, eurTrend, pouTrend, yenTrend, true);


    }

    //here the trends history gets updated  and so the history status is set to 1 to be updated in the GUI
    private void UpdateHistory(float dollars, float euros, float pound, float yen, bool inGame)
    {
        //depending on turn count, history gets updated
        int turnCount = GameManager.TURN_COUNT_VALUE;
        float dPercent = ((float)myRoom.CustomProperties[DOLLARS_TREND] - 1) * 100;
        float ePercent = ((float)myRoom.CustomProperties[EUROS_TREND] - 1) * 100;
        float pPercent = ((float)myRoom.CustomProperties[POUNDS_TREND] - 1) * 100;
        float yPercent = ((float)myRoom.CustomProperties[YEN_TREND] - 1) * 100;

        double dTrend = Math.Round(dPercent, 2);
        double eTrend = Math.Round(ePercent, 2);
        double pTrend = Math.Round(pPercent, 2);
        double yTrend = Math.Round(yPercent, 2);

        

        string newHistoryVal = $"D/{dollars.ToString()}/{dTrend.ToString()}_E/{euros.ToString()}/{eTrend.ToString()}_P/{pound.ToString()}/{pTrend.ToString()}_Y/{yen.ToString()}/{yTrend.ToString()}";
        if (!inGame)
        {
            switch (turnCount)
        {
            case 1:
                HISTORY_TURN_M3="D/1/0_E/1/0_P/1/0_Y/1/0";
                HISTORY_TURN_M2= "D/1/0_E/1/0_P/1/0_Y/1/0";
                HISTORY_TURN_M1= "D/1/0_E/1/0_P/1/0_Y/1/0";
                myRoom.CustomProperties[HISTORY_TURN_ACTUAL] = newHistoryVal;
                break;
            case 2:
                HISTORY_TURN_M1= (string)myRoom.CustomProperties[HISTORY_TURN_ACTUAL];
                myRoom.CustomProperties[HISTORY_TURN_ACTUAL] = newHistoryVal;
                break;
            case 3:
                HISTORY_TURN_M2= HISTORY_TURN_M1;
                HISTORY_TURN_M1= (string)myRoom.CustomProperties[HISTORY_TURN_ACTUAL];
                myRoom.CustomProperties[HISTORY_TURN_ACTUAL] = newHistoryVal;
                break;
            default:
                HISTORY_TURN_M3= HISTORY_TURN_M2;
                HISTORY_TURN_M2= HISTORY_TURN_M1;
                HISTORY_TURN_M1= (string)myRoom.CustomProperties[HISTORY_TURN_ACTUAL];
                myRoom.CustomProperties[HISTORY_TURN_ACTUAL] = newHistoryVal;
                break;

        }
             SetRoomHistoryStatus(1);
        }
        else
        {
            myRoom.CustomProperties[HISTORY_TURN_ACTUAL] = newHistoryVal;
        }

        //we set the history status as done.


    }
    
    private void UpdateHistoryNewTurn(float dollars, float euros, float pound, float yen)
    {
        //depending on turn count, history gets updated
        int turnCount = GameManager.TURN_COUNT_VALUE;
        float dPercent = ((float)myRoom.CustomProperties[DOLLARS_TREND] - 1) * 100;
        float ePercent = ((float)myRoom.CustomProperties[EUROS_TREND] - 1) * 100;
        float pPercent = ((float)myRoom.CustomProperties[POUNDS_TREND] - 1) * 100;
        float yPercent = ((float)myRoom.CustomProperties[YEN_TREND] - 1) * 100;

        double dTrend = Math.Round(dPercent, 2);
        double eTrend = Math.Round(ePercent, 2);
        double pTrend = Math.Round(pPercent, 2);
        double yTrend = Math.Round(yPercent, 2);


        string newHistoryVal = $"D/{dollars.ToString()}/{dTrend.ToString()}_E/{euros.ToString()}/{eTrend.ToString()}_P/{pound.ToString()}/{pTrend.ToString()}_Y/{yen.ToString()}/{yTrend.ToString()}";

            switch (turnCount)
        {
            case 1:
                HISTORY_TURN_M3="D/1/0_E/1/0_P/1/0_Y/1/0";
                HISTORY_TURN_M2= "D/1/0_E/1/0_P/1/0_Y/1/0";
                HISTORY_TURN_M1= "D/1/0_E/1/0_P/1/0_Y/1/0";
                myRoom.CustomProperties[HISTORY_TURN_ACTUAL] = newHistoryVal;
                break;
            case 2:
                HISTORY_TURN_M1= (string)myRoom.CustomProperties[HISTORY_TURN_ACTUAL];
                myRoom.CustomProperties[HISTORY_TURN_ACTUAL] = newHistoryVal;
                break;
            case 3:
                HISTORY_TURN_M2= HISTORY_TURN_M1;
                HISTORY_TURN_M1= (string)myRoom.CustomProperties[HISTORY_TURN_ACTUAL];
                myRoom.CustomProperties[HISTORY_TURN_ACTUAL] = newHistoryVal;
                break;
            default:
                HISTORY_TURN_M3= HISTORY_TURN_M2;
                HISTORY_TURN_M2= HISTORY_TURN_M1;
                HISTORY_TURN_M1= (string)myRoom.CustomProperties[HISTORY_TURN_ACTUAL];
                myRoom.CustomProperties[HISTORY_TURN_ACTUAL] = newHistoryVal;
                break;

        }
        UpdateHistoryGUI();
          
        //we set the history status as done.


    }

    //the fortune is updated based on the current amount of money owned by the player and the conversion to gold of each amount.
    //the player prefs and the room amount for this player are also updated and the text UI as well.

    private void UpdateFortune()
    {
        float euros = (float)myRoom.CustomProperties[EUROS_PRICE] * PLAYER_EUROS;//we get the value of x euros in gold
        float dollars = (float)myRoom.CustomProperties[DOLLARS_PRICE] * PLAYER_DOLLARS;//we get the value of x euros in gold
        float pounds = (float)myRoom.CustomProperties[POUNDS_PRICE] * PLAYER_POUNDS;//we get the value of x euros in gold
        float yens = (float)myRoom.CustomProperties[YEN_PRICE] * PLAYER_YENS;//we get the value of x euros in gold
        float gold = PLAYER_GOLD;
       



        double totalFortune = Math.Round(euros + dollars + pounds + yens + gold, MidpointRounding.AwayFromZero);
        PLAYER_FORTUNE= (float)totalFortune;
        float total = PLAYER_FORTUNE + PLAYER_SAVINGS;
        totalFortuneText.text = total.ToString();

        PLAYER_EUROS= (float)Math.Round(euros, MidpointRounding.AwayFromZero);
        PLAYER_DOLLARS= (float)Math.Round(dollars, MidpointRounding.AwayFromZero);
        PLAYER_POUNDS = (float)Math.Round(pounds, MidpointRounding.AwayFromZero);
        PLAYER_YENS= (float)Math.Round(yens, MidpointRounding.AwayFromZero);

        UpdateAmountText();
        UpdateMaxCurrency();

        if (!updateOnceFortune)
        {
            FriendsManager.changeFortune = true;
        }
        CheckFortune((float)totalFortune);


    }
    public void UpdateFortuneInGame()
    {
        
        float euros = PLAYER_EUROS;//we get the value of x euros in gold
        float dollars = PLAYER_DOLLARS;//we get the value of x euros in gold
        float pounds =PLAYER_POUNDS;//we get the value of x euros in gold
        float yens =PLAYER_YENS;//we get the value of x euros in gold
        float gold =PLAYER_GOLD;

        double totalFortune = Math.Round(euros + dollars + pounds + yens + gold, MidpointRounding.AwayFromZero);
      
        PLAYER_FORTUNE = (float)totalFortune;
        float total = PLAYER_FORTUNE + PLAYER_SAVINGS;
        totalFortuneText.text = total.ToString();
        //SetRoomAmounts(gold, dollars, euros, pounds, yens);//each player modifies its room amount.
        UpdateAmountText();
        UpdateMaxCurrency();
        FriendsManager.changeFortune = true;
        CheckFortune((float)totalFortune);
    }
    //simple wait function allowing to synchronise every room properties when needed
    private void Wait(float limit)
    {
        float time = 0;

        while (time < limit)
        {
            time += Time.deltaTime;
        }
    }

    //allows to update the history gui for each players
    private void UpdateHistoryGUI()
    {

        string M3 = HISTORY_TURN_M3;
        string M2 = HISTORY_TURN_M2;
        string M1 = HISTORY_TURN_M1;

        string Actual = (string)myRoom.CustomProperties[HISTORY_TURN_ACTUAL];

        string[] m3 = M3.Split('_');
        string[] m2 = M2.Split('_');
        string[] m1 = M1.Split('_');
        string[] actual = Actual.Split('_');

        string[] dM3 = m3[0].Split('/');
        string[] eM3 = m3[1].Split('/');
        string[] pM3 = m3[2].Split('/');
        string[] yM3 = m3[3].Split('/');

        dHistory[3].text = $"{dM3[1]}G  \n {dM3[2]}%";
        eHistory[3].text = $"{eM3[1]}G  \n {eM3[2]}%";
        pHistory[3].text = $"{pM3[1]}G  \n {pM3[2]}%";
        yHistory[3].text = $"{yM3[1]}G  \n {yM3[2]}%";

        string[] dM2 = m2[0].Split('/');
        string[] eM2 = m2[1].Split('/');
        string[] pM2 = m2[2].Split('/');
        string[] yM2 = m2[3].Split('/');

        dHistory[2].text = $"{dM2[1]}G  \n {dM2[2]}%";
        eHistory[2].text = $"{eM2[1]}G  \n {eM2[2]}%";
        pHistory[2].text = $"{pM2[1]}G  \n {pM2[2]}%";
        yHistory[2].text = $"{yM2[1]}G  \n {yM2[2]}%";

        string[] dM1 = m1[0].Split('/');
        string[] eM1 = m1[1].Split('/');
        string[] pM1 = m1[2].Split('/');
        string[] yM1 = m1[3].Split('/');

        dHistory[1].text = $"{dM1[1]}G  \n {dM1[2]}%";
        eHistory[1].text = $"{eM1[1]}G  \n {eM1[2]}%";
        pHistory[1].text = $"{pM1[1]}G  \n {pM1[2]}%";
        yHistory[1].text = $"{yM1[1]}G  \n {yM1[2]}%";

        string[] dM0 = actual[0].Split('/');
        string[] eM0 = actual[1].Split('/');
        string[] pM0 = actual[2].Split('/');
        string[] yM0 = actual[3].Split('/');

        dHistory[0].text = $"{dM0[1]}G  \n {dM0[2]}%";
        eHistory[0].text = $"{eM0[1]}G  \n {eM0[2]}%";
        pHistory[0].text = $"{pM0[1]}G  \n {pM0[2]}%";
        yHistory[0].text = $"{yM0[1]}G  \n {yM0[2]}%";




    }

    //allows to set a new or existing room property with an int.
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

    //allows to set a new or existing room property with an float.
    private void SetRoomProperty(string hashKey, float value)
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

    //allows to set a new or existing player property with an int.
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

    //allows to set a new or existing room property with an int for each players by the master player.
    private void SetCustomsPPT(string hashKeyIndex, int ind, Player p)
    {
        int playerIndex = ind;
        if (p.CustomProperties[hashKeyIndex] != null)
        {
            p.CustomProperties[hashKeyIndex] = playerIndex;
        }
        else
        {
            p.CustomProperties.Add(hashKeyIndex, playerIndex);
        }

        p.SetCustomProperties(p.CustomProperties);
    }


    private void CheckFortune(float fortune)
    {
        PLAYER_FORTUNE= fortune;
        GameModeManager.checkFortune = true;

    }
    //allows to update the amount of money owned by the player displayed
    public void UpdateAmountText()
    {
        
        goldAmount.text = PLAYER_GOLD.ToString();
        dollarsAmount.text = PLAYER_DOLLARS.ToString();
        eurosAmount.text = PLAYER_EUROS.ToString();
        poundsAmount.text = PLAYER_POUNDS.ToString();
        yenAmount.text = PLAYER_YENS.ToString();

        UpdateTextColor();
    }
    private void UpdateTextColor()
    {
        if (PLAYER_GOLD <= 0)//we put the text color to red
        {
            goldAmount.color = new Color32(255, 0, 0, 255);
        }
        else
        {
            goldAmount.color = new Color32(255, 255, 255, 255);
        }

        if (PLAYER_DOLLARS <= 0)//we put the text color to red
        {
            dollarsAmount.color = new Color32(255, 0, 0, 255);
        }
        else
        {
            dollarsAmount.color = new Color32(255, 255, 255, 255);
        }

        if (PLAYER_EUROS <= 0)//we put the text color to red
        {
            eurosAmount.color = new Color32(255, 0, 0, 255);
        }
        else
        {
            eurosAmount.color = new Color32(255, 255, 255, 255);
        }

        if (PLAYER_POUNDS <= 0)//we put the text color to red
        {
            poundsAmount.color = new Color32(255, 0, 0, 255);
        }
        else
        {
            poundsAmount.color = new Color32(255, 255, 255, 255);
        }

        if (PLAYER_YENS <= 0)//we put the text color to red
        {
            yenAmount.color = new Color32(255, 0, 0, 255);
        }
        else
        {
            yenAmount.color = new Color32(255, 255, 255, 255);
        }
    }
    public void UpdateMaxCurrency()
    {
        var l = new List<float>() { PLAYER_DOLLARS, PLAYER_EUROS, PLAYER_POUNDS,PLAYER_YENS };
        var max = l.Max();
        if(max == PLAYER_DOLLARS)
        {
            PLAYER_HIGHEST_CURRENCY_NAME = "d";
            PLAYER_HIGHEST_CURRENCY_VALUE = PLAYER_DOLLARS;
        }
        else if (max == PLAYER_EUROS)
        {
            PLAYER_HIGHEST_CURRENCY_NAME = "e";
            PLAYER_HIGHEST_CURRENCY_VALUE = PLAYER_EUROS;
        }
        else if (max == PLAYER_POUNDS)
        {
            PLAYER_HIGHEST_CURRENCY_NAME = "p";
            PLAYER_HIGHEST_CURRENCY_VALUE = PLAYER_POUNDS;
        }
        else if (max == PLAYER_YENS)
        {
            PLAYER_HIGHEST_CURRENCY_NAME = "y";
            PLAYER_HIGHEST_CURRENCY_VALUE = PLAYER_YENS;
        }

    }
    [PunRPC]
    private void TrendsUpdates(bool roll)//roll determines if the trend updates comes from when all players rolled the dices
    {
        if(myPlayer.IsMasterClient)
        {
            PrepareTrends(roll);
        }
        
    }    
    [PunRPC]
    private void TrendListUpdate(float dolTrend, float eurTrend, float pouTrend,float yenTrend)
    {
        UpdateTrendListWithDice(dolTrend,eurTrend,pouTrend,yenTrend);
        UpdateTrendDisplay();
        UpdateHistoryGUI();
        
    }

    public void NewTurnFunction()
    {
        float dolPrice = (float)myRoom.CustomProperties[DOLLARS_PRICE];
        float eurPrice = (float)myRoom.CustomProperties[EUROS_PRICE];
        float pouPrice = (float)myRoom.CustomProperties[POUNDS_PRICE];
        float yenPrice = (float)myRoom.CustomProperties[YEN_PRICE];
        UpdateHistoryNewTurn(dolPrice, eurPrice, pouPrice, yenPrice);
    }
    [PunRPC]
    private void SetTheHistoryTrends(string m3, string m2, string m1)
    {

        if (!myPlayer.IsMasterClient)
        {
            HISTORY_TURN_M3 = m3;
            HISTORY_TURN_M2 = m2;
            HISTORY_TURN_M1 = m1;

            Debug.Log("Changing m3,m2,m1 to :" + HISTORY_TURN_M3 + "" + HISTORY_TURN_M2 + " " + HISTORY_TURN_M1);
        }
    }
    public void PlayerDQMechanic(float g, float d, float e, float p, float y)
    {
        if(!photonView.IsMine)
        {
            photonView.TransferOwnership(myPlayer);
        }
        Debug.Log($"giving {g}, {d}, {e}, {p}, {y} to others");
        photonView.RPC("GiveToOthers", RpcTarget.AllBuffered, myPlayer.NickName,g, d, e, p, y);
    }
    [PunRPC]
    private void GiveToOthers(string giverName,float g, float d, float e,float p, float y)
    {
        if (!myPlayer.NickName.Equals(giverName))
        {
            PLAYER_GOLD += g;
            PLAYER_DOLLARS += d;
            PLAYER_EUROS += e;
            PLAYER_POUNDS += p;
            PLAYER_YENS += y;

            UpdateFortuneInGame();
        }
        else
        {
            Exit.exitAfterDQ = true;
        }

       
    }
}
