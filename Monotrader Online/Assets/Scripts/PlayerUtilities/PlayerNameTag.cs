using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;

/*
 * Allows to show the other player names and hide the name if it's our name.
 */
public class PlayerNameTag : MonoBehaviourPun
{
    [SerializeField] private TextMeshProUGUI nameText;

    public const string REDPREF="myRedVal";
    public const string GREENPREF="myGreenVal";
    public const string BLUEPREF="myBlueVal";
    private int r, g, b;
    
    void Start()
    {
        SetColor();
        if (photonView.IsMine) 
        {
            return;
        }
        SetName();
        
    }

    private void SetColor()
    {
        r = Random.Range(0, 256);
        g = Random.Range(0, 256);
        b = Random.Range(0, 256);
        PlayerPrefs.SetInt(REDPREF, r);
        PlayerPrefs.SetInt(GREENPREF, g);
        PlayerPrefs.SetInt(BLUEPREF, b);
    }
    //sets the text of the billboard to the corresponding player name
    private void SetName()
    {
        nameText.text = photonView.Owner.NickName;
        nameText.overrideColorTags = true;
        Debug.Log(r + " " + g + " " + b);
        nameText.color = new Color32((byte)r, (byte)g, (byte)b, 255);
        Debug.Log("Setting my name to " + nameText.text);

    }



}
