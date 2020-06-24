using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public GameObject bankings, history, trends;
    // Start is called before the first frame update
    public void HandleUI(GameObject UI)
    {
        switch(UI.name)
        {
            case "Trends":
                if (bankings.activeSelf)
                {
                    bankings.SetActive(false);
                }
                if (history.activeSelf)
                {
                    history.SetActive(false);
                }
                break;
            case "Bankings":
                if (trends.activeSelf)
                {
                    trends.SetActive(false);
                }
                if (history.activeSelf)
                {
                    history.SetActive(false);
                }
                break;
            case "History":
                if (bankings.activeSelf)
                {
                    bankings.SetActive(false);
                }
                if (trends.activeSelf)
                {
                    trends.SetActive(false);
                }
                break;

        }
        if(UI.activeSelf)
        {
            UI.SetActive(false);
        }
        else
        {
            UI.SetActive(true);
        }

    }
}
