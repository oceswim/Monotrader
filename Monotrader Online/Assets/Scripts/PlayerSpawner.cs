using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerSpawner : MonoBehaviourPun
{
    [SerializeField] private GameObject[] playerPrefab = new GameObject[4];
    public Transform[] Spawners;
    private Transform Spawn;
    private int[] otherNumbers;
    void Start()
    {
        int otherPlayers = PhotonNetwork.PlayerList.Length - 1;
        otherNumbers = new int[otherPlayers];
        int spawnNumber = 0;
        for(int i = 0; i< PhotonNetwork.PlayerListOthers.Length;i++)
        {
            otherNumbers[i] = PhotonNetwork.PlayerListOthers[i].ActorNumber;
        }
        for(int i = 0; i< PhotonNetwork.PlayerList.Length;i++)
        { 
            spawnNumber = PhotonNetwork.PlayerList[i].ActorNumber;
            if (!otherNumbers.Contains(spawnNumber))
            {
                Debug.Log(spawnNumber + " -> spawn num");
                SpawnMyPlayer(spawnNumber - 1);
            }
        }
        
       
       
        
    }
    private void SpawnMyPlayer(int i)
    {

        Spawn = Spawners[i];
        //GameObject myPlayer = PhotonNetwork.Instantiate(playerPrefab.name, Spawn.position, Quaternion.identity, 0);
        //myPlayer.name = (i+1).ToString();
        //myPlayer.transform.SetParent(Spawn);
        //Debug.Log(myPlayer.name + "spawning at :" + myPlayer.transform.parent.name);

    
        
    }

}


