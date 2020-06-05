using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MoneyManager : MonoBehaviour
{
    private const int INITIAL_GOLD = 5000;
    private const int INITIAL_YEN = 25000;
    private const int INITIAL_CURRENCIES = 2500;

    private const string PLAYER_STATE = "myState";
    private const string PLAYER_READY_HASHKEY = "playerReady";
    private string PLAYER_GOLD;
    private string PLAYER_DOLLARS;
    private string PLAYER_EUROS;
    private string PLAYER_YENS;
    private string PLAYER_POUNDS;
    private int myGold, myDollars, myEuros, myYens, myPounds;
    private Player myPlayer;
    private Room myRoom;
    private int playerCount;
    private string actorNumber;
    private bool playersReady;

    private List<Player> notReadyPlayers = new List<Player>();

    public TMP_Text goldAMount,dollarsAmount, eurosAmount, poundsAmount, yenAmount,waitingForText;
    public GameObject waitingForObject;
    // Start is called before the first frame update
    void Start()
    {
        myPlayer = PhotonNetwork.LocalPlayer;
        actorNumber = myPlayer.ActorNumber.ToString();
        playersReady = false;
        playerCount = PhotonNetwork.PlayerList.Length;
        myRoom = PhotonNetwork.CurrentRoom;
        InitialiseHashKeys();
    }

    // Update is called once per frame
    void Update()
    {
        if (myRoom.CustomProperties[PLAYER_READY_HASHKEY] != null)
        {
            if ((int)myRoom.CustomProperties[PLAYER_READY_HASHKEY] == playerCount && !playersReady)
            {
                playersReady = true;
                waitingForObject.SetActive(false);
                SetInitialAmounts(myPlayer.IsMasterClient);
                Debug.Log("everyone is ready!");
            }
            else if(!playersReady)
            {
                string waitingText = "Waiting for ";
                foreach (Player p in PhotonNetwork.PlayerListOthers)
                {
                    if (p.CustomProperties[PLAYER_STATE] == null)
                    {
                        if (!notReadyPlayers.Contains(p))
                        {
                            notReadyPlayers.Add(p);
                            
                        }
                        if (notReadyPlayers.IndexOf(p).Equals(notReadyPlayers.Count - 1))
                        {
                            waitingText += p.NickName + "....";
                        }
                        else
                        {
                            waitingText += p.NickName + ", ";
                        }
                    }
                    else
                    {
                        if (notReadyPlayers.Contains(p))
                        {
                            notReadyPlayers.Remove(p);
                        }

                     }
                }
                                  
                if(!waitingForText.text.Equals(waitingText))
                {
                    waitingForText.text = waitingText;
                }
                else
                {
                    Debug.Log("Same text so not updated");
                    Debug.Log(waitingText + "vs " + waitingForText.text);
                }
            }
            
        }
    }
    private void InitialiseHashKeys()
    {
        PLAYER_GOLD= "Player"+actorNumber+"Gold";
        PLAYER_DOLLARS= "Player" + actorNumber + "Dollars";
        PLAYER_EUROS = "Player" + actorNumber + "Euros";
        PLAYER_YENS = "Player" + actorNumber + "Yens";
        PLAYER_POUNDS = "Player" + actorNumber + "Pounds";
}
    private void SetInitialAmounts(bool masterClient)
    {

        myGold = INITIAL_GOLD;
        myEuros = myDollars = myPounds = INITIAL_CURRENCIES;
        myYens = INITIAL_YEN;
        if (masterClient)
        {
            SetRoomProperty(PLAYER_GOLD, myGold);
            SetRoomProperty(PLAYER_EUROS, myEuros);
            SetRoomProperty(PLAYER_DOLLARS, myDollars);
            SetRoomProperty(PLAYER_POUNDS, myPounds);
            SetRoomProperty(PLAYER_YENS, myYens);
        }
        UpdateAmountTexts();

    }
    private void SetInitialTrends()//value of each currencies in Gold
    {

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

    }

    private void UpdateAmountTexts()
    {
        goldAMount.text = myGold.ToString();
        dollarsAmount.text = myDollars.ToString();
        eurosAmount.text = myEuros.ToString();
        poundsAmount.text = myPounds.ToString();
        yenAmount.text = myPounds.ToString();
    }


}
