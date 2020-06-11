using Photon.Pun;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour
{
    
    [SerializeField] private TMP_InputField nameInputField = null;
    [SerializeField] private Button continueButton = null;

    private const string playerNameKey = "PlayerName";

    void Start()
    {
        if (string.IsNullOrEmpty(nameInputField.text))
        {
            continueButton.interactable = false;
        }
        SetUpInputField();
    }

    //if a name already exists for this player the input field is filled by the name
    private void SetUpInputField()
    {
        if (!PlayerPrefs.HasKey(playerNameKey)) { return; }
        string defaultName = PlayerPrefs.GetString(playerNameKey);
        nameInputField.text = defaultName;

        SetPlayerName(defaultName);
    }

    //sets the name of the player based on the input
    public void SetPlayerName(string name)
    {
        continueButton.interactable = !string.IsNullOrEmpty(name);
    }

    //saves the player name and updates the nickname value of PUN player
    public void SaveName()
    {
        string playerName = nameInputField.text;
        PhotonNetwork.NickName = playerName;
        PlayerPrefs.SetString(playerNameKey, playerName);
    }
}
