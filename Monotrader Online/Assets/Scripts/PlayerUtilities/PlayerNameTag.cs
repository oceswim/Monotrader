using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

/*
 * Allows to show the other player names and hide the name if it's our name.
 */
public class PlayerNameTag : MonoBehaviourPunCallBacks
{
    [SerializeField] private TextMeshProUGUI nameText;
    private Room myRoom;
    private Player myPlayer;
    private bool notStarted;
    void Start()
    {
        myRoom = GameManager.myRoom;
        myPlayer = PhotonNetwork.LocalPlayer;
        if (photonView.IsMine)
        {
            return;
        }
        notStarted = true;

    }
    private void Update()
    {
        if(notStarted && GameModeManager.playerNameTagOn)
        {
            notStarted = false;
            LateStart();
        }
    }
    void LateStart()
    {
     
        if (myPlayer.IsMasterClient)
        {
           
            SetName();
        }

    }

    //sets the text of the billboard to the corresponding player name
    private void SetName()
    {
        nameText.text = photonView.Owner.NickName;
        if (!nameText.text.Equals(myPlayer.NickName))
        {
            string rPpt = nameText.text + "RedVal";
            string gPpt = nameText.text + "GreenVal";
            string bPpt = nameText.text + "BlueVal";
            Debug.Log($" red key {rPpt} blue key {bPpt} green key {gPpt}");
            int r = (int)myRoom.CustomProperties[rPpt];
            int g = (int)myRoom.CustomProperties[gPpt]; 
            int b = (int)myRoom.CustomProperties[bPpt];
            Debug.Log(b + " blue color");
            nameText.overrideColorTags = true;
            nameText.color = new Color32((byte)r, (byte)g, (byte)b, 255);
        }
    }

    private void WaitOut(float seconds)
    {
        float start = 0;
        while (start < seconds)
        {
            //Debug.Log("Waiting " + start);
            start += Time.deltaTime;
        }
    }


}
