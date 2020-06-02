using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private static bool actionDone;
    // Start is called before the first frame update
    void Start()
    {
        actionDone = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(actionDone)
        {
            actionDone = false;
            GameManager.instance.SwitchTurn();
        }
        
    }

    public static void SetPosition(int index)
    {
        Debug.Log("You are on " + index);
        switch(index)    
        {
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
            case 8:
                break;
            case 9:
                break;
            case 10:
                break;
            case 11:
                break;
            case 12:
                break;
            case 13:
                break;
            case 14:
                break;
            case 15:
                break;
            case 16:
                break;
            case 17:
                break;
            case 18:
                break;
            case 19:
                break;
            case 20:
                break;
            case 21:
                break;
            case 22:
                break;
            case 23:
                break;
            case 24:
                break;
            case 25:
                break;
            case 26:
                break;
            case 27:
                break;

        }
        actionDone = true;
    }
    public static void SetPositionNewTurn(int index)
    {
        Debug.Log("New turn! you are on "+index);
        switch (index)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
            case 8:
                break;
            case 9:
                break;
            case 10:
                break;
            case 11:
                break;
            case 12:
                break;
            case 13:
                break;
            case 14:
                break;

        }
        actionDone = true;

    }
}
