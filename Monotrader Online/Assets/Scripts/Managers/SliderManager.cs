using UnityEngine;
using UnityEngine.UI;
public class SliderManager : MonoBehaviour
{
    private Slider mySlider;
    public GameObject OnText, OffText, modeToggler,playerUI,trendsHistory;
    // Start is called before the first frame update
    void Start()
    {
        mySlider = transform.GetComponent<Slider>();
    }

    // Update is called once per frame
    public void ChangeSlider()
    {
        if(mySlider.value == mySlider.maxValue)
        {
            mySlider.value = mySlider.minValue;
            OffText.SetActive(true);
            OnText.SetActive(false);
            modeToggler.SetActive(false);
            playerUI.SetActive(false);
            trendsHistory.SetActive(true);
        }
        else
        {
            mySlider.value = mySlider.maxValue;
            OffText.SetActive(false);
            OnText.SetActive(true);
            modeToggler.SetActive(true);
            playerUI.SetActive(true);
            trendsHistory.SetActive(false);

        }
    }
}
