using Photon.Pun;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour
{
    
    [SerializeField] private TMP_InputField nameInputField = null;
    [SerializeField] private Button continueButton = null;
    public EventTrigger buttonEventTrigger;
    private const string playerNameKey = "PlayerName";
    public AudioSource typingAudio,backAudio;
    private bool erase = false;
    void Start()
    {
        if (string.IsNullOrEmpty(nameInputField.text))
        {
            continueButton.interactable = false;
            buttonEventTrigger.enabled = false;
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
        if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKey(KeyCode.Backspace))
        {
            erase = true;
        }
        if (!erase)
        {
            typingAudio.Play();
        }
        else
        {
            backAudio.Play();
            erase = false;
        }
        continueButton.interactable = !string.IsNullOrEmpty(name);
        buttonEventTrigger.enabled = !string.IsNullOrEmpty(name); 
    }

    //saves the player name and updates the nickname value of PUN player
    public void SaveName()
    {

        string playerName = FirstLetterToUpper(nameInputField.text);
        PhotonNetwork.NickName = playerName;
        PlayerPrefs.SetString(playerNameKey, playerName);

    }

    public string FirstLetterToUpper(string str)
    {
        if (str == null)
            return null;

        if (str.Length > 1)
            return char.ToUpper(str[0]) + str.Substring(1);

        return str.ToUpper();
    }

}
