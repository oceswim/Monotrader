using UnityEngine;
using TMPro;
using Photon.Pun;


public class Test : MonoBehaviourPun
{
    public TMP_Text myText;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        
            
            if (Input.GetKey(KeyCode.Mouse0))
            {
            Debug.Log("Here" + PhotonNetwork.LocalPlayer.NickName);
            if (!photonView.IsMine)
                {
                    photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
                    Debug.Log("here" + PhotonNetwork.LocalPlayer.NickName);
                }
                string content = Time.deltaTime.ToString();
               photonView.RPC("Jump",RpcTarget.AllBuffered,content);
            }
        

    }
    [PunRPC]
    void Jump(string content)
    {
        myText.text = content;    
    }
}
