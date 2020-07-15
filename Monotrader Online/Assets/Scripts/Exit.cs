using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviourPunCallbacks
{
    private const string MENU_EXIT = "menu";
    private const string GAME_EXIT = "game";
    private const string MIN_PLAYER_KEY = "MinPlayers";
    public AudioSource exitSound;
    public GameObject onePlayerLeft;
    private GameObject Dice1, Dice2;

    public void ExitGame(string mode)
    {
        switch (mode)
        {
            case MENU_EXIT:
                StartCoroutine("QuitApplication");
                break;
            case GAME_EXIT:
                if (FriendsManager.instance.CallRPCFriendLeaving(PhotonNetwork.LocalPlayer.NickName))
                {
                    if (PhotonNetwork.PlayerListOthers.Length > 0)
                    {
                        GameManager.instance.SwitchTurn();
                        //SwitchOwnerShipDice();
                    }
                    PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
                    exitSound.Play();
                    PhotonNetwork.LeaveRoom();
                    

                }
                break;
        }

    }
    private void SwitchOwnerShipDice()
    {
        Dice1 = GameObject.Find("Dice1");
        Dice2 = GameObject.Find("Dice2");

        DicesManager dice1 = Dice1.GetComponent<DicesManager>();
        DicesManager dice2 = Dice2.GetComponent<DicesManager>();

        dice1.switchOwner = true;
        dice2.switchOwner = true;
    }
    private IEnumerator QuitApplication()
    {

        exitSound.Play();
        yield return new WaitForSeconds(1);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(PhotonNetwork.MasterClient.NickName + " is now master client");

        
        if (FriendsManager.instance.playerItems.Count < PlayerPrefs.GetInt(MIN_PLAYER_KEY))
        {
                
            onePlayerLeft.SetActive(true);
            
            
        }

    }

    public override void OnLeftRoom()
    {

        PhotonNetwork.LoadLevel(0);//back to menu on left room
    }


}
