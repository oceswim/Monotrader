using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;


public class GameManager : MonoBehaviourPunCallBacks
{
    private const string PREFDICE = "DiceVal";

    //room property keys
    private const string PLAYER_STATE = "myState";
    private const string PLAYER_READY_HASHKEY = "playerReady";
    private const string PLAYER_IN_ACTION_HASHKEY = "playerPlaying";
    private const string GAME_STATE_HASHKEY = "gameState";
    private const string DICE_1_HASHKEY = "Dice1Name";
    private const string DICE_2_HASHKEY = "Dice2Name";
    private const string TURN_COUNT = "turnCountOverall";
    private const string PLAYERS_NEW_TURN = "Player_new_turn";

    private string PLAYER_GOLD;

    //private variables
    private Player myPlayer;
    private List<DicesManager> inGameDices = new List<DicesManager>();
    private bool dicesRolling;
  
    //private int moveVal, diceStatus, turnCounter;
    private int moveVal;
    
    public int diceStatus;//TEMP


    //public static variables
    public static ExitGames.Client.Photon.Hashtable _myCustomProperty = new ExitGames.Client.Photon.Hashtable();
    public static GameManager instance = null;
    public static Room myRoom;
    public static int playerIndexToPlay,diceRollCount,TURN_COUNT_VALUE;

    //public variables
    public AudioSource mainTheme;
    public AudioSource secondaryTheme;
    public bool gameCanStart,myTurn;
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
        
        moveVal = diceStatus= 0 ;
        TURN_COUNT_VALUE=1;
        gameCanStart =dicesRolling= false;
        myRoom = PhotonNetwork.CurrentRoom;
        turnCountText.text = "Turn #"+TURN_COUNT_VALUE.ToString();
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
                if (playerReadyCount == PhotonNetwork.PlayerList.Length)
                {
                    
                    PlayersReady();//sets the game state for everyone
                    if(!photonView.IsMine)
                    {
                        photonView.TransferOwnership(myPlayer);
                    }
                    photonView.RPC("StartMainTheme", RpcTarget.AllBuffered);
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
            if (playerIndexToPlay < PhotonNetwork.PlayerList.Length)
            {
                Debug.Log(playerIndexToPlay + " it's the player ind to play it corresponds to " + PhotonNetwork.PlayerList[playerIndexToPlay].NickName);
                if(!myPlayer.Equals(PhotonNetwork.PlayerList[playerIndexToPlay]))
                {
                    myTurn = false;
                    DiceUI.SetActive(false);
                }
            }
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
        if (playerIndexToPlay < PhotonNetwork.PlayerList.Length)
        {
            Player playerToplay = PhotonNetwork.PlayerList[playerIndexToPlay];
            Debug.Log(" - Player to play is :" + playerToplay.NickName);
            if (playerToplay.Equals(myPlayer))
            {
                isItMyTurn = true;
            }
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
                prefab.name = "Dice1";
                SetRoomProperty(DICE_1_HASHKEY, prefabViewID);
 
            }
            else if (i == 1)
            {
                prefab.name = "Dice2";
                SetRoomProperty(DICE_2_HASHKEY, prefabViewID);

            }

            xVal += 3;//seperates the second dice from the first one
        }
        int starterIndex = UnityEngine.Random.Range(1, PhotonNetwork.PlayerList.Length+1);
        SetRoomProperty(PLAYER_IN_ACTION_HASHKEY, starterIndex);
        SetRoomProperty(GAME_STATE_HASHKEY, 1);
    }

    //allows to give each player the property of the dices
    public void SwitchTurn()
    {
        int index;
        //have to operate a change in index here when switch turn after a player leaves
        myTurn = false;
        if (playerIndexToPlay ==(PhotonNetwork.PlayerList.Length-1))
        {
            index = 0;
  
        }
        else
        {
            playerIndexToPlay += 1;
            index = playerIndexToPlay;

        }
        if (!photonView.IsMine)
        {
            photonView.TransferOwnership(myPlayer);
      
        }
        photonView.RPC("SetIndPlayerToPlay", RpcTarget.AllBuffered, index);

    }

    //checks if every player went through 1 lap and gives the player who just completed one a fixed amount of money.
    public void TurnManager()
    {
       
        TURN_COUNT_VALUE++;
        SetRoomPlayersNewTurn(1);
        int playersTurnUpdated = (int)myRoom.CustomProperties[PLAYERS_NEW_TURN];
        turnCountText.text = "Turn #"+TURN_COUNT_VALUE.ToString();
        NewTurnMechanic();
        /*if (playersTurnUpdated == PhotonNetwork.PlayerList.Length)
        {
            SetRoomPlayersNewTurn(0);
            UpdateTurnCount();//overall turn gets increased
            MoneyManager.newTurnFortune = true;
        }*/
        
        MoneyManager.newTurnFortune = true;//allows to update the current bankings value based on the new trends.
    }

    /*
     *  give player the money
     *  update the amount text
     *  update fortune in game
     */
    private void NewTurnMechanic()//one player cross new turn
    {
        MoneyManager.instance.NewTurnFunction();
        PLAYER_GOLD += 2000;
        BankManager.instance.UpdateGold(-2000);
        BankManager.Trigger = true;
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
            if (!photonView.IsMine)
            {
                photonView.TransferOwnership(myPlayer);

            }
            if (diceRollCount>0)
            {
              
                diceRollCount += 1;
                photonView.RPC("SetDiceRollCount", RpcTarget.AllBuffered,diceRollCount);

            }
            else
            {
             
                diceRollCount = 1;
                photonView.RPC("SetDiceRollCount", RpcTarget.AllBuffered, diceRollCount);

            }
            if (diceRollCount == myRoom.PlayerCount)
            {

                diceRollCount = 0;
                photonView.RPC("SetDiceRollCount", RpcTarget.AllBuffered, diceRollCount);
                photonView.RPC("TrendsUpdates", RpcTarget.AllBuffered, true);
            }
        }
    }


    //updates the dices player prefs value
    public void SetDicePrefs(int val)
    {
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
    }
    [PunRPC]
    private void StartMainTheme()
    {
        secondaryTheme.Stop();
        mainTheme.Play();
    }
    [PunRPC]
    private void SetIndPlayerToPlay(int ind)
    {
        if (playerIndexToPlay != ind)
        {
            playerIndexToPlay = ind;
        }
    }
    [PunRPC]
    private void GivePlayerIndToPlay()
    {
        if(myPlayer.IsMasterClient)
        {
            if (!photonView.IsMine)
            {
                photonView.TransferOwnership(myPlayer);
            }
            photonView.RPC("SetIndPlayerToPlay", RpcTarget.AllBuffered,playerIndexToPlay);
        }
    }
    [PunRPC]
    private void SetDiceRollCount(int ind)
    {
        if (diceRollCount != ind)
        {

            diceRollCount = ind;

        }
    }

    private void WaitOut(float seconds)
    {
        float start = 0;
        while (start < seconds)
        {

            start += Time.deltaTime;
        }
    }
}

