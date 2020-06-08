using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class MoneyManager : MonoBehaviour
{
    
    private const float INITIAL_GOLD = 5000;
    private const float INITIAL_CURRENCIES = 2500;
    private const string TURN_COUNT = "TurnCount";
    private const string EUROS_PRICE  = "Euros_Price";
    private const string DOLLARS_PRICE  = "Dollars_Price";
    private const string POUNDS_PRICE  = "Pounds_Price";
    private const string YEN_PRICE  = "Yen_Price";
    private const string TRENDS_STATUS  = "Trend_Status";
    private const string HISTORY_STATUS  = "History_Status";
    
    private const string HISTORY_TURN_ACTUAL  = "History_Turn_Actual";
    private const string HISTORY_TURN_M1 = "History_Turn_Minus1";
    private const string HISTORY_TURN_M2 = "History_Turn_Minus2";
    private const string HISTORY_TURN_M3 = "History_Turn_Minus3";
    
    private const string EUROS_TREND  = "Euros_Trend";
    private const string DOLLARS_TREND  = "Dollars_Trend";
    private const string POUNDS_TREND  = "Pounds_Trend";
    private const string YEN_TREND  = "Yen_Trend";


    private int[] TRENDS_VALUES;
    private const string PLAYER_STATE = "myState";
    private const string PLAYER_READY_HASHKEY = "playerReady";

    private string PLAYER_GOLD;
    private string PLAYER_DOLLARS;
    private string PLAYER_EUROS;
    private string PLAYER_YENS;
    private string PLAYER_POUNDS;
    private float myGold, myDollars, myEuros, myYens, myPounds;
    private Player myPlayer;
    private Room myRoom;
    private int playerCount;
    private string actorNumber;
    private bool playersReady,updateGUI;

    private List<Player> notReadyPlayers = new List<Player>();

    public TMP_Text goldAmount,dollarsAmount, eurosAmount, poundsAmount, yenAmount,waitingForText;
    public GameObject waitingForObject;
    public TMP_Text[] dollarsTrendInGame, eurosTrendInGame, poundsTrendInGame, yensTrendInGame,dHistory,eHistory,pHistory,yHistory;
    public static bool newTurn;
    // Start is called before the first frame update
    void Start()
    {

        PopulateTrendsList();
        myPlayer = PhotonNetwork.LocalPlayer;
        actorNumber = myPlayer.ActorNumber.ToString();
        playersReady =newTurn=updateGUI= false;
        playerCount = PhotonNetwork.PlayerList.Length;
        myRoom = PhotonNetwork.CurrentRoom;
        InitialiseHashKeys();
    }

    // Update is called once per frame
    void Update()
    {
        if (myRoom.CustomProperties[PLAYER_READY_HASHKEY] != null)
        {
            if ((int)myRoom.CustomProperties[PLAYER_READY_HASHKEY] == playerCount && !playersReady)
            {
                playersReady = true;
                waitingForObject.SetActive(false);
                SetInitialAmounts(myPlayer.IsMasterClient);
                if (myPlayer.IsMasterClient)
                {
                    PrepareTrends();
                }
                Debug.Log("everyone is ready!");
            }
            else if(!playersReady)
            {
                string waitingText = "Waiting for ";
                foreach (Player p in PhotonNetwork.PlayerListOthers)
                {
                    if (p.CustomProperties[PLAYER_STATE] == null)
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
                                  
                if(!waitingForText.text.Equals(waitingText))
                {
                    waitingForText.text = waitingText;
                }
                else
                {
                    Debug.Log("Same text so not updated");
                    Debug.Log(waitingText + "vs " + waitingForText.text);
                }
            }
            else if(myRoom.CustomProperties[TRENDS_STATUS]!=null)
            {
                if ((int)myRoom.CustomProperties[TRENDS_STATUS] == 1)
                {
                    if (myPlayer.IsMasterClient)
                    {
                        SetTrends();
                        updateGUI = true;
                    }
                }
                else if ((int)myRoom.CustomProperties[TRENDS_STATUS] == 0 && updateGUI)
                {
                    //update history look
                    updateGUI = false;
                    int turn = (int)myRoom.CustomProperties[TURN_COUNT];
                    UpdateHistoryGUI();
                }
            }
            
        }
        if(newTurn)
        {

            newTurn = false;
            PrepareTrends();
        }
    }
    private void PopulateTrendsList()
    {
         TRENDS_VALUES= new int[21];
        for(int i =-10;i<11;i++)
        {
            int index = i + 10;
            TRENDS_VALUES[index] = i;
        }
    }

    private void InitialiseHashKeys()
    {
        PLAYER_GOLD= "Player"+actorNumber+"Gold";
        PLAYER_DOLLARS= "Player" + actorNumber + "Dollars";
        PLAYER_EUROS = "Player" + actorNumber + "Euros";
        PLAYER_YENS = "Player" + actorNumber + "Yens";
        PLAYER_POUNDS = "Player" + actorNumber + "Pounds";
}
    private void SetInitialAmounts(bool masterClient)
    {

        myGold = INITIAL_GOLD;
        myEuros = myDollars = myPounds = myYens = INITIAL_CURRENCIES;
        float initD;
        float initE;
        float initP;
        float initY;
        initD= initE = initP=initY = 1;
        if (masterClient)
        {

            SetRoomAmounts(myGold, myDollars, myEuros, myPounds, myYens);
            SetRoomPrices(initD, initE, initP, initY);


        }
        
        UpdateAmountTexts();

    }
    private void SetRoomAmounts(float g,float d, float e, float p, float y)
    {
        
       SetRoomProperty(PLAYER_GOLD, g);
        SetRoomProperty(PLAYER_EUROS, e);
        SetRoomProperty(PLAYER_DOLLARS, d);
        SetRoomProperty(PLAYER_POUNDS, p);
        SetRoomProperty(PLAYER_YENS, y);
    }
    private void SetRoomPrices(float dol,float eur,float pou,float yen)
    {
        SetRoomProperty(EUROS_PRICE, eur);
        SetRoomProperty(DOLLARS_PRICE, dol);
        SetRoomProperty(POUNDS_PRICE, pou);
        SetRoomProperty(YEN_PRICE, yen);
    }
    private void SetRoomTrends(float d, float e, float p, float y)
    {
        SetRoomProperty(DOLLARS_TREND, d);
        SetRoomProperty(EUROS_TREND, e);
        SetRoomProperty(POUNDS_TREND, p);
        SetRoomProperty(YEN_TREND, y);
    }
    private void UpdateTrendLists(int d,int e, int p, int y)
    {
        dollarsTrendInGame[d].enabled=false;
        eurosTrendInGame[e].enabled=false;
        poundsTrendInGame[p].enabled=false;
        yensTrendInGame[y].enabled =false;
    }
    private void PrepareTrends()//set at each new turn
    {
            int randIndDol = Random.Range(0, 21);
            int randIndEur = Random.Range(0, 21);
            int randIndPou = Random.Range(0, 21);
            int randIndYen = Random.Range(0, 21);

        UpdateTrendLists(randIndDol, randIndEur, randIndPou, randIndYen);//we update the visual

        float dolTrend = (float)TRENDS_VALUES[randIndDol] / 10;
        float eurTrend = (float)TRENDS_VALUES[randIndEur] / 10;
        float pouTrend = (float)TRENDS_VALUES[randIndPou] / 10;
        float yenTrend = (float)TRENDS_VALUES[randIndYen] / 10;

        Debug.Log($"D:{dolTrend} E: {eurTrend} P:{pouTrend} Y:{yenTrend} ");


        SetRoomTrends(dolTrend, eurTrend, pouTrend, yenTrend);
        SetRoomProperty(TRENDS_STATUS, 1);

    }
    private void SetTrends()
    {
        //get the increase or decrease value
        float dolPrice = (float)myRoom.CustomProperties[DOLLARS_PRICE];
        float eurPrice = (float)myRoom.CustomProperties[EUROS_PRICE];
        float pouPrice = (float)myRoom.CustomProperties[POUNDS_PRICE];
        float yenPrice = (float)myRoom.CustomProperties[YEN_PRICE];

        float dolTrend = (float)myRoom.CustomProperties[DOLLARS_TREND];
        float eurTrend = (float)myRoom.CustomProperties[EUROS_TREND];
        float pouTrend = (float)myRoom.CustomProperties[POUNDS_TREND];
        float yenTrend = (float)myRoom.CustomProperties[YEN_TREND];

        float newDolPrice = (dolPrice + dolTrend);
        float newEurPrice = (eurPrice + eurTrend);
        float newPouPrice = (pouPrice + pouTrend);
        float newYenPrice = (yenPrice + yenTrend);

        Debug.Log($"new d price :{newDolPrice} new E price : {newEurPrice} new P price :{newPouPrice} new Y price:{newYenPrice} ");

        
            SetRoomPrices(newDolPrice, newEurPrice, newPouPrice, newYenPrice);
        
            UpdateHistory(newDolPrice, newEurPrice, newPouPrice, newYenPrice);
        
            SetRoomProperty(TRENDS_STATUS, 0);
            
        
    }
    private void UpdateHistory(float dollars,float euros,float pound,float yen)
    {
        //depending on turn count, history gets updated
        int turnCount = (int)myRoom.CustomProperties[TURN_COUNT];
        int dTrend = (int)myRoom.CustomProperties[DOLLARS_TREND]*10;
        int eTrend = (int)myRoom.CustomProperties[EUROS_TREND]*10;
        int pTrend = (int)myRoom.CustomProperties[POUNDS_TREND]*10;
        int yTrend = (int)myRoom.CustomProperties[YEN_TREND]*10;

        string newHistoryVal =$"D/{dollars.ToString()}/{dTrend.ToString()}_E/{euros.ToString()}/{eTrend.ToString()}_P/{pound.ToString()}/{pTrend.ToString()}_Y/{yen.ToString()}/{yTrend.ToString()}" ;
        Debug.Log(newHistoryVal);
        switch (turnCount)
        {
            case 1:
                myRoom.CustomProperties.Add(HISTORY_TURN_M3, "D/1/0_E/1/0_P/1/0_Y/1/0");
                myRoom.CustomProperties.Add(HISTORY_TURN_M2, "D/1/0_E/1/0_P/1/0_Y/1/0");
                myRoom.CustomProperties.Add(HISTORY_TURN_M1, "D/1/0_E/1/0_P/1/0_Y/1/0");
                myRoom.CustomProperties.Add(HISTORY_TURN_ACTUAL, newHistoryVal);
                break;
            case 2:
                myRoom.CustomProperties[HISTORY_TURN_M1] = myRoom.CustomProperties[HISTORY_TURN_ACTUAL];
                myRoom.CustomProperties[HISTORY_TURN_ACTUAL]= newHistoryVal;
                break;
            case 3:
                myRoom.CustomProperties[HISTORY_TURN_M2] = myRoom.CustomProperties[HISTORY_TURN_M1];
                myRoom.CustomProperties[HISTORY_TURN_M1] = myRoom.CustomProperties[HISTORY_TURN_ACTUAL];
                myRoom.CustomProperties[HISTORY_TURN_ACTUAL] = newHistoryVal;
                break;
            default:
                myRoom.CustomProperties[HISTORY_TURN_M3] = myRoom.CustomProperties[HISTORY_TURN_M2];
                myRoom.CustomProperties[HISTORY_TURN_M2] = myRoom.CustomProperties[HISTORY_TURN_M1];
                myRoom.CustomProperties[HISTORY_TURN_M1] = myRoom.CustomProperties[HISTORY_TURN_ACTUAL];
                myRoom.CustomProperties[HISTORY_TURN_ACTUAL] = newHistoryVal;
                break;
                
        }

    }
    private void UpdateHistoryGUI()
    {
        //D-1-0_E-1-0_P-1-0_Y-1-0
        string M3 = (string)myRoom.CustomProperties[HISTORY_TURN_M3];
        string M2 = (string)myRoom.CustomProperties[HISTORY_TURN_M2];
        string M1 = (string)myRoom.CustomProperties[HISTORY_TURN_M1];
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
    private void SetRoomProperty(string hashKey, int value)//general room properties
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
    private void SetRoomProperty(string hashKey, float value)//general room properties
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

    private void UpdateAmountTexts()
    {
        goldAmount.text = myGold.ToString();
        dollarsAmount.text = myDollars.ToString();
        eurosAmount.text = myEuros.ToString();
        poundsAmount.text = myPounds.ToString();
        yenAmount.text = myPounds.ToString();
    }


}
