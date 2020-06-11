using UnityEngine;
using TMPro;
using Photon.Pun;



/*
 * Allows to show the other player names and hide the name if it's our name.
 */
public class PlayerNameTag : MonoBehaviourPun
{
    [SerializeField] private TextMeshProUGUI nameText;
    void Start()
    {
        if (photonView.IsMine) { return; }
        SetName();
    }


    //sets the text of the billboard to the corresponding player name
    private void SetName()
    {
        nameText.text = photonView.Owner.NickName;
        Debug.Log("Setting my name to " + nameText.text);

    }



}
