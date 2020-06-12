using UnityEngine;
using TMPro;

//single player
public class TaxesManager : MonoBehaviour
{

    //fortune 0 - 10,000 = 5% = 1% each monney gets decreased.
    //fortune 10,001 - 20,000 = 10% = 2% each money gets decreased.
    //fortune 20,001 - 8 = 15% = 3% each money gets decreased.
    //if no more of one currency take on the gold.
    private float playerE, playerD, playerP, playerY, playerG, playerFortune;
    public TMP_Text content;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
