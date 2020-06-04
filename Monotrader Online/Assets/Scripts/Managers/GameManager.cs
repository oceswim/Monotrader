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
    private const string PLAYER_READY_HASHKEY = "playerReady";
    private const string PLAYER_IN_ACTION_HASHKEY = "playerPlaying";
    private const string GAME_STATE_HASHKEY = "gameState";
    private const string DICE_1_HASHKEY = "Dice1Name";
    private const string DICE_2_HASHKEY = "Dice2Name";
    private List<DicesManager> inGameDices = new List<DicesManager>();
    
    private bool myTurn,gameCanStart,dicesRolling;
    private int playerCount, playerTurnIndex,moveVal, diceStatus;

    private Room myRoom; 

    public GameObject DiceUI;
    public static GameManager instance = null;
    public GameObject Dice;
    public TMP_Text PlayerTurn;
    // Start is called before the first frame update
    private Player myPlayer;
    private Player[] allPlayers;

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
     
        playerCount = PhotonNetwork.PlayerList.Length;
        moveVal  =playerTurnIndex = diceStatus= 0;
        gameCanStart =dicesRolling= false;
        myPlayer = PhotonNetwork.LocalPlayer;
        allPlayers = PhotonNetwork.PlayerList;
        myRoom = PhotonNetwork.CurrentRoom;

    }

    // Update is called once per frame
    void Update()
    {
     

        if (!gameCanStart)
        {
            if (myPlayer.IsMasterClient && myRoom.CustomProperties[PLAYER_READY_HASHKEY] !=null)
            {
                int playerReadyCount = (int)myRoom.CustomProperties[PLAYER_READY_HASHKEY];
                if (playerReadyCount == allPlayers.Length)
                {
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
                    //add dice instance to list here
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
                    //switch turn once all actions are done
                    //switchturn();
                    
                }
            }
        }
        else if(!myTurn && gameCanStart)
        {
            if(CheckIfMyTurn())
            {
                myTurn = true;
                DiceUI.SetActive(true);
            }
        }
 


    }

    private void SetRoomProperty()//the room property to check when players are ready
    {
        if (myRoom.CustomProperties[PLAYER_READY_HASHKEY] == null)
        {
            myRoom.CustomProperties.Add(PLAYER_READY_HASHKEY, 1);
        }
        else
        {
            int newValue = (int)myRoom.CustomProperties[PLAYER_READY_HASHKEY];
            myRoom.CustomProperties[PLAYER_READY_HASHKEY] = (newValue+1);
            
        }
        myRoom.SetCustomProperties(myRoom.CustomProperties);
        Debug.Log(myRoom.CustomProperties[PLAYER_READY_HASHKEY]);
    }
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
    public void OnConfirmation()
    {

        SetRoomProperty();

       
    }
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

    public void SwitchTurn()
    {
        int index = (int)myRoom.CustomProperties[PLAYER_IN_ACTION_HASHKEY];
        myTurn = false;
        if (index ==(allPlayers.Length-1))
        {
            index = 0;
            Debug.Log("Switching back to 0");
            
        }
        else
        {
            index += 1;
            Debug.Log("Adding 1");
            
        }
        SetRoomProperty(PLAYER_IN_ACTION_HASHKEY, index);
        
    }

    public void RollDices()
    {

        foreach (DicesManager s in inGameDices)
        {
            s.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
            s.roll = true;
        }
        dicesRolling = true;

    }
    public void SetDicePrefs(int val)
    {

        moveVal += val;
        Debug.Log("move val " + moveVal);
        
        diceStatus += 1;
        Debug.Log(diceStatus+"dice status");


    }

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

