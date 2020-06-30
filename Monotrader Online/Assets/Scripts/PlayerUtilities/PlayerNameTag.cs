using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

/*
 * Allows to show the other player names and hide the name if it's our name.
 */
public class PlayerNameTag : MonoBehaviourPun
{
    [SerializeField] private TextMeshProUGUI nameText;
    private Room myRoom;
    private Player myPlayer;
    private string REDPLAYERPPT;
    private string GREENPLAYERPPT;
    private string BLUEPLAYERPPT;
    private int r, g, b;
    void Start()
    {
        myRoom = GameManager.myRoom;
        myPlayer = PhotonNetwork.LocalPlayer;
        if (photonView.IsMine)
        {
            SetKeys();
            SetColor();
            return;
        }
        SetName();

    }
    private void SetKeys()
    {
        REDPLAYERPPT = myPlayer.NickName + "RedVal";
        GREENPLAYERPPT = myPlayer.NickName + "GreenVal";
        BLUEPLAYERPPT = myPlayer.NickName + "BlueVal";
    }
    private void SetColor()
    {
        r = Random.Range(0, 256);
        g = Random.Range(0, 256);
        b = Random.Range(0, 256);
        PlayerPrefs.SetInt(REDPLAYERPPT, r);
        PlayerPrefs.SetInt(GREENPLAYERPPT, g);
        PlayerPrefs.SetInt(BLUEPLAYERPPT, b);
        SetCustomsPPT(REDPLAYERPPT, r);
        SetCustomsPPT(GREENPLAYERPPT, g);
        SetCustomsPPT(BLUEPLAYERPPT, b);
        SetRoomProperty(REDPLAYERPPT, r);
        Debug.Log(REDPLAYERPPT + " is now " + myRoom.CustomProperties[REDPLAYERPPT]);
        SetRoomProperty(GREENPLAYERPPT, g);
        SetRoomProperty(BLUEPLAYERPPT, b);

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
            int r = (int)myRoom.CustomProperties[rPpt];
            int g = (int)myRoom.CustomProperties[gPpt];
            int b = (int)myRoom.CustomProperties[bPpt];
            nameText.overrideColorTags = true;
            nameText.color = new Color32((byte)r, (byte)g, (byte)b, 255);
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

    }
    private void SetCustomsPPT(string hashKeyIndex, int ind)
    {
        int playerIndex = ind;
        if (GameManager._myCustomProperty[hashKeyIndex] != null)
        {
            GameManager._myCustomProperty[hashKeyIndex] = playerIndex;
        }
        else
        {
            GameManager._myCustomProperty.Add(hashKeyIndex, playerIndex);
        }
        PhotonNetwork.LocalPlayer.CustomProperties = GameManager._myCustomProperty;
        PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
        //Debug.Log($"Setting {myPlayer.NickName} {hashKeyIndex} to {ind} : {PhotonNetwork.LocalPlayer.CustomProperties[hashKeyIndex]}");
    }
}
