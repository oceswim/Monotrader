using UnityEngine;

public class MechanicsManager : MonoBehaviour
{
    public static MechanicsManager instance = null;


    public GameObject currenciesObject, taxesObject, crisisObject, nationalobject, worldWideObject, luckObject, savingsObject, blackTuesdayObject;
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
    public void TaxesManager()
    {
        taxesObject.SetActive(true);
    }
    public void CrisisManager()
    {
        crisisObject.SetActive(true);
    }
    public void BlackTuesday()
    {
        blackTuesdayObject.SetActive(true);
    }
    public void NationalVariation()
    {
        nationalobject.SetActive(true);
        VariationManager.nationalVar = true;
    }
    public void WorldWideVariation()
    {
        worldWideObject.SetActive(true);
        VariationManager.worldVar = true;
    }
    public void Savings()
    {
        savingsObject.SetActive(true);
    }
    public void Luck()
    {
        luckObject.SetActive(true);
    }
    
}
