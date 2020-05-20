using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviourPun
{
    [SerializeField] private float moveSpeed = 0f;
    private CharacterController controller = null;
   
    private void Start()
    {
        
        controller = GetComponent<CharacterController>();
       
    }
    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine)
        {
            TakeInput();
        }
    }

    private void TakeInput()
    {
        Vector3 movement = new Vector3
        {
            x = Input.GetAxisRaw("Horizontal"),
            y = 0f,
            z = Input.GetAxisRaw("Vertical")
        }.normalized;

        controller.SimpleMove(movement * moveSpeed);
    }
}
