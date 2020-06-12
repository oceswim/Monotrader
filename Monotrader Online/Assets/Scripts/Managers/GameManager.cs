using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System;
using System.Collections;

public class GameManager : MonoBehaviourPun
{
    private const string PREFDICE = "DiceVal";


    //room property keys
    private const string PLAYER_STATE = "myState";
    private const string PLAYER_READY_HASHKEY = "playerReady";
    private const string PLAYER_IN_ACTION_HASHKEY = "playerPlaying";
    private const string GAME_STATE_HASHKEY = "gameState";
    private const string DICE_1_HASHKEY = "Dice1Name";
    private const string DICE_2_HASHKEY = "Dice2Name";
    private const string TURN_COUNT = "TurnCount";
    private const string PLAYERS_NEW_TURN = "Player_new_turn";
    private const string NEW_TURN_ACTIVE = "NewTurnActive";

    //private variables
    private Player myPlayer;
    private Player[] allPlayers;
    private List<DicesManager> inGameDices = new List<DicesManager>();
    private bool myTurn, gameCanStart, dicesRolling;
    private int moveVal, diceStatus, turnCounter;
    

    //public static variables
    public static ExitGames.Client.Photon.Hashtable _myCustomProperty = new ExitGames.Client.Photon.Hashtable();
    public static GameManager instance = null;
    public static Room myRoom;

    //public variables
    public GameObject DiceUI;
    public GameObject Dice;
    public TMP_Text PlayerTurn,turnCountText;

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
    void Start()
    {
        moveVal   = diceStatus= turnCounter= 0;
        gameCanStart =dicesRolling= false;
        myPlayer = PhotonNetwork.LocalPlayer;
        allPlayers = PhotonNetwork.PlayerList;
        myRoom = PhotonNetwork.CurrentRoom;
        SetRoomProperty(TURN_COUNT, 1);
        turnCountText.text = "Turn #1";
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
                    Debug.Log("EVERYONE READY " + myPlayer.NickName);
                    PlayersReady();//sets the game state for everyone
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
                    Debug.Log("Move Val: " + moveVal);
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
        Debug.Log("there are "+myRoom.CustomProperties[PLAYER_READY_HASHKEY]+" players ready");
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
        int playerToPlay = (int)myRoom.CustomProperties[PLAYER_IN_ACTION_HASHKEY];
        if (myIndex == playerToPlay)
        {
            isItMyTurn = true;
            PlayerTurn.text = "Your turn";
        }
        else
        {
            PlayerTurn.text = PhotonNetwork.PlayerList[playerToPlay].NickName+"'s turn";
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
        int index = (int)myRoom.CustomProperties[PLAYER_IN_ACTION_HASHKEY];
        myTurn = false;
        if (index ==(allPlayers.Length-1))
        {
            index = 0;
  
        }
        else
        {
            index += 1;

        }
        SetRoomProperty(PLAYER_IN_ACTION_HASHKEY, index);
        
    }

    //checks if every player went through 1 lap and gives the player who just completed one a fixed amount of money.
    public void TurnManager()
    {
        turnCounter++;
        
        SetRoomPlayersNewTurn(1);
        int playersTurnUpdated = (int)myRoom.CustomProperties[PLAYERS_NEW_TURN];
        Debug.Log(playersTurnUpdated + "vs " + myRoom.CustomProperties[PLAYERS_NEW_TURN]);
        if (playersTurnUpdated == PhotonNetwork.PlayerList.Length)
        {
            SetRoomPlayersNewTurn(0);
            Debug.Log("every player passed one tour");
            UpdateTurnCount();//overall turn gets increased
            SetRoomProperty(NEW_TURN_ACTIVE, 1);//allows money manager to do the new turn 
            
        }
        else
        {
            Debug.Log("not yet new turn");
            //give player the money
            //update the amount text
        }

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
                Debug.Log("INF");
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
        Debug.Log("previous count was :" + (int)myRoom.CustomProperties[TURN_COUNT] + " it is now : " + previousCount);
        SetRoomProperty(TURN_COUNT, previousCount);
    }

    //starts the dice rolling mechanics when the player clicks on the roll button.
    public void RollDices()
    {

        foreach (DicesManager s in inGameDices)
        {
            s.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
            s.roll = true;
        }
        dicesRolling = true;

    }


    //updates the dices player prefs value
    public void SetDicePrefs(int val)
    {

        moveVal += val;
        Debug.Log("move val " + moveVal);
        
        diceStatus += 1;
        Debug.Log(diceStatus+"dice status");


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

   
}

