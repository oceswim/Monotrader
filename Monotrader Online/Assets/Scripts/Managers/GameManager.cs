using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;


public class GameManager : MonoBehaviourPun
{
    private const string PREFDICE = "DiceVal";

    //room property keys
    private const string DICEROLLCOUNTER = "diceRollsCount";
    private const string PLAYER_STATE = "myState";
    private const string PLAYER_READY_HASHKEY = "playerReady";
    private const string PLAYER_IN_ACTION_HASHKEY = "playerPlaying";
    private const string GAME_STATE_HASHKEY = "gameState";
    private const string DICE_1_HASHKEY = "Dice1Name";
    private const string DICE_2_HASHKEY = "Dice2Name";
    private const string TURN_COUNT = "TurnCount";
    private const string PLAYERS_NEW_TURN = "Player_new_turn";

    private string PLAYER_GOLD;

    //private variables
    private Player myPlayer;
    private Player[] allPlayers;
    private List<DicesManager> inGameDices = new List<DicesManager>();
    private bool myTurn, dicesRolling;
    //private int moveVal, diceStatus, turnCounter;
    private int moveVal, turnCounter;
    
    public int diceStatus;//TEMP

    //public static variables
    public static ExitGames.Client.Photon.Hashtable _myCustomProperty = new ExitGames.Client.Photon.Hashtable();
    public static GameManager instance = null;
    public static Room myRoom;

    //public variables
    public AudioSource mainTheme;
    public AudioSource secondaryTheme;
    public bool gameCanStart;
    public GameObject DiceUI,Dice, gameModeMaster,gameModeOther;
    public TMP_Text turnCountText;

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
        myPlayer = PhotonNetwork.LocalPlayer;
        if (myPlayer.IsMasterClient)
        {
            gameModeMaster.SetActive(true);
        }
        else
        {
            gameModeOther.SetActive(true);
        }
    }
    void Start()
    {
        
        moveVal   = diceStatus= 0 ;
        turnCounter = 1;
        PlayerPrefs.SetInt(TURN_COUNT, turnCounter);
        gameCanStart =dicesRolling= false;

        allPlayers = PhotonNetwork.PlayerList;
        myRoom = PhotonNetwork.CurrentRoom;
        SetRoomProperty(TURN_COUNT, 1);
        turnCountText.text = "Turn #"+turnCounter.ToString();
        SetRoomProperty(PLAYERS_NEW_TURN, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameCanStart)
        {
            if (myPlayer.IsMasterClient && myRoom.CustomProperties[PLAYER_READY_HASHKEY] != null)
            {
                int playerReadyCount = (int)myRoom.CustomProperties[PLAYER_READY_HASHKEY];
                if (playerReadyCount == allPlayers.Length)
                {
                    secondaryTheme.Stop();
                    PlayersReady();//sets the game state for everyone
                    mainTheme.Play();
                }
            }

            if (myRoom.CustomProperties[GAME_STATE_HASHKEY] != null)
            {
                int gameState = (int)myRoom.CustomProperties[GAME_STATE_HASHKEY];
                if (gameState == 1)
                {
                    gameCanStart = true;
                    AddDiceInstance();
                    
                }

            }
        }
        if (myTurn && gameCanStart)
        {
            if (dicesRolling)
            {

                if (diceStatus == 2)
                {
                    diceStatus = 0;
                    dicesRolling = false;
           
                    PlayerPrefs.SetInt(PREFDICE, moveVal);
                    moveVal = 0;
                    //activate movement
                    MovementManager.moveMe = true;
                }
            }
        }
        else if (!myTurn && gameCanStart)
        {
            if (CheckIfMyTurn())
            {

                myTurn = true;
                DiceUI.SetActive(true);
            }
        }


    }
    //sets the player's custom properties with an int
    private void SetCustomPpties(string hashKey,int ind)//
    {
        string playerIndexString = ind.ToString();   
        _myCustomProperty[hashKey] = playerIndexString; 
        myPlayer.CustomProperties = _myCustomProperty;   
        myPlayer.SetCustomProperties(myPlayer.CustomProperties);

        
    }


    //sets the room property when players are ready to play.
    private void SetRoomProperty()//the room property to check when players are ready
    {
        int newValue;
        if (myRoom.CustomProperties[PLAYER_READY_HASHKEY] == null)
        {
            newValue = 1;
            SetRoomProperty(PLAYER_READY_HASHKEY, newValue);
        }
        else
        {
            newValue = (int)myRoom.CustomProperties[PLAYER_READY_HASHKEY] +1;
            SetRoomProperty(PLAYER_READY_HASHKEY,newValue);
            
        }
        SetCustomPpties(PLAYER_STATE, 1);
        
    }

    //setting the room property with an int
    private void SetRoomProperty(string hashKey,int value)//general room properties
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

    //setting the room property with a string
    private void SetRoomProperty(string hashKey, string value)//dice properties
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

    //when the player confirms his character choice
    public void OnConfirmation()
    {

        SetRoomProperty();

       
    }

    //allows to check if it's the player's turn
    private bool CheckIfMyTurn()
    {
        bool isItMyTurn = false;
        int myIndex = myPlayer.ActorNumber - 1;
        int playerToPlay = PlayerPrefs.GetInt(PLAYER_IN_ACTION_HASHKEY);
        if (myIndex == playerToPlay)
        {
            isItMyTurn = true;
        }
        return isItMyTurn;
    }

    //checks if everyplayer is in the room and  ready to play
    public void PlayersReady()
    {

        int xVal = 0;
        for (int i = 0; i < 2; i++)
        {
            GameObject prefab = PhotonNetwork.InstantiateSceneObject(Dice.name, new Vector3(xVal, 2, 0), Quaternion.identity);
            int prefabViewID = prefab.GetComponent<PhotonView>().ViewID;
            if(i==0)
            {
                SetRoomProperty(DICE_1_HASHKEY, prefabViewID);
 
            }
            else if (i == 1)
            {
                SetRoomProperty(DICE_2_HASHKEY, prefabViewID);

            }

            xVal += 3;//seperates the second dice from the first one
        }
        int starterIndex = UnityEngine.Random.Range(0, allPlayers.Length);
        SetRoomProperty(PLAYER_IN_ACTION_HASHKEY, starterIndex);
        SetRoomProperty(GAME_STATE_HASHKEY, 1);
    }

    //allows to give each player the property of the dices
    public void SwitchTurn()
    {
        int index = PlayerPrefs.GetInt(PLAYER_IN_ACTION_HASHKEY);
    
        myTurn = false;
        if (index ==(allPlayers.Length-1))
        {
            index = 0;
  
        }
        else
        {
            index += 1;

        }
        if (!photonView.IsMine)
        {
            photonView.TransferOwnership(myPlayer);
      
        }
        photonView.RPC("SetPlayerTurnPref", RpcTarget.AllBuffered, index);

    }

    //checks if every player went through 1 lap and gives the player who just completed one a fixed amount of money.
    public void TurnManager()
    {
        turnCounter++;
        PlayerPrefs.SetInt(TURN_COUNT, turnCounter);
        SetRoomPlayersNewTurn(1);
        int playersTurnUpdated = (int)myRoom.CustomProperties[PLAYERS_NEW_TURN];
        Debug.Log("Turn manager at " + Time.deltaTime + " by " + PhotonNetwork.LocalPlayer.NickName+" and playersturn updated :"+playersTurnUpdated);
        turnCountText.text = "Turn #"+turnCounter.ToString();
        NewTurnMechanic();
        if (playersTurnUpdated == PhotonNetwork.PlayerList.Length)
        {
            SetRoomPlayersNewTurn(0);
            Debug.Log("New turn for both players by " + PhotonNetwork.LocalPlayer.NickName + " and playersturn updated :" + (int)myRoom.CustomProperties[PLAYERS_NEW_TURN]);
            UpdateTurnCount();//overall turn gets increased
            MoneyManager.newTurnFortune = true;
        }

    }

    /*
     *  give player the money
     *  update the amount text
     *  update fortune in game
     */
    private void NewTurnMechanic()//one player cross new turn
    {
        MoneyManager.instance.NewTurnFunction();
        PLAYER_GOLD = PlayerPrefs.GetString("MYGOLD");
        float newGold = PlayerPrefs.GetFloat(PLAYER_GOLD) + 2000;
        BankManager.instance.UpdateGold(-2000);
        BankManager.Trigger = true;
        PlayerPrefs.SetFloat(PLAYER_GOLD, newGold);
        MoneyManager.instance.UpdateFortuneInGame();

    }
    //updates the room player new turn property
    private void SetRoomPlayersNewTurn(int ind)
    {
        if (ind == 0)
        {
            SetRoomProperty(PLAYERS_NEW_TURN, 0);
        }
        else
        {
            if (myRoom.CustomProperties[PLAYERS_NEW_TURN] != null)
            {
         
                int nextValue = (int)myRoom.CustomProperties[PLAYERS_NEW_TURN] + 1;
                SetRoomProperty(PLAYERS_NEW_TURN, nextValue);
            }
          

        }
    }

    //the turn count is updated when every player goes through one lap which updates the trends 
    private void UpdateTurnCount()
    {
        int previousCount;
        previousCount = (int)myRoom.CustomProperties[TURN_COUNT]+1;
  
        SetRoomProperty(TURN_COUNT, previousCount);
    }

    //starts the dice rolling mechanics when the player clicks on the roll button.
    public void RollDices(bool taxesRoll)
    {

        foreach (DicesManager s in inGameDices)
        {
            s.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
            WaitOut(1);
            s.roll = true;
            if(taxesRoll)
            {
                s.taxesRoll = true;
            }
        }
        if (!taxesRoll)
        {
            dicesRolling = true;

            if (myRoom.CustomProperties[DICEROLLCOUNTER] != null)
            {
                int newCount = (int)myRoom.CustomProperties[DICEROLLCOUNTER] + 1;
                SetRoomProperty(DICEROLLCOUNTER, newCount);
            }
            else
            {
                SetRoomProperty(DICEROLLCOUNTER, 1);
            }
            if ((int)myRoom.CustomProperties[DICEROLLCOUNTER] == myRoom.PlayerCount)
            {
                SetRoomProperty(DICEROLLCOUNTER, 0);
       
                if (!photonView.IsMine)
                {
                    photonView.TransferOwnership(myPlayer);
      
                }
                photonView.RPC("TrendsUpdates", RpcTarget.AllBuffered,true);
            }
        }
    }


    //updates the dices player prefs value
    public void SetDicePrefs(int val)
    {
        Debug.Log("In dice prefs");
        moveVal += val;
  
        diceStatus += 1;
     


    }

    //allows to seperate each dice instantiated
    public void AddDiceInstance()
    {
        int dice1Ind = (int)myRoom.CustomProperties[DICE_1_HASHKEY];
        int dice2Ind = (int)myRoom.CustomProperties[DICE_2_HASHKEY];
        Transform dice1 = PhotonNetwork.GetPhotonView(dice1Ind).transform;
        Transform dice2 = PhotonNetwork.GetPhotonView(dice2Ind).transform;
        inGameDices.Add(dice1.GetComponent<DicesManager>());
        inGameDices.Add(dice2.GetComponent<DicesManager>());
    }

    [PunRPC]
    private void SetPlayerTurnPref(int index)
    {  
        PlayerPrefs.SetInt(PLAYER_IN_ACTION_HASHKEY, index);
        Debug.Log("player pref in action set to :" + index + " by " + myPlayer.NickName) ;
    }

    private void WaitOut(float seconds)
    {
        float start = 0;
        while (start < seconds)
        {
            //Debug.Log("Waiting " + start);
            start += Time.deltaTime;
        }
    }
}

