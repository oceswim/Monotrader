using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private static bool actionDone, newTurn;
    // Start is called before the first frame update
    void Start()
    {
        actionDone = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(actionDone)
        {
            actionDone = false;
            GameManager.instance.SwitchTurn();
            if(newTurn)
            {
                newTurn = false;
                Debug.Log("CHECKING NEW TURN MANAGER HERE");
                GameManager.instance.TurnManager();
            }
        }
        
    }

    public static void SetPosition(int index)
    {
        Debug.Log("You are on " + index);
        PositionManager(index);
        actionDone = true;
    }
    public static void SetPositionNewTurn(int index)
    {
        Debug.Log("New turn! you are on "+index);
        PositionManager(index);
        actionDone = true;
        newTurn = true;
    }

    private static void PositionManager(int index)
    {
        if (index == 1 || index == 11 || index == 16 || index == 22)
        {
            MechanicsManager.instance.CurrenciesTrading("euros");
            //
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
            MechanicsManager.instance.CrisisManager();
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
            MechanicsManager.instance.TaxesManager();
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
