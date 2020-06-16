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



    public void CurrenciesTrading(string mode)
    {
        currenciesObject.SetActive(true);
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
        taxesObject.SetActive(true);
        TaxesManager.BeginProcess = true;
    }
    public void CrisisManage()
    {
        crisisObject.SetActive(true);
        CrisisManager.BeginProcess = true;
    }
    public void BlackTuesday()
    {
        blackTuesdayObject.SetActive(true);
        BlackTuesdayManager.BeginProcess = true;
    }
    public void NationalVariation()
    {
        nationalobject.SetActive(true);
        VariationManager.BeginProcess = true;
        VariationManager.nationalVar = true;

    }
    public void WorldWideVariation()
    {
        worldWideObject.SetActive(true);
        VariationManager.BeginProcess = true;
        VariationManager.worldVar = true;
    }
    public void Savings()
    {
        SavingsManager.BeginProcess = true;
        savingsObject.SetActive(true);
    }
    public void Luck()
    {
        SpinTheWheel.BeginProcess = true;
        luckObject.SetActive(true);
    }
    
}
