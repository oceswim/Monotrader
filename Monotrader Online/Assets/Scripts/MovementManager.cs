using Photon.Pun;
using UnityEngine;
public class MovementManager : MonoBehaviourPun
{
    private const string PREFDICE1 = "Dice1Val";
    private const string PREFdICE2 = "Dice2Val";
    private const int TARGET_LIST_SIZE = 27;
    private const int CORNER_1 = 7;
    private const int CORNER_2 = 14;
    private const int CORNER_3 = 21;
    private const int CORNER_4 = 0;
    private const int MOVE_STEP = 1;
    private const string PLAYER_NAME_PREF_KEY = "myName";
    private const string POSITION_INDEX_PREF_KEY = "myPositionIndex";
    public static bool moveMe;
    private Transform[] Targets;
    private Transform transformToMove;
    private int myActorNum,myPositionIndex;

    private void Start()
    {
        Targets = new Transform[27];
        PlayerPrefs.SetInt(POSITION_INDEX_PREF_KEY, 0);
        myPositionIndex = PlayerPrefs.GetInt(POSITION_INDEX_PREF_KEY);
       string myName = PlayerPrefs.GetString(PLAYER_NAME_PREF_KEY);
       myActorNum = PhotonNetwork.LocalPlayer.ActorNumber;
       string spawnPath = "StartP" + myActorNum.ToString();
       string fullPath = "BoardGame/Spawners/" + spawnPath + "/" + myName;
       transformToMove = GameObject.Find(fullPath).transform;
       moveMe = false;
       string pathToTargets = "BoardGame/Player"+myActorNum.ToString()+"Spots";
       InitialiseTargetList(pathToTargets);
    }
    private void InitialiseTargetList(string path)
    {
        Transform parentTarget = GameObject.Find(path).transform;
        Debug.Log("Parent target = " + parentTarget.name);
        for(int i = 0; i< Targets.Length;i++)
        {
            Targets[i] = parentTarget.GetChild(i);
            
        }
    }
    private void Update()
    {
        if(moveMe)
        {
            moveMe = false;
            int dice1Val = PlayerPrefs.GetInt(PREFDICE1);
            int dice2Val = PlayerPrefs.GetInt(PREFdICE2);
            int movementVal = dice1Val + dice2Val;
            Movement(movementVal);
        }
    }

    private void Movement(int value)
    {
        int oldPosition = myPositionIndex;
        int newPositionIndex = oldPosition + value;

        if (newPositionIndex > TARGET_LIST_SIZE)
        {
            newPositionIndex = 0;
        }
        Transform finalTarget = Targets[newPositionIndex];
        if(oldPosition<7 && newPositionIndex >=8)
        {
            OverlapMovement(Targets[oldPosition], Targets[CORNER_1], finalTarget);
            Debug.Log("1 -Starting from less than 7 moving to more than 8: +" + newPositionIndex);
        }
        else if(oldPosition < 14 && newPositionIndex >= 15)
        {
            OverlapMovement(Targets[oldPosition], Targets[CORNER_2], finalTarget);
            Debug.Log("2 -Starting from less than 14 moving to more than 14: +" + newPositionIndex);
        }
        else if(oldPosition < 21 && newPositionIndex >= 22)
        {
            OverlapMovement(Targets[oldPosition], Targets[CORNER_3], finalTarget);
            Debug.Log("3 -Starting from less than 21 moving to more than 21: +" + newPositionIndex);
        }
        else if(oldPosition < 28 && newPositionIndex >= 0)
        {
            OverlapMovement(Targets[oldPosition], Targets[CORNER_4], finalTarget);
            Debug.Log("4 -Starting from less than 27 moving to more than 0: +" + newPositionIndex);
            //if zero : new turn!

        }
        else if(oldPosition<7 && newPositionIndex>14)
        {
            OverlapMovement(Targets[oldPosition], Targets[CORNER_1], Targets[CORNER_2], finalTarget);
            Debug.Log("5 -Starting from less than seven moving to more than 14: +" + newPositionIndex);

        }
        else if(oldPosition < 14 && newPositionIndex > 21)
        {
            OverlapMovement(Targets[oldPosition], Targets[CORNER_2], Targets[CORNER_3], finalTarget);
            Debug.Log("6 -Starting from less than 14 moving to more than 21: +" + newPositionIndex);
        }
        else if(oldPosition < 21 && newPositionIndex >= 0)
        {
            OverlapMovement(Targets[oldPosition], Targets[CORNER_3], Targets[CORNER_4], finalTarget);
            Debug.Log("7 -Starting from less than 21 moving to more than 0: +" + newPositionIndex);
            //new turn!
        }
        else if(oldPosition < 28 && newPositionIndex > 7)
        {
            OverlapMovement(Targets[oldPosition], Targets[CORNER_4], Targets[CORNER_1], finalTarget); 
            Debug.Log("8 -Starting from less than 28 moving to more than 7: +" + newPositionIndex);
        }
        else if((oldPosition>=0 && newPositionIndex<=7)||(oldPosition >=7 && newPositionIndex <= 14) ||(oldPosition >=14 && newPositionIndex <= 21) ||(oldPosition >=21  && newPositionIndex <= 27))
        {
            Movement(Targets[oldPosition], Targets[newPositionIndex]);
            Debug.Log("9 -Moving down one line: +" + newPositionIndex);
        }
        else if((oldPosition >= 21 && newPositionIndex == 0))
        {
            //new turn logic
        }
    }
    /**
     * set halfway target as target
     * player moves to it
     * then switch target to end target
     * then look at end target 
     * then finish moving
     */
    private void OverlapMovement(Transform startTarget,Transform halfWayTarget,Transform endTarget)
    {

        transformToMove.position = Vector3.MoveTowards(startTarget.position, halfWayTarget.position, MOVE_STEP);
        transformToMove.LookAt(endTarget);
        transformToMove.position = Vector3.MoveTowards(halfWayTarget.position, endTarget.position, MOVE_STEP);
    }
    /**
    * set halfway target 1 as target
    * player moves to it
    * then switch target to halfway target 2
    * looks at halfway target 2
    * player moves to it
    * then switch target to end target
    * then look at end target 
    * then finish moving
    */
    private void OverlapMovement(Transform startTarget, Transform halfWayTarget1, Transform halfWayTarget2, Transform endTarget)
    {
        transformToMove.position = Vector3.MoveTowards(startTarget.position, halfWayTarget1.position, MOVE_STEP);
        transformToMove.LookAt(halfWayTarget2);
        transformToMove.position = Vector3.MoveTowards(halfWayTarget1.position, halfWayTarget2.position, MOVE_STEP);
        transform.LookAt(endTarget);
        transformToMove.position = Vector3.MoveTowards(halfWayTarget2.position, endTarget.position, MOVE_STEP);
    }
    /**
    * set target to end target
    * then look at end target 
    * then moves to end target
    */
    private void Movement(Transform startTarget,  Transform endTarget)
    {
        transformToMove.position = Vector3.MoveTowards(startTarget.position, endTarget.position, MOVE_STEP);
    }
}
