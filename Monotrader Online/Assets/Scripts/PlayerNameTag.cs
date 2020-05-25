using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using System;

public class PlayerScript : MonoBehaviourPun
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
    }

  
    
}
