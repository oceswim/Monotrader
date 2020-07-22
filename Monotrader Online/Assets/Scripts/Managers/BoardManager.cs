using Photon.Pun;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private static bool actionDone,newTurn,reset;
    public GameObject taxeSubPanel, taxesPanel;

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
                GameManager.instance.TurnManager();
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
        //FakeFuntion();
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
}
