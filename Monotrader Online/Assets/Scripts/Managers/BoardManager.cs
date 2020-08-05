using Photon.Pun;
using UnityEngine;

public class BoardManager : MonoBehaviourPunCallbacks
{
    private static bool actionDone,newTurn,reset;
    public GameObject taxeSubPanel, taxesPanel,disqualifiedPanel;
    private const int NEW_TURN_MONEY = 2000;
    private static int x = 0;
    void Start()
    {
        actionDone =reset= false;
    }

    // Update is called once per frame
    void Update()
    {
        if(actionDone)
        {
            actionDone = false;
            if (newTurn)
            {
                newTurn = false;
                if (!CheckForBankingsValues())
                {
                    GameManager.instance.TurnManager();
                }
                else
                {
                    disqualifiedPanel.SetActive(true);
                }
            }
            else
            {


                GameManager.instance.SwitchTurn();
            }
            
        }
        if(reset)
        {
            reset = false;
            
            taxesPanel.SetActive(false); 
            taxeSubPanel.SetActive(true);
        }

    }

    public static void SetPosition(int index,bool turn)
    {

        PositionManager(index);
 
 

       
        if (turn)
        {
          newTurn = turn;
        }
        
    }
    public void BackWardMechanic()
    {
        reset = true;
    }
    public static void NextTurn()
    {
        actionDone = true;
    }
    private static void PositionManager(int index)
    {
        if(index == 0)
        {
            actionDone = true;

        }
        else if (index == 1 || index == 11 || index == 16 || index == 22)
        {
            MechanicsManager.instance.CurrenciesTrading("euros");
            
            //euros currency -> can buy or sell
        }
        else if (index == 3 || index == 8 || index == 17 || index == 25)
        {
            MechanicsManager.instance.CurrenciesTrading("dollars");
            //dollars currency -> can buy or sell
            
        }
        else if (index == 6 || index == 10 || index == 19 || index == 23)
        {
            MechanicsManager.instance.CurrenciesTrading("pounds");
            //pounds currency -> can buy or sell
            
        }
        else if (index == 5 || index == 12 || index == 20 || index == 24)
        {
            MechanicsManager.instance.CurrenciesTrading("yens");
            //yen currency -> can buy or sell
            
        }
        else if (index == 2 || index == 9)
        {
            MechanicsManager.instance.CrisisManage();
            
            //crisis
        }
        else if (index == 13 || index == 26)
        {
            MechanicsManager.instance.WorldWideVariation();
            
            //world variation
        }
        else if (index == 4 || index == 15)
        {
            MechanicsManager.instance.NationalVariation();
            
            //national variation
        }
        else if (index == 18 || index == 27)
        {
            
            MechanicsManager.instance.TaxesManage();
           
            //taxes
        }
        else
        {
            switch (index)
            {
                case 7:
                    MechanicsManager.instance.BlackTuesday();
                   
                    //black tuesday
                    break;
                case 14:
                    MechanicsManager.instance.Savings();
                    
                    //bank
                    break;
        
                case 21:
                    MechanicsManager.instance.Luck();
                    
                    //luck?
                    break;

            }
        }
    }
    private static void FakeFunction()
    {
      
            MechanicsManager.instance.TaxesManage();
        
      
    }

    private bool CheckForBankingsValues()
    {
        bool returnValue = false;
        if((MoneyManager.PLAYER_GOLD + NEW_TURN_MONEY)<=0 ||
            MoneyManager.PLAYER_DOLLARS <=0 || 
            MoneyManager.PLAYER_EUROS <=0 || 
            MoneyManager.PLAYER_POUNDS <=0 || 
            MoneyManager.PLAYER_YENS <=0)
        {
            returnValue = true;
            
        }
        return returnValue;
    }

    public void ExitAfterDisqualified()
    {
        // we give the player's leftovers to the rest if player count left is more than 1
        if(PhotonNetwork.PlayerListOthers.Length>1)
        {
            float g=0, d=0, e=0, p=0, y = 0;
            int quotient = PhotonNetwork.PlayerListOthers.Length;
            if (MoneyManager.PLAYER_GOLD > 0)
            {
                g = MoneyManager.PLAYER_GOLD / quotient;
            }
            if (MoneyManager.PLAYER_EUROS > 0)
            {
                e = MoneyManager.PLAYER_EUROS / quotient;
            }
            if (MoneyManager.PLAYER_DOLLARS > 0)
            {
                d = MoneyManager.PLAYER_DOLLARS / quotient;
            }
            if (MoneyManager.PLAYER_POUNDS > 0)
            {
                p = MoneyManager.PLAYER_POUNDS / quotient;
            }
            if (MoneyManager.PLAYER_YENS>0)
            {
                y = MoneyManager.PLAYER_YENS / quotient;
            }

            MoneyManager.instance.PlayerDQMechanic(g, d, e, p, y);
            

        }
        else if(PhotonNetwork.PlayerListOthers.Length == 1)
        {
            //player left wins
            GameModeManager.disqualified = true;
        }
        else
        {
            Exit.exitAfterDQ = true;
        }
        
    }
}
