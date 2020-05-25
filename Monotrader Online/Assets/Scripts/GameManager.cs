using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPun
{
    public static GameManager instance = null;
    public GameObject Dice;
    private List<ShootADie> inGameDices = new List<ShootADie>();
    // Start is called before the first frame update

    private void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnConfirmButton()
    {
        if (photonView.IsMine)
        {
            int xVal = 0;
            for (int i = 0; i < 2; i++)
            {
                GameObject prefab = PhotonNetwork.Instantiate(Dice.name, new Vector3(xVal, 2, 0), Quaternion.identity);
                xVal += 3;
                prefab.name = "Dice" + (i + 1).ToString();
                prefab.SetActive(false);
            }
        }
    }

    public void RollDices()
    {
        foreach (ShootADie s in inGameDices)
        {
            s.roll = true;
        }
    }
    public void AddDiceInstance(ShootADie s)
    {
        inGameDices.Add(s);
    }
}
