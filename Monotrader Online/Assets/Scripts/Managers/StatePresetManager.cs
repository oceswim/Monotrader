using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;


/*
 * Allows to set each player's state in order to have the money manager be completely synced among players
 * when every player has a state property, money manager is enabled as well as the confirm and random button of the character 
 * selection UI
 */
public class StatePresetManager : MonoBehaviour
{
    private const string PLAYER_STATE = "My_State";
    private bool notYet,once;
    public MoneyManager myMoneyManager;
    public Button ConfirmButton, RandomButton;
    // Start is called before the first frame update
    void Start()
    {
        notYet = true;
        once = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(notYet)
        {
            SetCustomPpties(0);
            notYet = false;
        }
        if(!notYet)
        {
           foreach(Player p in PhotonNetwork.PlayerListOthers)
            {
                if(p.CustomProperties[PLAYER_STATE]==null)
                {
                    once = true;
                }
          
                
            }
            if (!once)
            {
                myMoneyManager.enabled = true;
                ConfirmButton.interactable = true;
                RandomButton.interactable = true;
                this.enabled = false;
            }
            else
            {
                once = false;
            }
        }
    }

    private void SetCustomPpties(int ind)
    {
        //sets the local value of the selected object and sync it with other player
        int playerIndex = ind;
        if (GameManager._myCustomProperty[PLAYER_STATE] != null)
        {
            GameManager._myCustomProperty[PLAYER_STATE] = playerIndex;
        }
        else
        {
            GameManager._myCustomProperty.Add(PLAYER_STATE, playerIndex);
        }
        PhotonNetwork.LocalPlayer.CustomProperties = GameManager._myCustomProperty;
        PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);

    }
}
