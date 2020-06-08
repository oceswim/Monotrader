using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class CharSelectionScript : MonoBehaviour
{
    public GameObject[] prefabs;
    private int index;
    public TMP_Text ObjectName;
    public GameObject CharSelectionObject,mainCamera,charNotAvailable,playerUI;
    public static bool confirmation;

    private ExitGames.Client.Photon.Hashtable _myCustomProperty = new ExitGames.Client.Photon.Hashtable();
    private const string hashKeyIndex = "CharSelected";
    // Update is called once per frame

    private void Start()
    {

        confirmation = false;
        index = 0;
        UpdatePrefs(index);
        ShowCorrespondingPrefab(index);
    }
 

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

    private void ShowCorrespondingPrefab(int ind)
    {
        prefabs[ind].SetActive(true);
        ObjectName.text = prefabs[ind].name;


    }
    private void HideCorrespondingPrefab(int ind)
    {
        prefabs[ind].SetActive(false);

    }

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
    private void UpdatePrefs(int ind)
    {
        PlayerPrefs.SetInt("CharIndex", ind);
    }

    public void Confirm()
    {
        //confirmation = true;
        int theIndex = PlayerPrefs.GetInt("CharIndex");
        SetCustomPpties(theIndex);
       if (CheckSelectedAvailability())
        {
            PlayerSpawner.spawn = true;
            //hide charselection menu
            CharSelectionObject.SetActive(false);
            //activates main camera
            mainCamera.SetActive(true);
            playerUI.SetActive(true);
            GameManager.instance.OnConfirmation();
        }


    }
    private void SetCustomPpties(int ind)
    {
        //sets the local value of the selected object and sync it with other player
        int playerIndex = ind;
        string playerIndexString = playerIndex.ToString();
        _myCustomProperty[hashKeyIndex] = playerIndexString;
        PhotonNetwork.LocalPlayer.CustomProperties = _myCustomProperty;
        PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);

    }
  
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
    private void CharNotAvailable()
    {
        Debug.Log("Not available");
        charNotAvailable.SetActive(true);
    }
   
}
