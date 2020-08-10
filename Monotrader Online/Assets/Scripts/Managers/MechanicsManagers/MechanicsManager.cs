using UnityEngine;
using TMPro;
public class MechanicsManager : MonoBehaviour
{
    public static MechanicsManager instance = null;


    public GameObject currenciesObject, taxesObject, crisisObject, nationalobject, worldWideObject, luckObject, savingsObject, blackTuesdayObject;
    public TMP_Text savingsText;
    // Start is called before the first frame update

    private void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

    }
    void Start()
    {
        savingsText.text = "0 G";
    }

    public void DeactivateMechanic(string mechanic)
    {
        switch(mechanic)
        {
            case "currency":
                currenciesObject.SetActive(false);
                break;
            case "taxes":
                taxesObject.SetActive(false);
                break;
            case "crisis":
                crisisObject.SetActive(false);
                break;
            case "national":
                nationalobject.SetActive(false);
                break;
            case "worldwide":
                worldWideObject.SetActive(false);
                break;
            case "luck":
                luckObject.SetActive(false);
                break;
            case "savings":
                savingsObject.SetActive(false);
                break;
            case "tuesday":
                blackTuesdayObject.SetActive(false);
                break;
        }
        GameManager.actionInPlace = "null";
    }

    public void CurrenciesTrading(string mode)
    {
        GameManager.actionInPlace = "currency";
        currenciesObject.SetActive(true);
        TradingManager.BeginProcess = true;
        switch (mode)
        {
            case "euros":
                TradingManager.eurosTrading = true;
                break;
            case "dollars":
                TradingManager.dollarsTrading = true;
                break;
            case "pounds":
                TradingManager.poundsTrading = true;
                break;
            case "yens":
                TradingManager.yensTrading = true;
                break;
        }

    }
    public void TaxesManage()
    {
        GameManager.actionInPlace = "taxes";
        taxesObject.SetActive(true);
        TaxesManager.BeginProcess = true;
    }
    public void CrisisManage()
    {
        GameManager.actionInPlace = "crisis";
        crisisObject.SetActive(true);
        CrisisManager.BeginProcess = true;
    }
    public void BlackTuesday()
    {
        GameManager.actionInPlace = "tuesday";
        blackTuesdayObject.SetActive(true);
        BlackTuesdayManager.BeginProcess = true;
    }
    public void NationalVariation()
    {
        GameManager.actionInPlace = "national";
        nationalobject.SetActive(true);
        VariationManager.BeginProcess = true;
        VariationManager.nationalVar = true;

    }
    public void WorldWideVariation()
    {
        GameManager.actionInPlace = "worldwide";
        worldWideObject.SetActive(true);
        VariationManager.BeginProcess = true;
        VariationManager.worldVar = true;
    }
    public void Savings()
    {
        GameManager.actionInPlace = "savings";
        SavingsManager.BeginProcess = true;
        savingsObject.SetActive(true);
    }
    public void Luck()
    {
        GameManager.actionInPlace = "luck";
        SpinTheWheel.BeginProcess = true;
        luckObject.SetActive(true);
    }
    
}
