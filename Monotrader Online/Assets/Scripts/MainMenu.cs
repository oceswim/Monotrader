﻿
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine.UI;


/*
 * This script allows to handle the photon unity engine connexion functions.
 */
public class MainMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject findOponnentPanel = null;
    [SerializeField] private GameObject waitingStatusPanel = null;
    [SerializeField] private TextMeshProUGUI waitingStatusText = null;

    public AudioSource GameStarting;
    public TMP_Text gameMode;
    public GameObject FindOponentPanel;
    private int index;
    private bool isConnecting = false;
    private const string GameVersion = "0.1";
    private const string MULTIPLAYER_MODE = "Multiplayer";
    private const string SOLO_MODE = "Solo";
    private string REDPLAYERPPT;
    private string GREENPLAYERPPT;
    private string BLUEPLAYERPPT;
    private const int maxPlayerPerRoom =4;
    private int minPlayerPerRoom;
    private const string MIN_PLAYER_KEY = "MinPlayers";
    private void Awake()
    {
      
        PlayerPrefs.DeleteAll();//TO DELLLLLL
        PhotonNetwork.AutomaticallySyncScene = false;
        index = minPlayerPerRoom = 1;
        gameMode.text = MULTIPLAYER_MODE;
  
    }


    public void FindOponents()
    {
        isConnecting = true;
        if (findOponnentPanel.activeSelf)
        {
            findOponnentPanel.SetActive(false);
        }
        waitingStatusPanel.SetActive(true);
        if (minPlayerPerRoom == 1)
        {
            waitingStatusText.text = "Game is about to start...";
        }
        else
        {
            waitingStatusText.text = "Searching.. ";
        }

        if(PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.GameVersion = GameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    public override void OnConnectedToMaster()
    {

        if(isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        if (waitingStatusPanel != null)
        {
            waitingStatusPanel.SetActive(false);
        }
        if (findOponnentPanel != null)
        {
            findOponnentPanel.SetActive(true);
        }
        Debug.Log($"Disconnected due to : {cause }");
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {

        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayerPerRoom, IsOpen=true });
        
    }
    public override void OnJoinedRoom()//this is for the player trying to join a party
    {

        SetKeys();
        SetColor(PhotonNetwork.CurrentRoom);
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        if(playerCount < minPlayerPerRoom)
        {
            waitingStatusText.text = "Waiting for oponent";
            Debug.Log("Client waiting for an opponent");
        }
        else
        {
            StartCoroutine("LoadLevel");
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)//this shows to the player already in the room
    {
       
        if (PhotonNetwork.CurrentRoom.PlayerCount == maxPlayerPerRoom)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false; //stops new player from joining
            StartCoroutine("LoadLevel");
        }
        else if(PhotonNetwork.CurrentRoom.PlayerCount >= minPlayerPerRoom && PhotonNetwork.CurrentRoom.PlayerCount <maxPlayerPerRoom)
        {
            StartCoroutine("LoadLevel");
        }
    }
    private IEnumerator LoadLevel()
    {
        waitingStatusText.text = "Starting!";
        GameStarting.Play();
        yield return new WaitForSeconds(1);
        PhotonNetwork.LoadLevel("Game");
    }
    private void SetKeys()
    {
        REDPLAYERPPT = PhotonNetwork.NickName + "RedVal";
        GREENPLAYERPPT = PhotonNetwork.NickName + "GreenVal";
        BLUEPLAYERPPT = PhotonNetwork.NickName + "BlueVal";
    }
    private void SetColor(Room theRoom)
    {
        int r = UnityEngine.Random.Range(0, 256);
        int g = UnityEngine.Random.Range(0, 256);
        int b = UnityEngine.Random.Range(0, 256);
        PlayerPrefs.SetInt(REDPLAYERPPT, r);
        PlayerPrefs.SetInt(GREENPLAYERPPT, g);
        PlayerPrefs.SetInt(BLUEPLAYERPPT, b);
        SetRoomProperty(theRoom,REDPLAYERPPT, r);
        SetRoomProperty(theRoom,GREENPLAYERPPT, g);
        SetRoomProperty(theRoom,BLUEPLAYERPPT, b);

    }
    private void SetRoomProperty(Room theRoom,string hashKey, int value)//general room properties
    {
        if (theRoom.CustomProperties[hashKey] == null)
        {
            theRoom.CustomProperties.Add(hashKey, value);
        }
        else
        {

            theRoom.CustomProperties[hashKey] = value;

        }
        theRoom.SetCustomProperties(theRoom.CustomProperties);

    }
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void SwitchModes(int i)
    {
        index += i;
        if(index>1)
        {
            index = 0;
        }
        else if(index <0)
        {
            index = 1;
        }
        switch(index)
        {
            case 0:
                gameMode.text = SOLO_MODE;
                break;
            case 1:
                gameMode.text = MULTIPLAYER_MODE;
                break;
        }
    }
    public void ConfirmMode()
    {
        Debug.Log("index is :" + index);
        switch (index)
        {
            case 0:
                minPlayerPerRoom = 1;
                FindOponents();
                break;
            case 1:
                minPlayerPerRoom = 2;
                findOponnentPanel.SetActive(true);
                break;
        }
        PlayerPrefs.SetInt(MIN_PLAYER_KEY, minPlayerPerRoom);
    }
    public void ResetButton(Button theButton)
    {
        theButton.enabled = false;
        theButton.enabled = true;
    }
}
