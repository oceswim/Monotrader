﻿using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class GameModeManager : MonoBehaviourPunCallbacks
{
    private const string FORTUNE = "myFortune";
    private const string WIN_MESSAGE = "Congrats ! You won the game!";
    private const string DRAW_MESSAGE = "Congrats ! You reached the end, it's a draw!";
    public const string TIMER_TOGGLE_NAME = "Timer";
    public const string SCORE_TOGGLE_NAME = "ScoreReach";
    public const string STARTED_ALREADY = "startedAlready";
    private const int TIME_LIMIT = 1;
    private const int AMOUNT_REACH = 2;
    private const float AMOUNT_LIMIT = 30000;
    private const float AMOUNT_LIMIT_SINGLE = 20000;
    private float THE_AMOUNT = 0;
    private const int FINAL_SECONDS = 6;
    private float TIMER_LIMIT = 900; //15 mins = 60 * 15
    private float TIMER_END = 0;
    private string winnerName;
    private int mode,playerAmount;
    private bool timerOn, started,amountGoal,finalSeconds, ticksOn;
    private Room myRoom;
    private Player myPlayer;
    public AudioSource ClockTicks,winSound,loseSound;
    public Toggle timeLimit, scoreLimit;
    public GameObject MasterPanel, OtherPanel, display, charSelectionPanel, winPanel, losePanel, diceRollButton, bankManager,scoresPanel,line2Panel;
    public PrefabSpawner thePrefabSpawner;
    public StatePresetManager theStateManager;
    public TMP_Text gameModeText,lostText,wonText,p1Score,p2Score,p3Score,p4Score;

    public static bool checkFortune, playerNameTagOn, arrivedLate, disqualified;


    // Start is called before the first frame update
    // Update is called once per frame

    private void Start()
    {

        myPlayer = PhotonNetwork.LocalPlayer;
        myRoom = GameManager.myRoom;
        playerAmount = myRoom.PlayerCount;
        started =playerNameTagOn=arrivedLate=finalSeconds= false;
        if (myRoom.CustomProperties[STARTED_ALREADY] != null)
        {
            if ((int)myRoom.CustomProperties[STARTED_ALREADY] == 1)
            {
                Debug.Log("Oh no i'm late");
                LateSetUp();
            }
        }
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
                    photonView.RPC("ActivateGameMode", RpcTarget.AllBuffered, mode);

                }
            }
            else
            {
                if (timerOn)
                {
                if (TIMER_LIMIT > 0)
                {
                    if(finalSeconds)
                    {
                        if(!ticksOn)
                        {
                            ClockTicks.Play();
                            ticksOn = true;
                        }
                        TIMER_END = TIMER_END + Time.deltaTime;
                        if (TIMER_END >= 0.5)
                        {
                            gameModeText.enabled = true;
                        }
                        if (TIMER_END >= 1)
                        {
                            gameModeText.enabled = false;
                            TIMER_END = 0;
                        }
                    }
                    TIMER_LIMIT -= Time.deltaTime;
                    DisplayTime(TIMER_LIMIT);
                }
                else
                {
                    ClockTicks.Stop();
                    DeactivateRollButton();
                    gameModeText.text = "Done!";
                    string key = "Player" + myPlayer.ActorNumber + "Fortune";
                    if (myRoom.CustomProperties[key] == null)
                    {
                        float totalFortune = MoneyManager.PLAYER_FORTUNE + MoneyManager.PLAYER_SAVINGS;
                        SetRoomProperty(key,totalFortune);
                    }
                    if (myPlayer.IsMasterClient)
                    {
                        bool goodToGo = true;
                        foreach(Player p in PhotonNetwork.PlayerListOthers)
                        {
                            string theKey = "Player" + p.ActorNumber + "Fortune";
                            if(myRoom.CustomProperties[theKey] == null)
                            {
                                goodToGo = false;
                                continue;
                            }
                        }
                        if (goodToGo)
                        {
                            if (PhotonNetwork.PlayerListOthers.Length > 0)
                            {
                                CheckWinner();
                            }
                            else
                            {
                                EndGameMessage();
                            }
                            timerOn = false;
                        }
                        
                    }
                    else
                    {
                        timerOn = false;
                    }
                       
                    
                    }
                }
                else
                {
                    if (amountGoal)
                    {
                        if (checkFortune)
                        {
                            checkFortune = false;
                            if ((MoneyManager.PLAYER_FORTUNE + MoneyManager.PLAYER_SAVINGS) > THE_AMOUNT)
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
        
            if(disqualified)
        {
            disqualified = false;
            if (!photonView.IsMine)
            {
                photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            }
            string winnerName = PhotonNetwork.PlayerListOthers[0].NickName;
            photonView.RPC("WinnerPanelActivationAfterDQ", RpcTarget.AllBuffered, winnerName);
        }
    }
    //this is for master player only
    public void ConfirmGameMode()
    {
        SetRoomProperty(STARTED_ALREADY, 1);
       if(timeLimit.isOn)
        {
            mode = TIME_LIMIT;
        }
       else if(scoreLimit.isOn)
        {
            mode = AMOUNT_REACH;
        }
        if(!photonView.IsMine)
        {
            photonView.TransferOwnership(myPlayer);
        }
        photonView.RPC("GoToCharSelect", RpcTarget.AllBuffered);
        int temp = UnityEngine.Random.Range(0, PhotonNetwork.PlayerList.Length);
        photonView.RPC("SetIndPlayerToPlay", RpcTarget.AllBuffered, temp);
    }

    [PunRPC]
    private void GoToCharSelect()
    {

        if(PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;//we close the room to anyone else trying to join it when master client chose the game mode.
            MasterPanel.SetActive(false);

        }
        else
        {
            OtherPanel.SetActive(false);
        }
        thePrefabSpawner.enabled = true;
        theStateManager.enabled = true;
        charSelectionPanel.SetActive(true);
        display.SetActive(true);
        bankManager.SetActive(true);
        playerNameTagOn = true;
    }    
 
    public void LateSetUp()
    {
        if (!photonView.IsMine)
        {
            photonView.TransferOwnership(myPlayer);
        }
        photonView.RPC("GivePlayerIndToPlay", RpcTarget.AllBuffered);
        Debug.Log("player ind to play :" + GameManager.playerIndexToPlay);
        OtherPanel.SetActive(false);
        thePrefabSpawner.enabled = true;
        theStateManager.enabled = true;
        charSelectionPanel.SetActive(true);
        display.SetActive(true);
        bankManager.SetActive(true);
        playerNameTagOn = true;
        
    }

    private void DisplayTime(float timeToDisplay)
    {
        
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        
        gameModeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        if(timeToDisplay<FINAL_SECONDS && !finalSeconds)
        {
            finalSeconds = true;
            gameModeText.color = new Color(255, 0, 0, 255);
        }
    }
    private void CheckWinner()
    {
        float winnerFortune = MoneyManager.PLAYER_FORTUNE + MoneyManager.PLAYER_SAVINGS;
        winnerName = myPlayer.NickName;
        int draw = 0;
        foreach(Player p in PhotonNetwork.PlayerListOthers)
        {

            string key = "Player" + p.ActorNumber + "Fortune";

            if((float)myRoom.CustomProperties[key]>winnerFortune)
            {
                winnerFortune = (float)myRoom.CustomProperties[key];
                winnerName = p.NickName;

            }
            else
            {
                draw++;
            }
        }
        if(draw.Equals(PhotonNetwork.PlayerListOthers.Length))
        {
            winnerName = "draw";
        }
        if (!photonView.IsMine)
        {
            photonView.TransferOwnership(myPlayer);
        }
        photonView.RPC("WinnerPanelActivation", RpcTarget.AllBuffered, winnerName);

    }
    private void EndGameMessage()
    {
       
        winnerName = myPlayer.NickName;
        DeactivateRollButton();
        DisplayWinPannel();
        winPanel.SetActive(true);
        scoresPanel.SetActive(true);
        ShowScores();
        winSound.Play();
        wonText.text = "You reached the end of the game, congrats!";

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
                if (PhotonNetwork.PlayerListOthers.Length>0)
                {
                    THE_AMOUNT = AMOUNT_LIMIT;
                }
                else
                {
                    THE_AMOUNT = AMOUNT_LIMIT_SINGLE;
                }
                gameModeText.text = "Get a fortune of " +THE_AMOUNT.ToString()+ " to win!";
                break;
        }
    }
    [PunRPC]
    private void WinnerPanelActivation(string name)
    {
        ChangeStateTime(false);
        DeactivateRollButton();
        if(name.Equals(myPlayer.NickName))
        {

            DisplayWinPannel();

        }
        else if (name.Equals("draw"))
        {
            DisplayDrawPanel();
        }
        else
        {
            DisplayLosePannel(name);
        }
    }
    [PunRPC]
    private float GetPlayerFortune(string name)
    {
        float theFortune = 0;
        if (name.Equals(myPlayer.NickName))
        {

            theFortune = MoneyManager.PLAYER_FORTUNE;
        }
        return theFortune;
    }
    [PunRPC]
    private void WinnerPanelActivationAfterDQ(string name)
    {
        
        DeactivateRollButton();
        if (name.Equals(myPlayer.NickName))
        {
            Exit.playerWasDQ = true;
            DisplayWinPannel();
        }
        else
        {
            Exit.exitAfterDQ = true;
        }
    }
    private void DeactivateRollButton()
    {

        if (diceRollButton.activeSelf)
        {
            diceRollButton.SetActive(false);
        }
    }
    private void DisplayDrawPanel()
    {
        winPanel.SetActive(true);
        scoresPanel.SetActive(true);
        ShowScores();
        winSound.Play();
        wonText.text = DRAW_MESSAGE;
    }
    private void DisplayWinPannel()
    {
        winPanel.SetActive(true);
        scoresPanel.SetActive(true);
        ShowScores();
        
        winSound.Play();
        wonText.text = WIN_MESSAGE;
    }
    private void ShowScores()
    {
        switch(myRoom.PlayerCount)
        {
            case 1:
                line2Panel.SetActive(false);
                p2Score.gameObject.SetActive(false);
                break;
            case 2:
                line2Panel.SetActive(false);
                break;
            case 3:
                p4Score.gameObject.SetActive(false);
                break;
        }
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.ActorNumber == 1)
            {
                float total = MoneyManager.PLAYER_FORTUNE + MoneyManager.PLAYER_SAVINGS;
                p1Score.text = "Your score:\n" + total.ToString();
            }
            else
            {
                string key = "Player" + p.ActorNumber + "Fortune";
                float total = (float)myRoom.CustomProperties[key];
                if (p.ActorNumber == 2)
                {
                    p2Score.text = "P2 score:\n" + total.ToString();
                }
                else if (p.ActorNumber == 3)
                {
                    p3Score.text = "P3 score:\n" + total.ToString();
                }
                else if (p.ActorNumber == 4)
                {
                    p4Score.text = "Your score:\n" + total.ToString();
                }
            }



        }
    }
    private void DisplayLosePannel(string theWinner)
    {
        losePanel.SetActive(true);
        scoresPanel.SetActive(true);
        ShowScores();
        loseSound.Play();
        lostText.text = $"Sorry but {theWinner} won this game... Your time will come soon!";
    }
  
    private void SetRoomProperty(string hashKey, float value)//general room properties
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
        while(myRoom.CustomProperties[hashKey] == null)
        {
            Debug.Log("Waiting");
        }
    }
    private void SetRoomProperty(string hashKey, int value)//general room properties
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
        while (myRoom.CustomProperties[hashKey] == null)
        {
            Debug.Log("Waiting");
        }
    }
    public void ChangeStateTime(bool on)
    {
        if(on)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
    }
}
