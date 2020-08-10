using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
public class PrefabSpawner : MonoBehaviourPunCallBacks
{
 
    [SerializeField] private GameObject playerPrefab;
    public Transform[] Spawners;
    private Transform Spawn;
    private int[] otherNumbers;
    public GameObject[] inGamePrefabs = new GameObject[4];
    public static bool spawn;
    public MovementManager theMovementManager;
    void Start()
    {
        spawn = false;
        int otherPlayers = PhotonNetwork.PlayerList.Length - 1;
        otherNumbers = new int[otherPlayers];
        for(int i = 0; i< PhotonNetwork.PlayerListOthers.Length;i++)
        {
            otherNumbers[i] = PhotonNetwork.PlayerListOthers[i].ActorNumber;
        }
        int spawnNumber;
        for (int i = 0; i< PhotonNetwork.PlayerList.Length;i++)
        { 
            spawnNumber = PhotonNetwork.PlayerList[i].ActorNumber;
            if (!otherNumbers.Contains(spawnNumber))
            {
                
                SpawnMyPlayer(spawnNumber - 1);
            }
        }
         
    }
    private void Update()
    {
        if(spawn)
        {
            spawn = false;
            SpawnModel();
        }
    }

    //for each players present in the room one instance of the player prefab spawns
    private void SpawnMyPlayer(int i)
    {
        Spawn = Spawners[i];
        GameObject myPlayer = PhotonNetwork.Instantiate(playerPrefab.name, Spawn.position, Quaternion.identity, 0);
        myPlayer.name = "Player"+(i+1).ToString();
        myPlayer.transform.SetParent(Spawn);
        theMovementManager.enabled = true;
    }

    //depending on the index chosen the corresponding model is spawned
    private void SpawnModel()
    {
        int actorNum = PhotonNetwork.LocalPlayer.ActorNumber;
        string spawnPath = "StartP" + actorNum.ToString();
        Transform spawnTarget = GameObject.Find(spawnPath).transform;
        int ind = PlayerPrefs.GetInt("CharIndex");
        string prefabName = inGamePrefabs[ind].name;
        Vector3 prefabPos = spawnTarget.position;
        GameObject prefab = PhotonNetwork.Instantiate(prefabName,prefabPos,Quaternion.identity);
        prefab.name = inGamePrefabs[ind].name;
        prefab.transform.SetParent(spawnTarget.GetChild(0));
        AdjustPrefab(prefab);
        
    }

    //depending on the prefab name some adjustments are needed
    private void AdjustPrefab(GameObject prefb)
    {
        string prefbName = prefb.name;
        switch(prefbName)
        {
            case "CoinsStack":
                prefb.transform.localPosition = new Vector3(0, -.25f, 0);
                //prefb.transform.localScale = new Vector3(150, 150, 150);
                break;
            case "Chest":
                prefb.transform.Rotate(new Vector3(0, 180, 0));
                //prefb.transform.localScale = new Vector3(2, 2, 2);
                break;
            case "GoldIngot":
                prefb.transform.localPosition = new Vector3(0, -.25f, 0);
                break;
            case "PileBill":
                prefb.transform.localPosition = new Vector3(0, .25f, 0);
                prefb.transform.Rotate(new Vector3(-90, 0, 0));
                //prefb.transform.localScale = new Vector3(40, 40, 40);
                break;
        }

    }
}


