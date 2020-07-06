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
    public void ExitGame(string mode)
    {
        switch (mode)
        {
            case MENU_EXIT:
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                Application.Quit();
                break;
            case GAME_EXIT:
                if (FriendsManager.instance.CallRPCFriendLeaving(PhotonNetwork.LocalPlayer.NickName))
                {
                    PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
                    PhotonNetwork.LeaveRoom();
                }
                break;
        }

    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (FriendsManager.instance.playerItems.Count < PlayerPrefs.GetInt(MIN_PLAYER_KEY))
        {
            ExitGame(GAME_EXIT);
        }
    }
    public override void OnLeftRoom()
    {

        PhotonNetwork.LoadLevel(0);//back to menu on left room
    }


}
