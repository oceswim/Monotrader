using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using System;
using Photon.Realtime;

public class GameModeManager : MonoBehaviourPun
{
    private const string FORTUNE = "myFortune";
    private const string WIN_MESSAGE = "Congrats ! You won the game!";
    public const string TIMER_TOGGLE_NAME = "Timer";
    public const string SCORE_TOGGLE_NAME = "ScoreReach";
    private const int TIME_LIMIT = 1;
    private const int AMOUNT_REACH = 2;
    private const float AMOUNT_LIMIT = 50000;
    private float TIMER_LIMIT = 900; //15 mins = 60 * 15
    private string winnerName;
    private int mode;
    private bool timerOn, started,amountGoal;
    private Room myRoom;
    private Player myPlayer;

    public Toggle timeLimit, scoreLimit;
    public GameObject MasterPanel, OtherPanel,display,charSelectionPanel,winPanel,losePanel;
    public StatePresetManager theStateManager;
    public TMP_Text gameModeText,lostText,wonText;

    public static bool checkFortune;


    // Start is called before the first frame update
    // Update is called once per frame

    private void Start()
    {
        myPlayer = PhotonNetwork.LocalPlayer;
        myRoom = GameManager.myRoom;
        started = false;
    }
    public void ConfirmGameMode()
    {
       if(timeLimit.isOn)
        {
            Debug.Log("time limit");
            mode = TIME_LIMIT;
        }
       else if(scoreLimit.isOn)
        {
            Debug.Log("Score limit");
            mode = AMOUNT_REACH;
        }
        if(!photonView.IsMine)
        {
            photonView.TransferOwnership(myPlayer);
        }
        photonView.RPC("GoToCharSelect", RpcTarget.AllBuffered);

    }

    [PunRPC]
    private void GoToCharSelect()
    {
        theStateManager.enabled = true;
        if(PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            MasterPanel.SetActive(false);
        }
        else
        {
            OtherPanel.SetActive(false);
        }
        charSelectionPanel.SetActive(true);
        display.SetActive(true);
    }
    private void Update()
    {
        if (GameManager.instance.gameCanStart && !started)
        {
            started = true;
            if (myPlayer.IsMasterClient)
            {
                if (!photonView.IsMine)
                {
                    photonView.TransferOwnership(myPlayer);
                }
                photonView.RPC("ActivateGameMode", RpcTarget.AllBuffered,mode);
                
            }
            

        }
        else
        {
            if (timerOn)
            {
                if (TIMER_LIMIT > 0)
                {
                    TIMER_LIMIT -= Time.deltaTime;
                    DisplayTime(TIMER_LIMIT);
                }
                else
                {
                    Debug.Log("timer done");
                    gameModeText.text = "Done!";
                    if (myPlayer.IsMasterClient)
                    {
                        CheckWinner();
                    }
                    else
                    {
                        //display a "and the winner is..." panel
                    }
                    timerOn = false;
                }
            }
            else 
            {
                if (amountGoal)
                {
                    if (checkFortune)
                    {
                        checkFortune = false;
                        if (PlayerPrefs.GetFloat(FORTUNE) > AMOUNT_LIMIT)
                        {
                            amountGoal = false;
                            if (!photonView.IsMine)
                            {
                                photonView.TransferOwnership(myPlayer);
                            }
                            winnerName = myPlayer.NickName;
                            photonView.RPC("WinnerPanelActivation", RpcTarget.AllBuffered, winnerName);
                            //you won the game
                        }
                    }
                }
            }
        }
    }
    private void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        gameModeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    private void CheckWinner()
    {
        float winnerFortune = PlayerPrefs.GetFloat(FORTUNE);
        winnerName = myPlayer.NickName;
        foreach(Player p in PhotonNetwork.PlayerListOthers)
        {

            string key = "Player" + p.ActorNumber + "Fortune";
            if((float)myRoom.CustomProperties[key]>winnerFortune)
            {
                winnerFortune = (float)myRoom.CustomProperties[key];
                winnerName = p.NickName;
            }
        }
        if (!photonView.IsMine)
        {
            photonView.TransferOwnership(myPlayer);
        }
        photonView.RPC("WinnerPanelActivation", RpcTarget.AllBuffered, winnerName);

    }
    [PunRPC]
    private void ActivateGameMode(int theMode)
    {
        switch(theMode)
        {
            case TIME_LIMIT:
                timerOn = true;
                DisplayTime(TIMER_LIMIT);
                break;
            case AMOUNT_REACH:
                amountGoal = true;
                gameModeText.text = "Get a fortune of 50,000 to win!";
                break;
        }
    }
    [PunRPC]
    private void WinnerPanelActivation(string name)
    {
        if(name.Equals(myPlayer.NickName))
        {
            Debug.Log("YOU WON");
            DisplayWinPannel();
        }
        else
        {
            DisplayLosePannel(name);
        }
    }
    private void DisplayWinPannel()
    {
        winPanel.SetActive(true);
        wonText.text = WIN_MESSAGE;
    }    
    private void DisplayLosePannel(string WinnerName)
    {
        losePanel.SetActive(true);
        lostText.text = $"Sorry but {winnerName} won this game... Your time will come soon!";
    }
    private void SetRoomProperty(string hashKey, float value)
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
}
