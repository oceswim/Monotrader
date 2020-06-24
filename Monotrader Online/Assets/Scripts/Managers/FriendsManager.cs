using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class FriendsManager : MonoBehaviourPun
{
    //keeps track of the friend item instances.
    //when can update -> request ownership of the view and update the values on change.
    //depending on the namelabeltext of friendItem.
    //have a list of friends items.
    private FriendItem myPlayerItem;
    public List<FriendItem> playerItems = new List<FriendItem>();
    public static FriendsManager instance = null;
    private FriendItem myItem;
    private const string FORTUNE = "myFortune";
    public static bool changeFortune,initialFortune;
    private Player myPlayer;
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
        changeFortune = false;
        myPlayer = PhotonNetwork.LocalPlayer;
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
                Debug.Log("Fortune ownership to : " + photonView.Owner.NickName);
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
                    Debug.Log("Fortune ownership to master : " + photonView.Owner.NickName);
                }
                string fortune = PlayerPrefs.GetFloat(FORTUNE).ToString();
                photonView.RPC("UpdateAllFortune", RpcTarget.AllBuffered, fortune);

            }
        }

    }
    public void SetFriendInstance(FriendItem instance)
    {
        myPlayerItem = instance;
        Debug.Log("instance set for " + PhotonNetwork.LocalPlayer.NickName);
    }
    public void AddInstanceToList(FriendItem instance)
    {
        playerItems.Add(instance);
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
        photonView.RPC("UpdateOtherPlayer", RpcTarget.AllBuffered, myName, fortune);
    }

    [PunRPC]
    private void UpdateOtherPlayer(string name,string fortune)
    {
        if(!myPlayer.NickName.Equals(name))
        {
            Debug.Log("In if update other player");
            foreach(FriendItem f in playerItems)
            {
                Debug.Log(f.NameLabel.text + " in foreach vs name : "+ name);
                if(f.NameLabel.text.Equals(name))
                {
                    Debug.Log($"im {PhotonNetwork.LocalPlayer.NickName} and i update {name}");
                    f.FortuneLabel.text = fortune;
                }
            }
        }
    }
}
