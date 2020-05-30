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

    // Start is called before the first frame update
    void Start()
    {
        if (string.IsNullOrEmpty(nameInputField.text))
        {
            continueButton.interactable = false;
        }
        SetUpInputField();
    }

    private void SetUpInputField()
    {
        if (!PlayerPrefs.HasKey(playerNameKey)) { return; }
        string defaultName = PlayerPrefs.GetString(playerNameKey);
        nameInputField.text = defaultName;

        SetPlayerName(defaultName);
    }

    public void SetPlayerName(string name)
    {
        continueButton.interactable = !string.IsNullOrEmpty(name);
    }

    public void SaveName()
    {
        string playerName = nameInputField.text;
        PhotonNetwork.NickName = playerName;
        PlayerPrefs.SetString(playerNameKey, playerName);
    }
}
