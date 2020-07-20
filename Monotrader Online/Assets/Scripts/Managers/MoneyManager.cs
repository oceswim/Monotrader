using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class MoneyManager : MonoBehaviourPun
{

    private const float INITIAL_GOLD = 5000;
    private const float INITIAL_CURRENCIES = 2500;
    private const string FORTUNE = "myFortune";
    private const string TURN_COUNT = "TurnCount";
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



    private const string HISTORY_TURN_ACTUAL = "History_Turn_Actual";
    private const string HISTORY_TURN_M1 = "History_Turn_Minus1";
    private const string HISTORY_TURN_M2 = "History_Turn_Minus2";
    private const string HISTORY_TURN_M3 = "History_Turn_Minus3";

    private const string EUROS_TREND = "Euros_Trend";
    private const string DOLLARS_TREND = "Dollars_Trend";
    private const string POUNDS_TREND = "Pounds_Trend";
    private const string YEN_TREND = "Yen_Trend";

    private const string EUROS_TREND_IND = "Euros_Trend_Ind";
    private const string DOLLARS_TREND_IND = "Dollars_Trend_Ind";
    private const string POUNDS_TREND_IND = "Pounds_Trend_Ind";
    private const string YEN_TREND_IND = "Yen_Trend_Ind";

    private int[] TRENDS_VALUES;

    private const string GAME_STATE_HASHKEY = "gameState";

    private string PLAYER_GOLD;
    private string PLAYER_DOLLARS;
    private string PLAYER_EUROS;
    private string PLAYER_YENS;
    private string PLAYER_POUNDS;
    private string PLAYER_FORTUNE;

    private float myGold, myDollars, myEuros, myYens, myPounds;

    private Player myPlayer;
    private Room myRoom;
    private string actorNumber;
    private bool playersReady,updateOnceFortune, updateOnceGUI,setUpOnceTrend,changeTurn;

    private List<Player> notReadyPlayers = new List<Player>();

    public TMP_Text goldAmount, dollarsAmount, eurosAmount, poundsAmount, yenAmount, waitingForText, totalFortuneText;
    public GameObject waitingForObject,sectionsManagerObject;
    public TMP_Text[] dollarsTrendInGame, eurosTrendInGame, poundsTrendInGame, yensTrendInGame, dHistory, eHistory, pHistory, yHistory;
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
        actorNumber = myPlayer.ActorNumber.ToString();
        playersReady =setUpOnceTrend=changeTurn= false;
        myRoom = GameManager.myRoom;
        InitialiseHashKeys();
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
                        //set setctions object to true
                        sectionsManagerObject.SetActive(true);
                        FriendsManager.initialFortune = true;


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
    }

    //the trends list for each currency, from -10 to +10 %
    private void PopulateTrendsList()
    {
        TRENDS_VALUES = new int[21];
        for (int i = -10; i < 11; i++)
        {
            int index = i + 10;
            TRENDS_VALUES[index] = i;
        }
    }


    //the different hashkeys for each players
    private void InitialiseHashKeys()
    {
        PLAYER_FORTUNE = "Player" + actorNumber + "Fortune";
        PLAYER_GOLD = "Player" + actorNumber + "Gold";
        PlayerPrefs.SetString("MYGOLD", PLAYER_GOLD);
        PLAYER_DOLLARS = "Player" + actorNumber + "Dollars";
        PlayerPrefs.SetString("MYDOLLARS", PLAYER_DOLLARS);
        PLAYER_EUROS = "Player" + actorNumber + "Euros";
        PlayerPrefs.SetString("MYEUROS", PLAYER_EUROS);
        PLAYER_YENS = "Player" + actorNumber + "Yens";
        PlayerPrefs.SetString("MYYENS", PLAYER_YENS);
        PLAYER_POUNDS = "Player" + actorNumber + "Pounds";
        PlayerPrefs.SetString("MYPOUNDS", PLAYER_POUNDS);
    }

    private void InitialiseHistoryPrefs()
    {
        PlayerPrefs.SetString(HISTORY_TURN_M3, "D/1/0_E/1/0_P/1/0_Y/1/0");
        PlayerPrefs.SetString(HISTORY_TURN_M2, "D/1/0_E/1/0_P/1/0_Y/1/0");
        PlayerPrefs.SetString(HISTORY_TURN_M1, "D/1/0_E/1/0_P/1/0_Y/1/0");
    }
    //the initial prices of each currency at the beginning of the game
    private void SetInitialAmounts(bool masterClient)
    {
        myGold = INITIAL_GOLD;
        myEuros = myDollars = myPounds = myYens = INITIAL_CURRENCIES;
        PlayerPrefs.SetFloat(PLAYER_GOLD, myGold);
        PlayerPrefs.SetFloat(PLAYER_DOLLARS, myDollars);
        PlayerPrefs.SetFloat(PLAYER_EUROS, myEuros);
        PlayerPrefs.SetFloat(PLAYER_POUNDS, myPounds);
        PlayerPrefs.SetFloat(PLAYER_YENS, myYens);
        Debug.Log(PlayerPrefs.GetFloat(PLAYER_GOLD) + " " + myPlayer.NickName);
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

    //allows to set each currencies amount each player owns to the room properties
    private void SetRoomAmounts(float g, float d, float e, float p, float y)
    {

        SetRoomProperty(PLAYER_GOLD, g);
        SetRoomProperty(PLAYER_EUROS, e);
        SetRoomProperty(PLAYER_DOLLARS, d);
        SetRoomProperty(PLAYER_POUNDS, p);
        SetRoomProperty(PLAYER_YENS, y);



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
                PlayerPrefs.GetString(HISTORY_TURN_M3),
                PlayerPrefs.GetString(HISTORY_TURN_M2),
                PlayerPrefs.GetString(HISTORY_TURN_M1));//we update current price with new variation

        }


    }


    //allows to set the trends based on the newly created indexes in preparetrends()
    private void SetTrends()
    {
        //get the increase or decrease value
        float dolPrice = (float)myRoom.CustomProperties[DOLLARS_PRICE];
        float eurPrice = (float)myRoom.CustomProperties[EUROS_PRICE];
        float pouPrice = (float)myRoom.CustomProperties[POUNDS_PRICE];
        float yenPrice = (float)myRoom.CustomProperties[YEN_PRICE];


        //we have values from -.1 to +.1 = a variation in %
        //if -10% : price * .9f
        //if +10% : price * 1.1f

        float dolTrend = (float)myRoom.CustomProperties[DOLLARS_TREND];
        float eurTrend = (float)myRoom.CustomProperties[EUROS_TREND];
        float pouTrend = (float)myRoom.CustomProperties[POUNDS_TREND];
        float yenTrend = (float)myRoom.CustomProperties[YEN_TREND];

        double newDolPrice = CalculateNewPrice(dolPrice, dolTrend);
        double newEurPrice = CalculateNewPrice(eurPrice, eurTrend);
        double newPouPrice = CalculateNewPrice(pouPrice, pouTrend);
        double newYenPrice = CalculateNewPrice(yenPrice, yenTrend);


        SetRoomPrices((float)newDolPrice, (float)newEurPrice, (float)newPouPrice, (float)newYenPrice);
        UpdateHistory((float)newDolPrice, (float)newEurPrice, (float)newPouPrice, (float)newYenPrice,false);

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

        float dolTrend = (float)TRENDS_VALUES[randIndDol] / 10;
        float eurTrend = (float)TRENDS_VALUES[randIndEur] / 10;
        float pouTrend = (float)TRENDS_VALUES[randIndPou] / 10;
        float yenTrend = (float)TRENDS_VALUES[randIndYen] / 10;

        SetRoomTrends(dolTrend, eurTrend, pouTrend, yenTrend);


    }
    private void UpdateTrendListWithDice(float dollars, float euros, float pound, float yen)
    {
        float dolPrice = (float)myRoom.CustomProperties[DOLLARS_PRICE];
        float eurPrice = (float)myRoom.CustomProperties[EUROS_PRICE];
        float pouPrice = (float)myRoom.CustomProperties[POUNDS_PRICE];
        float yenPrice = (float)myRoom.CustomProperties[YEN_PRICE];

        int randIndDol = (int)myRoom.CustomProperties[DOLLARS_TREND_IND];
        int randIndEur = (int)myRoom.CustomProperties[EUROS_TREND_IND];
        int randIndPou = (int)myRoom.CustomProperties[POUNDS_TREND_IND];
        int randIndYen = (int)myRoom.CustomProperties[YEN_TREND_IND];

        float dolTrend = (float)TRENDS_VALUES[randIndDol] / 10 + dollars;//the final trends
        float eurTrend = (float)TRENDS_VALUES[randIndEur] / 10 + euros;
        float pouTrend = (float)TRENDS_VALUES[randIndPou] / 10 + pound;
        float yenTrend = (float)TRENDS_VALUES[randIndYen] / 10 + yen;

        float dolNew = (float)TRENDS_VALUES[randIndDol];//the new trends just initiated
        float eurNew = (float)TRENDS_VALUES[randIndEur];
        float pouNew = (float)TRENDS_VALUES[randIndPou];
        float yenNew = (float)TRENDS_VALUES[randIndYen];


        SetRoomTrends(dolTrend, eurTrend, pouTrend, yenTrend);

        double newDolPrice = CalculateNewPrice(dolPrice, dolNew/10);
        double newEurPrice = CalculateNewPrice(eurPrice, eurNew/10);
        double newPouPrice = CalculateNewPrice(pouPrice, pouNew/10);
        double newYenPrice = CalculateNewPrice(yenPrice, yenNew/10);


        SetRoomPrices((float)newDolPrice, (float)newEurPrice, (float)newPouPrice, (float)newYenPrice);
        UpdateHistory((float)newDolPrice, (float)newEurPrice, (float)newPouPrice, (float)newYenPrice, true);


    }

    //here the trends history gets updated  and so the history status is set to 1 to be updated in the GUI
    private void UpdateHistory(float dollars, float euros, float pound, float yen, bool inGame)
    {
        //depending on turn count, history gets updated
        int turnCount = PlayerPrefs.GetInt(TURN_COUNT);
        double dTrend = Math.Round((float)myRoom.CustomProperties[DOLLARS_TREND] * 10, 2);
        double eTrend = Math.Round((float)myRoom.CustomProperties[EUROS_TREND] * 10, 2);
        double pTrend = Math.Round((float)myRoom.CustomProperties[POUNDS_TREND] * 10, 2);
        double yTrend = Math.Round((float)myRoom.CustomProperties[YEN_TREND] * 10, 2);

        string newHistoryVal = $"D/{dollars.ToString()}/{dTrend.ToString()}_E/{euros.ToString()}/{eTrend.ToString()}_P/{pound.ToString()}/{pTrend.ToString()}_Y/{yen.ToString()}/{yTrend.ToString()}";
        if (!inGame)
        {
            switch (turnCount)
        {
            case 1:
                PlayerPrefs.SetString(HISTORY_TURN_M3,"D/1/0_E/1/0_P/1/0_Y/1/0");
                PlayerPrefs.SetString(HISTORY_TURN_M2, "D/1/0_E/1/0_P/1/0_Y/1/0");
                PlayerPrefs.SetString(HISTORY_TURN_M1, "D/1/0_E/1/0_P/1/0_Y/1/0");
                myRoom.CustomProperties[HISTORY_TURN_ACTUAL] = newHistoryVal;
                break;
            case 2:
                PlayerPrefs.SetString(HISTORY_TURN_M1, (string)myRoom.CustomProperties[HISTORY_TURN_ACTUAL]);
                myRoom.CustomProperties[HISTORY_TURN_ACTUAL] = newHistoryVal;
                break;
            case 3:
                PlayerPrefs.SetString(HISTORY_TURN_M2, PlayerPrefs.GetString(HISTORY_TURN_M1));
                PlayerPrefs.SetString(HISTORY_TURN_M1, (string)myRoom.CustomProperties[HISTORY_TURN_ACTUAL]);
               
                myRoom.CustomProperties[HISTORY_TURN_ACTUAL] = newHistoryVal;
                break;
            default:
                PlayerPrefs.SetString(HISTORY_TURN_M3, PlayerPrefs.GetString(HISTORY_TURN_M2));
                PlayerPrefs.SetString(HISTORY_TURN_M2, PlayerPrefs.GetString(HISTORY_TURN_M1));
                PlayerPrefs.SetString(HISTORY_TURN_M1, (string)myRoom.CustomProperties[HISTORY_TURN_ACTUAL]);

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
        int turnCount = PlayerPrefs.GetInt(TURN_COUNT);
        double dTrend = Math.Round((float)myRoom.CustomProperties[DOLLARS_TREND] * 10, 2);
        double eTrend = Math.Round((float)myRoom.CustomProperties[EUROS_TREND] * 10, 2);
        double pTrend = Math.Round((float)myRoom.CustomProperties[POUNDS_TREND] * 10, 2);
        double yTrend = Math.Round((float)myRoom.CustomProperties[YEN_TREND] * 10, 2);

        string newHistoryVal = $"D/{dollars.ToString()}/{dTrend.ToString()}_E/{euros.ToString()}/{eTrend.ToString()}_P/{pound.ToString()}/{pTrend.ToString()}_Y/{yen.ToString()}/{yTrend.ToString()}";
       
            switch (turnCount)
        {
            case 1:
                PlayerPrefs.SetString(HISTORY_TURN_M3,"D/1/0_E/1/0_P/1/0_Y/1/0");
                PlayerPrefs.SetString(HISTORY_TURN_M2, "D/1/0_E/1/0_P/1/0_Y/1/0");
                PlayerPrefs.SetString(HISTORY_TURN_M1, "D/1/0_E/1/0_P/1/0_Y/1/0");
                myRoom.CustomProperties[HISTORY_TURN_ACTUAL] = newHistoryVal;
                break;
            case 2:
                PlayerPrefs.SetString(HISTORY_TURN_M1, (string)myRoom.CustomProperties[HISTORY_TURN_ACTUAL]);
                myRoom.CustomProperties[HISTORY_TURN_ACTUAL] = newHistoryVal;
                break;
            case 3:
                PlayerPrefs.SetString(HISTORY_TURN_M2, PlayerPrefs.GetString(HISTORY_TURN_M1));
                PlayerPrefs.SetString(HISTORY_TURN_M1, (string)myRoom.CustomProperties[HISTORY_TURN_ACTUAL]);
               
                myRoom.CustomProperties[HISTORY_TURN_ACTUAL] = newHistoryVal;
                break;
            default:
                PlayerPrefs.SetString(HISTORY_TURN_M3, PlayerPrefs.GetString(HISTORY_TURN_M2));
                PlayerPrefs.SetString(HISTORY_TURN_M2, PlayerPrefs.GetString(HISTORY_TURN_M1));
                PlayerPrefs.SetString(HISTORY_TURN_M1, (string)myRoom.CustomProperties[HISTORY_TURN_ACTUAL]);

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

      
        float euros = (float)myRoom.CustomProperties[EUROS_PRICE] * PlayerPrefs.GetFloat(PLAYER_EUROS);//we get the value of x euros in gold
        
        float dollars = (float)myRoom.CustomProperties[DOLLARS_PRICE] * PlayerPrefs.GetFloat(PLAYER_DOLLARS);//we get the value of x euros in gold
        float pounds = (float)myRoom.CustomProperties[POUNDS_PRICE] * PlayerPrefs.GetFloat(PLAYER_POUNDS);//we get the value of x euros in gold
        float yens = (float)myRoom.CustomProperties[YEN_PRICE] * PlayerPrefs.GetFloat(PLAYER_YENS);//we get the value of x euros in gold
        float gold = PlayerPrefs.GetFloat(PLAYER_GOLD);


       
        double totalFortune = Math.Round(euros + dollars + pounds + yens + gold, 2);
        PlayerPrefs.SetFloat(FORTUNE, (float)totalFortune);
        totalFortuneText.text = totalFortune.ToString();

        SetRoomAmounts(gold, dollars, euros, pounds, yens);//each player modifies its room amount.
        PlayerPrefs.SetFloat(PLAYER_EUROS, euros);
        PlayerPrefs.SetFloat(PLAYER_DOLLARS, dollars);
        PlayerPrefs.SetFloat(PLAYER_POUNDS, pounds);
        PlayerPrefs.SetFloat(PLAYER_YENS, yens);

        UpdateAmountText();
        if (!updateOnceFortune)
        {
            FriendsManager.changeFortune = true;
        }
        CheckFortune((float)totalFortune);


    }
    public void UpdateFortuneInGame()
    {
        float euros = PlayerPrefs.GetFloat(PLAYER_EUROS);//we get the value of x euros in gold
        float dollars = PlayerPrefs.GetFloat(PLAYER_DOLLARS);//we get the value of x euros in gold
        float pounds =PlayerPrefs.GetFloat(PLAYER_POUNDS);//we get the value of x euros in gold
        float yens = PlayerPrefs.GetFloat(PLAYER_YENS);//we get the value of x euros in gold
        float gold = PlayerPrefs.GetFloat(PLAYER_GOLD);

        Debug.Log($"new euros:{euros} new dollars:{dollars} new pounds:{pounds} new yens:{yens} new gold:{gold}");
    
        double totalFortune = Math.Round(euros + dollars + pounds + yens + gold, 2);
        PlayerPrefs.SetFloat(FORTUNE, (float)totalFortune);
        totalFortuneText.text = totalFortune.ToString();

        SetRoomAmounts(gold, dollars, euros, pounds, yens);//each player modifies its room amount.
        UpdateAmountText();
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

        string M3 = PlayerPrefs.GetString(HISTORY_TURN_M3);
        string M2 = PlayerPrefs.GetString(HISTORY_TURN_M2);
        string M1 = PlayerPrefs.GetString(HISTORY_TURN_M1);
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
        //Debug.Log($"Setting {myPlayer.NickName} {hashKeyIndex} to {ind} : {PhotonNetwork.LocalPlayer.CustomProperties[hashKeyIndex]}");
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

    //allows to update the amount of money owned by the player displayed
    private void CheckFortune(float fortune)
    {
        SetRoomProperty(PLAYER_FORTUNE, fortune);
        GameModeManager.checkFortune = true;

    }

    private void UpdateAmountText()
    {
        goldAmount.text = PlayerPrefs.GetFloat(PLAYER_GOLD).ToString();
        dollarsAmount.text = PlayerPrefs.GetFloat(PLAYER_DOLLARS).ToString();
        eurosAmount.text = PlayerPrefs.GetFloat(PLAYER_EUROS).ToString();
        poundsAmount.text = PlayerPrefs.GetFloat(PLAYER_POUNDS).ToString();
        yenAmount.text = PlayerPrefs.GetFloat(PLAYER_YENS).ToString();
    }

    [PunRPC]
    private void TrendsUpdates(bool roll)
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
            PlayerPrefs.SetString(HISTORY_TURN_M3, m3);
            PlayerPrefs.SetString(HISTORY_TURN_M2, m2);
            PlayerPrefs.SetString(HISTORY_TURN_M1, m1);
        }
    }
}
