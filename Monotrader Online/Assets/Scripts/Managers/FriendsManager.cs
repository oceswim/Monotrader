using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class FriendsManager : MonoBehaviourPun
{
    //keeps track of the friend item instances.
    //when can update -> request ownership of the view and update the values on change.
    //depending on the namelabeltext of friendItem.
    //have a list of friends items.
    private FriendItem myPlayerItem;
    public List<FriendItem> playerItems = new List<FriendItem>();
    public static FriendsManager instance = null;
    private const string FORTUNE = "myFortune";
    private const string FRIENDTOCHANGE = "friendToChange";
    private string REDPREF;
    private string GREENPREF;
    private string BLUEPREF;
    public static bool changeFortune, initialFortune, colorSet;//are activated in money manager
    private Player myPlayer;
    private Room myRoom;
    private int r, g, b, transparency;
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
        myPlayer = PhotonNetwork.LocalPlayer;
        myRoom = GameManager.myRoom;
        SetKeys();
        changeFortune = colorSet = false;
        InitialiseColor();
        Debug.Log($"my color:{r}-{g}-{b}/{myPlayer.NickName}");
    }
    private void SetKeys()
    {
        REDPREF = myPlayer.NickName + "RedVal";
        GREENPREF = myPlayer.NickName + "GreenVal";
        BLUEPREF = myPlayer.NickName + "BlueVal";
    }
    // Update is called once per frame
    void Update()
    {
        if (changeFortune)
        {
            changeFortune = false;
            if (!photonView.IsMine)
            {
                photonView.TransferOwnership(myPlayer);

            }
            string theFortune = PlayerPrefs.GetFloat(FORTUNE).ToString();
            UpdateMyFortune(theFortune);
        }

        if (initialFortune)
        {
            initialFortune = false;
            if (myPlayer.IsMasterClient)
            {
                if (!photonView.IsMine)
                {
                    photonView.TransferOwnership(myPlayer);

                }
                string fortune = PlayerPrefs.GetFloat(FORTUNE).ToString();
                photonView.RPC("UpdateAllFortune", RpcTarget.AllBuffered, fortune);

            }
        }

        if (playerItems.Count == PhotonNetwork.PlayerList.Length && !colorSet)
        {
            colorSet = true;

            if (!photonView.IsMine)
            {
                photonView.TransferOwnership(myPlayer);

            }

            photonView.RPC("UpdateOtherColor", RpcTarget.AllBuffered, "You");
        }

    }

    public static void UpdateColors(string nameOfFriend)
    {
        Debug.Log("In update other function of :" + PhotonNetwork.LocalPlayer.NickName + " friend to update: " + nameOfFriend);
        PlayerPrefs.SetString(FRIENDTOCHANGE, nameOfFriend);
        colorSet = true;

    }
    private void InitialiseColor()
    {
        r = PlayerPrefs.GetInt(REDPREF);
        g = PlayerPrefs.GetInt(GREENPREF);
        b = PlayerPrefs.GetInt(BLUEPREF);

        if (r > 200 && b > 200 && g > 200)
        {
            transparency = 100;
        }
        else
        {
            transparency = 255;
        }
    }
    public void SetFriendInstance(FriendItem instance)
    {
        myPlayerItem = instance;
    }
    public void AddInstanceToList(FriendItem instance)
    {
        playerItems.Add(instance);
        if (instance.NameLabel.Equals("You"))
        {
            Debug.Log(myPlayer.NickName + " set his item instance");
            myPlayerItem = instance;
        }

    }

    [PunRPC]
    private void UpdateAllFortune(string fortune)
    {
        foreach (FriendItem f in playerItems)
        {
            f.FortuneLabel.text = fortune;
        }

    }

    private void UpdateMyFortune(string fortune)
    {
        myPlayerItem.FortuneLabel.text = fortune;
        string myName = myPlayer.NickName;
        photonView.RPC("UpdateOtherPlayer", RpcTarget.AllBuffered, myName, fortune);//update the fortune of this player to other players.
    }

    [PunRPC]
    private void UpdateOtherPlayer(string name, string fortune)
    {
        if (!myPlayer.NickName.Equals(name))
        {

            foreach (FriendItem f in playerItems)
            {

                if (f.NameLabel.text.Equals(name))
                {

                    f.FortuneLabel.text = fortune;
                }
            }
        }
    }

    private void UpdateMyItemColor(FriendItem theItem)
    {


        Color32 color = new Color32((byte)r, (byte)g, (byte)b, (byte)transparency);
        var newColorBlock = theItem.GetComponent<Button>().colors;
        newColorBlock.disabledColor = color;
        theItem.GetComponent<Button>().colors = newColorBlock;



    }

    [PunRPC]
    private void UpdateOtherColor(string theName)
    {

        foreach (FriendItem f in playerItems)
        {
            if (f.NameLabel.text.Equals(theName))
            {
                int red = PlayerPrefs.GetInt(REDPREF);
                int green = PlayerPrefs.GetInt(GREENPREF);
                int blue = PlayerPrefs.GetInt(BLUEPREF);
                Color32 color = new Color32((byte)red, (byte)green, (byte)blue, 255);
                var newColorBlock = f.GetComponent<Button>().colors;
                newColorBlock.disabledColor = color;
                f.GetComponent<Button>().colors = newColorBlock;
                Debug.Log($"I'm {myPlayer.NickName} and i update {f.NameLabel.text} color with : {red}-{green}-{blue}");
            }
            else
            {
                string name = f.NameLabel.text;
                Debug.Log(name + " vs " + myPlayer.NickName);
                string rPpt = name + "RedVal";
                string gPpt = name + "GreenVal";
                string bPpt = name + "BlueVal";

                int red = (int)myRoom.CustomProperties[rPpt];
                int green = (int)myRoom.CustomProperties[gPpt];
                int blue = (int)myRoom.CustomProperties[bPpt];

                Color32 color = new Color32((byte)red, (byte)green, (byte)blue, 255);
                var newColorBlock = f.GetComponent<Button>().colors;
                newColorBlock.disabledColor = color;
                f.GetComponent<Button>().colors = newColorBlock;
                Debug.Log($"I'm {myPlayer.NickName} and i update {f.NameLabel.text} color with : {red}-{green}-{blue}");
            }
        }

    }


}

