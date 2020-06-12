using UnityEngine;

//all players
public class VariationManager : MonoBehaviour
{
    public static bool worldVar,nationalVar;
    // Start is called before the first frame update
    void Start()
    {
        worldVar=nationalVar = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(worldVar)
        {
            worldVar = false;
            WorldVariation();
        }
        if(nationalVar)
        {
            nationalVar = false;
            NationalVariation();
        
        }
    }
    private void WorldVariation()
    {
        switch(VariationMode())
        {
            case 0://less value
                break;
            case 1://more value
                break;
        }
    }
    private void NationalVariation()
    {

        switch (VariationMode())
        {
            case 0://less value
                break;
            case 1://more value
                break;
        }
    }

    private int VariationMode()
    {
        int rand = Random.Range(0, 2);
        return rand;
    }
}
