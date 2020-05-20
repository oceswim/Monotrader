
using UnityEngine;

using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class MainMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject findOponnentPanel = null;
    [SerializeField] private GameObject waitingStatusPanel = null;
    [SerializeField] private TextMeshProUGUI waitingStatusText = null;

    private bool isConnecting = false;
    private const string GameVersion = "0.1";
    private const int maxPlayerPerRoom = 2;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
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
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayerPerRoom });
    }
    public override void OnJoinedRoom()//this is for the player trying to join a party
    {
        Debug.Log("Client successfully joined the room");
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        if(playerCount != maxPlayerPerRoom)
        {
            waitingStatusText.text = "Waiting for oponent";
            Debug.Log("Client waiting for an opponent");
        }
        else
        {
            waitingStatusText.text = "Oponent found!";
            Debug.Log("Match ready to begin");
            //PhotonNetwork.LoadLevel("Game");
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)//this shows to the player already in the room
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount == maxPlayerPerRoom)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false; //stops new player from joining
            Debug.Log("Match ready to begin - room full");
            waitingStatusText.text = "Opponent found";

            PhotonNetwork.LoadLevel("Game");
        }
    }
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
