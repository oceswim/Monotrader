
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;


/*
 * This script allows to handle the photon unity engine connexion functions.
 */
public class MainMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject findOponnentPanel = null;
    [SerializeField] private GameObject waitingStatusPanel = null;
    [SerializeField] private TextMeshProUGUI waitingStatusText = null;

    private bool isConnecting = false;
    private const string GameVersion = "0.1";
    private string REDPLAYERPPT;
    private string GREENPLAYERPPT;
    private string BLUEPLAYERPPT;
    private const int maxPlayerPerRoom =4;
    private const int minPlayerPerRoom =2;
    private const string MIN_PLAYER_KEY = "MinPlayers";
    private void Awake()
    {
        PlayerPrefs.DeleteAll();//TO DELLLLLL
        PlayerPrefs.SetInt(MIN_PLAYER_KEY, minPlayerPerRoom);
        PhotonNetwork.AutomaticallySyncScene = false;
    }

    public void FindOponents()
    {
        isConnecting = true;
        findOponnentPanel.SetActive(false);
        waitingStatusPanel.SetActive(true);
        waitingStatusText.text = "Searching.. ";

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
        Debug.Log("Connected to master");
        if(isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        waitingStatusPanel.SetActive(false);
        findOponnentPanel.SetActive(true);
        Debug.Log($"Disconnected due to : {cause }");
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No clients are waiting for oponent, creating new room");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayerPerRoom, IsOpen=true });
        
    }
    public override void OnJoinedRoom()//this is for the player trying to join a party
    {
        Debug.Log("Client successfully joined the room");
        SetKeys();
        SetColor(PhotonNetwork.CurrentRoom);
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log(PhotonNetwork.CurrentRoom.Name + " and p count : " + playerCount);
        if(playerCount < minPlayerPerRoom)
        {
            waitingStatusText.text = "Waiting for oponent";
            Debug.Log("Client waiting for an opponent");
        }
        else
        {
            Debug.Log("Oponent found!-1");
            Debug.Log("Match ready to begin");
            PhotonNetwork.LoadLevel("Game");
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)//this shows to the player already in the room
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == maxPlayerPerRoom)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false; //stops new player from joining
            PhotonNetwork.LoadLevel("Game");
        }
        else if(PhotonNetwork.CurrentRoom.PlayerCount >= minPlayerPerRoom && PhotonNetwork.CurrentRoom.PlayerCount <maxPlayerPerRoom)
        {
            PhotonNetwork.LoadLevel("Game");
        }
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
}
