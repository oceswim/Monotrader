﻿using Photon.Pun;
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
    public static bool exitAfterDQ, playerWasDQ;
    private bool turnToSet=true;
    private void Update()
    {
        if(exitAfterDQ)
        {
            exitAfterDQ = false;
            ExitGame(GAME_EXIT);
        }
    }
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
                    if (!photonView.IsMine)
                    {
                        photonView.TransferOwnership(PhotonNetwork.LocalPlayer);

                    }
                    photonView.RPC("TurnToSet", RpcTarget.AllBuffered, GameManager.instance.myTurn); 
                    PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
                    exitSound.Play();
                    PhotonNetwork.LeaveRoom();
                    

                }
                break;
        }

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
        if (!turnToSet)
        {
            if (GameManager.instance.myTurn)
            {
                Debug.Log("Im " + PhotonNetwork.LocalPlayer + " and it's my turn");
                int temp = GetIndex();
                if (!photonView.IsMine)
                {
                    photonView.TransferOwnership(PhotonNetwork.LocalPlayer);

                }
                photonView.RPC("SetIndPlayerToPlay", RpcTarget.AllBuffered, temp);
               
            }
        }
        else
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                int size = PhotonNetwork.PlayerList.Length;
                int randInt = Random.Range(0, size);
                Debug.Log("the random int :" + randInt);
                if (!photonView.IsMine)
                {
                    photonView.TransferOwnership(PhotonNetwork.LocalPlayer);

                }
                photonView.RPC("SetIndPlayerToPlay", RpcTarget.AllBuffered, randInt);
            }
        }
        GameManager.diceRollCount = 0;//resets the dice roll count to make sure its value matches the new player count    

        if (!playerWasDQ)
        {
            if (FriendsManager.instance.playerItems.Count < PlayerPrefs.GetInt(MIN_PLAYER_KEY))
            {
                onePlayerLeft.SetActive(true);
            }
        }
        else
        {
            playerWasDQ = false;
        }


    }
    private int GetIndex()
    {
        int index = 0;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if(PhotonNetwork.PlayerList[i].Equals(PhotonNetwork.LocalPlayer))
            {
                index = i;
                continue;
            }
        }
        return index;
    }
    public void ContinueAlone()
    {
       
        GameManager.playerIndexToPlay = 0;
        
    }
    public override void OnLeftRoom()
    {

        PhotonNetwork.LoadLevel(0);//back to menu on left room
    }

    [PunRPC]
    private void TurnToSet(bool toSet)
    {
        turnToSet = toSet;
    }
}
