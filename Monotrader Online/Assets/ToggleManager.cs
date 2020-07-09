using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleManager : MonoBehaviour
{

    public EventTrigger eventTrigger1, eventTrigger2;
    public Toggle toggle1, toggle2;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Update()
    {
        if(toggle1.isOn && eventTrigger1.enabled)
        {
            eventTrigger1.enabled = false;
            if(!eventTrigger2.enabled)
            {
                eventTrigger2.enabled = true;
            }
        }
        else if(toggle2.isOn && eventTrigger2.enabled)
        {
            eventTrigger2.enabled = false;
            if (!eventTrigger1.enabled)
            {
                eventTrigger1.enabled = true;
            }
        }
    }
    // Update is called once per frame
    public void ToggleValueChange()
    {     
        eventTrigger1.enabled = !toggle1.isOn;
        eventTrigger2.enabled = !toggle2.isOn;
        
    }
}
