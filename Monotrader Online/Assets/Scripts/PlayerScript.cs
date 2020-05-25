using UnityEngine;
using TMPro;
using Photon.Pun;


public class PlayerNameTag : MonoBehaviourPun
{
    // Start is called before the first frame update
    [SerializeField] private TextMeshProUGUI nameText;
    
    private CharacterController controller = null;
    private float moveSpeed = 10f;
    void Start()
    {
        if (photonView.IsMine) { return; }
        controller = GetComponent<CharacterController>();
        SetName();

    }
    void Update()
    {
       
        TakeInput();
        
    }

    private void SetName()
    {
        nameText.text = photonView.Owner.NickName;
        nameText.color = new Color(255, 0, 0,255);
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
