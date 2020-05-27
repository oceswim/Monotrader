using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System;

public class GameManager : MonoBehaviourPun
{
    private const string prefDice1 = "Dice1Val";
    private const string prefDice2 = "Dice2Val";

    //room property keys
    private const string dicesDoneRollingHashKey = "dicesDone";
    private const string playerReadyHashKey = "playerReady";
    private const string playerInActionHashKey = "playerPlaying";
    private const string gameStateHashKey = "gameState";
    private List<ShootADie> inGameDices = new List<ShootADie>();
    
    private bool myTurn,gameCanStart,dicesRolling;
    private int playerCount, playerTurnIndex,dice1Val,dice2Val;

    private Room myRoom; 
    private ExitGames.Client.Photon.Hashtable _myCustomProperty = new ExitGames.Client.Photon.Hashtable();
    public GameObject DiceUI;
    public static GameManager instance = null;
    public GameObject Dice;
    public TMP_Text temp;
    // Start is called before the first frame update

    private Player myPlayer;
    private Player[] allPlayers;
    private Player[] otherPlayer;
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
        Debug.Log("The player count : " + playerCount);
        dice1Val = dice2Val =playerTurnIndex = 0;
        Debug.Log(PhotonNetwork.PlayerList[playerTurnIndex].NickName + " you will start");
        gameCanStart =dicesRolling= false;


        myPlayer = PhotonNetwork.LocalPlayer;
        allPlayers = PhotonNetwork.PlayerList;
        otherPlayer = PhotonNetwork.PlayerListOthers;
        myRoom = PhotonNetwork.CurrentRoom;

    }

    // Update is called once per frame
    void Update()
    {
     

        if (!gameCanStart)
        {
            if (myPlayer.IsMasterClient && myRoom.CustomProperties[playerReadyHashKey] !=null)
            {
                int playerReadyCount = (int)myRoom.CustomProperties[playerReadyHashKey];
                if (playerReadyCount == allPlayers.Length)
                {
                    PlayersReady();//sets the game state for everyone
                }
            }
            if (myRoom.CustomProperties[gameStateHashKey] != null)
            {
                int gameState = (int)myRoom.CustomProperties[gameStateHashKey];
                if (gameState == 1)
                {
                    gameCanStart = true;
                }
            }
        }
        if (myTurn && gameCanStart)
        {
            if (dicesRolling)
            {
                Debug.Log("Checking if the dices are done");
                temp.text = "ARe dices done ?";
                int diceStatus = (int)myRoom.CustomProperties[dicesDoneRollingHashKey];
                if (diceStatus == 2)
                {
                    dicesRolling = false;
                    dice1Val = PlayerPrefs.GetInt(prefDice1);
                    dice2Val = PlayerPrefs.GetInt(prefDice2);
                    Debug.Log("Dice stopped rolling");
                    Debug.Log("Dice1: " + dice1Val.ToString() + " Dice 2: " + dice2Val.ToString());
                    temp.text = "Moving of " + (dice1Val + dice2Val).ToString();
                    //activate movement
                    MovementManager.moveMe = true; 
                    //switch turn once all actions are done
                    //SwitchTurn();
                    
                }
            }
        }
        else if(!myTurn && gameCanStart)
        {
            Debug.Log("Checking my turn");
            temp.text = "Checking if my turn";
            if(CheckIfMyTurn())
            {
                myTurn = true;
                DiceUI.SetActive(true);
            }
        }
 


    }

    private void SetRoomProperty()//the room property to check when players are ready
    {
        if (myRoom.CustomProperties[playerReadyHashKey] == null)
        {
            myRoom.CustomProperties.Add(playerReadyHashKey, 1);
        }
        else
        {
            int newValue = (int)myRoom.CustomProperties[playerReadyHashKey];
            myRoom.CustomProperties[playerReadyHashKey] = (newValue+1);
            
        }
        myRoom.SetCustomProperties(myRoom.CustomProperties);
        Debug.Log(myRoom.CustomProperties[playerReadyHashKey]);
        temp.text = myRoom.CustomProperties[playerReadyHashKey].ToString();
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
        int tempo = (int)myRoom.CustomProperties[hashKey];
        temp.text = allPlayers[tempo].NickName + " it's your turn";
    }
    public void OnConfirmation()
    {

        SetRoomProperty();

       
    }
    private bool CheckIfMyTurn()
    {
        bool isItMyTurn = false;
        int myIndex = myPlayer.ActorNumber - 1;
        int playerToPlay = (int)myRoom.CustomProperties[playerInActionHashKey];
        if (myIndex == playerToPlay)
        {
            temp.text = "It's my turn!";
            isItMyTurn = true;
        }
        return isItMyTurn;
    }
    public void PlayersReady()
    {
        int xVal = 0;
        for (int i = 0; i < 2; i++)
        {
            GameObject prefab = PhotonNetwork.InstantiateSceneObject(Dice.name, new Vector3(xVal, 2, 0), Quaternion.identity);
            //transfer ownership to the player who's turn it is.  
            xVal += 3;
            prefab.name = "Dice" + (i + 1).ToString();
        }
        int starterIndex = UnityEngine.Random.Range(0, allPlayers.Length);
        Debug.Log(allPlayers[starterIndex].NickName + " you are starting");
        SetRoomProperty(playerInActionHashKey, starterIndex);
        SetRoomProperty(gameStateHashKey, 1);
    }

    private void SwitchTurn()
    {
        int index = (int)myRoom.CustomProperties[playerInActionHashKey];
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
        SetRoomProperty(playerInActionHashKey, index);
        
    }

    public void RollDices()
    {
        foreach (ShootADie s in inGameDices)
        {
            s.transform.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
            s.roll = true;
            
        }
        dicesRolling = true;
        SetRoomProperty(dicesDoneRollingHashKey, 0);


    }

    public void AddDiceInstance(ShootADie s)
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " added " + s.gameObject.name + " to dices instance");
        inGameDices.Add(s);
    }

}

