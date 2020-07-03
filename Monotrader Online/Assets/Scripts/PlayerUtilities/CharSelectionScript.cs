using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class CharSelectionScript : MonoBehaviour
{
    public GameObject[] prefabs;
    private int index;
    public TMP_Text ObjectName;
    public GameObject CharSelectionObject,mainCamera,charNotAvailable,playerUI;
    public static bool confirmation;


    private const string hashKeyIndex = "CharSelected";
    // Update is called once per frame

    private void Start()
    {

        confirmation = false;
        index = 0;
        UpdatePrefs(index);
        SetCustomPpties(-1);
        ShowCorrespondingPrefab(index);
    }
 

    //when player clicks on left button
    public void leftButton()
    {
        HideCorrespondingPrefab(index);
        Debug.Log("Index before left:" + index);
        if (index == 0)
        {
            index = 3;
        }
        else
        {
            index--;
        }
        Debug.Log("Index after left:" + index);
        ShowCorrespondingPrefab(index);
        UpdatePrefs(index);
    }

    //when player clicks on right button
    public void rightButton()
    {
        HideCorrespondingPrefab(index);
        Debug.Log("Index before right:" + index);
        if (index == 3)
        {
            index = 0;
        }
        else
        {
            index++;
        }
        Debug.Log("Index after right:" + index);
        ShowCorrespondingPrefab(index);
        UpdatePrefs(index);
    }

    //shows the corresponding prefab based on the index
    private void ShowCorrespondingPrefab(int ind)
    {
        prefabs[ind].SetActive(true);
        ObjectName.text = prefabs[ind].name;


    }

    //deactivates the corresponding prefab when index changes
    private void HideCorrespondingPrefab(int ind)
    {
        prefabs[ind].SetActive(false);

    }


    //function choosing a random character.
    public void RandomChar()
    {
        HideCorrespondingPrefab(index);
        int randomInt = Random.Range(0, prefabs.Length);
        Debug.Log(randomInt);
        if( randomInt == index)
        {
           while(randomInt == index)
            {
                randomInt = Random.Range(0, prefabs.Length);
                Debug.Log(randomInt);
            }

        }
        index = randomInt;
        ShowCorrespondingPrefab(randomInt);
        UpdatePrefs(index);
    }

    //updates the player prefs when choice changes
    private void UpdatePrefs(int ind)
    {
        PlayerPrefs.SetInt("CharIndex", ind);
    }


    //when player confirms his choice, it is first checked that no other player chose that same character.
    public void Confirm()
    {
        //confirmation = true;
        int theIndex = PlayerPrefs.GetInt("CharIndex");
        SetCustomPpties(theIndex);
        if (CheckSelectedAvailability())
        {
            PrefabSpawner.spawn = true;
            //hide charselection menu
            CharSelectionObject.SetActive(false);
            //activates main camera
            mainCamera.SetActive(true);
            playerUI.SetActive(true);
            GameManager.instance.OnConfirmation();

        }


    }

    //Sets the player custom ppt to the int so that it's synced with other players.
    private void SetCustomPpties(int ind)
    {
        //sets the local value of the selected object and sync it with other player
        int playerIndex = ind;
        string playerIndexString = playerIndex.ToString();
        GameManager._myCustomProperty[hashKeyIndex] = playerIndexString;
        PhotonNetwork.LocalPlayer.CustomProperties = GameManager._myCustomProperty;
        PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);

    }
  

    //checks the availablity of a prefab based on player's custom properties
    private bool CheckSelectedAvailability()
    {
        string localProperty = PhotonNetwork.LocalPlayer.CustomProperties[hashKeyIndex].ToString();
        bool isAvailable = true;
        foreach (Player p in PhotonNetwork.PlayerListOthers)
        {
            if (p.CustomProperties[hashKeyIndex] != null)
            {
                if (p.CustomProperties[hashKeyIndex].Equals(localProperty))
                {
                    Debug.Log(p.NickName+ " has already "+  p.CustomProperties[hashKeyIndex]);
                    isAvailable = false;
                }
            }
        }
        if(!isAvailable)
        {

            CharNotAvailable();
           
        }
        return isAvailable;
    }

    //if not available, a panel is shown.
    private void CharNotAvailable()
    {
        Debug.Log("Not available");
        charNotAvailable.SetActive(true);
    }
   
}
