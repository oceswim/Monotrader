using UnityEngine;
using TMPro;
using Photon.Pun;


public class PlayerNameTag : MonoBehaviourPun
{
    // Start is called before the first frame update
    [SerializeField] private TextMeshProUGUI nameText;
    void Start()
    {
        if (photonView.IsMine) { return; }
        SetName();
    }

    private void SetName()
    {
        nameText.text = photonView.Owner.NickName;
        Debug.Log("Setting my name to " + nameText.text);

    }



}
