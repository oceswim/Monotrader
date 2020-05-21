using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RotateDisplay : MonoBehaviour
{
    private Vector3 rotY = new Vector3(0, 1, 0);
    public GameObject[] prefabs;
    private int index;
    public TMP_Text ObjectName;
    // Update is called once per frame

    private void Start()
    {
        index = 0;
        ShowCorrespondingPrefab(index);
    }
    void Update()
    {

     transform.Rotate(new Vector3(0, .5f, 0), Space.Self);
    }

    public void leftButton()
    {
        HideCorrespondingPrefab(index);
        Debug.Log("Index before left:" + index);
        if (index == 0 )
        {
            index = 3;
        }
        else
        {
            index--;
        }
        Debug.Log("Index after left:" + index);
        ShowCorrespondingPrefab(index);
    }    
    public void rightButton()
    {
        HideCorrespondingPrefab(index);
        Debug.Log("Index before right:" + index);
        if (index == 3 )
        {
            index = 0;
        }
        else
        {
            index++;
        }
        Debug.Log("Index after right:" + index);
        ShowCorrespondingPrefab(index);
    }

    private void ShowCorrespondingPrefab(int ind)
    {
         prefabs[ind].SetActive(true);
         ObjectName.text = prefabs[ind].name;
           
        
    }
    private void HideCorrespondingPrefab(int ind)
    {
        prefabs[ind].SetActive(false);
   
    }

}
