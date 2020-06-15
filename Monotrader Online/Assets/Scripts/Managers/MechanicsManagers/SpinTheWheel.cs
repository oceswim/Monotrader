using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinTheWheel : MonoBehaviour
{
    private const string MALUS = "myMalus";
    public GameObject theWheel,confirmButton;
    private bool spin;
    private float rotSpeed;
    // Start is called before the first frame update
    void Start()
    {
        spin = false;
        rotSpeed = 0;
        theWheel.transform.eulerAngles = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(spin)
        {
            if(rotSpeed<=.5f)
            {
                spin = false;
                float finalRot =theWheel.transform.localEulerAngles.z;
                DeterminePerk(finalRot);
            }
            theWheel.transform.Rotate(0, 0, rotSpeed);
            Spinning();
        }
    }

    public void Spin()
    {        
        spin = true;
        rotSpeed = UnityEngine.Random.Range(50, 76);
    }

    private void Spinning()
    {
        float x = 1;
        while(x>.1f)
        {
            x = x - Time.deltaTime;

        }
        rotSpeed -= .5f;
    }

    private void DeterminePerk(float rotation)
    {
        Debug.Log("final rot:" + rotation);
        double value = Math.Round(rotation, 1);

        if(value>=0 && value<=96)
        {
            Debug.Log("+500");
        }
        else if(value >= 96.1 && value <=190)
        {
            Debug.Log("-250");
        }
        else if(value >= 274.1 && value <= 360)
        {
            Debug.Log("Malus applied.");
            //use player pref here.
            PlayerPrefs.SetInt(MALUS, 1);
        }
        else if(value >= 190.1 && value <= 274)
        {
            Debug.Log("+500$");
            int randomInt = UnityEngine.Random.Range(0, 4);
            switch(randomInt)
            {
                case 0:
                    //dollars
                    break;
                case 1:
                    //euros
                    break;
                case 2:
                    //pounds
                    break;
                case 3:
                    //yen
                    break;

            }
        }

        confirmButton.SetActive(true);
    }
}
