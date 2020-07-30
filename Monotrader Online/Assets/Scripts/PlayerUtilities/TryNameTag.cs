using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

/*
 * Allows to show the other player names and hide the name if it's our name.
 */
public class TryNameTag : MonoBehaviourPunCallBacks
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
       SetName();
    }

    //sets the text of the billboard to the corresponding player name
    private void SetName()
    {
        string theWholeName = photonView.Owner.NickName;
        foreach (char c in photonView.Owner.NickName)
        {
            if(photonView.Owner.NickName.StartsWith(c.ToString()))
            {
                Debug.Log("First letter is: " + c);
                nameText.text = c.ToString().ToUpper();
                continue;
            }
        }
       
        if (!theWholeName.Equals(myPlayer.NickName))
        {
            string rPpt = theWholeName + "RedVal";
            string gPpt = theWholeName + "GreenVal";
            string bPpt = theWholeName + "BlueVal";
            Debug.Log($" red key {rPpt} blue key {bPpt} green key {gPpt}");
            int r = (int)myRoom.CustomProperties[rPpt];
            int g = (int)myRoom.CustomProperties[gPpt]; 
            int b = (int)myRoom.CustomProperties[bPpt];
            Debug.Log(b + " blue color");
            nameText.overrideColorTags = true;
            nameText.color = new Color32((byte)r, (byte)g, (byte)b, 255);
        }
    }

}
