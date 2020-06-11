using Photon.Pun;
using System.Collections;
using UnityEngine;
public class MovementManager : MonoBehaviourPun
{
    //the different hashkeys and constant private variables
    private const string PREFDICE = "DiceVal";
    private const int TARGET_AMOUNT = 27;
    private const int TARGET_LIST_SIZE = 28;
    private const int CORNER_1 = 7;
    private const int CORNER_2 = 14;
    private const int CORNER_3 = 21;
    private const int CORNER_4 = 0;
    private const float SPEED = 5f;
    private const string PLAYER_NAME_PREF_KEY = "myName";
    private const string POSITION_INDEX_PREF_KEY = "myPositionIndex";
    private Transform[] Targets;
    private Transform transformToMove, endPoint;
    private int myActorNum, myPositionIndex, movementIndex;
    private bool step1Complete, step2Complete, doneMoving, newTurn;
    private Transform halfwayTarget1, halfwayTarget2;
    private CharacterController controller = null;

    //bool allowing to start the movement mechanics
    public static bool moveMe;
    
    private void Start()
    {
        PlayerPrefs.SetInt(POSITION_INDEX_PREF_KEY, 0);
        transformToMove = FindMyTransform();
        controller = transformToMove.GetComponent<CharacterController>();
        string pathToTargets = "BoardGame/Player" + myActorNum.ToString() + "Spots";
        Targets = new Transform[TARGET_LIST_SIZE];
        InitialiseTargetList(pathToTargets);
        moveMe = step1Complete = step2Complete = doneMoving = newTurn = false;
        movementIndex = 0;
    }

    private void Update()
    {
        //if moveme is on the player starts going to the target
        if (moveMe)
        {
            moveMe = false;
            int diceVal = PlayerPrefs.GetInt(PREFDICE);
            Movement(diceVal);
        }


    }

    //allows to instantiate the transform for our player 
    private Transform FindMyTransform()
    {
        Transform myTransf;
        myPositionIndex = PlayerPrefs.GetInt(POSITION_INDEX_PREF_KEY);
        string myName = PlayerPrefs.GetString(PLAYER_NAME_PREF_KEY);
        myActorNum = PhotonNetwork.LocalPlayer.ActorNumber;
        string spawnPath = "StartP" + myActorNum.ToString();
        string fullPath = "BoardGame/Spawners/" + spawnPath + "/" + myName;
        myTransf = GameObject.Find(fullPath).transform;
        //Debug.Log("HEY" + myTransf.name);
        return myTransf;
    }
   
    //the target list of the board
    private void InitialiseTargetList(string path)
    {
        Transform parentTarget = GameObject.Find(path).transform;
        for (int i = 0; i < Targets.Length; i++)
        {
            Targets[i] = parentTarget.GetChild(i);
        }
    }


    //the offset needed to calculate the overall displacement
    private Vector3 GetOffset(Transform p1, Transform p2)
    {
        Vector3 offset = p1.position - p2.position;
        return offset;

    }


    /*movement function that will determine if it is a
     * simple move : the player has to move in a straight line
     * simple overlap : the player needs to go to one corner then to its target
     * complex overlap : the player has to go to the first corner, then to a second and finally to the endpoint
     * if a new turn is detected, the newturn logic is activated
     */
    private void Movement(int value)
    {
        int oldPosition = myPositionIndex;
        int newPositionIndex = oldPosition + value;
        int overlap = -1;

        if (newPositionIndex > TARGET_AMOUNT)
        {
            int temp = newPositionIndex - TARGET_AMOUNT;
            overlap = temp - 1;

            newTurn = true;
        }
        Transform finalTarget;
        Transform newTurnTarget;
        if (overlap >= 0)
        {
            finalTarget = Targets[0];
            newTurnTarget = Targets[overlap];
        }
        else
        {
            finalTarget = Targets[newPositionIndex];
            newTurnTarget = null;
        }
        movementIndex = 0;
        int movementMode = 0;
        if (!newTurn)
        {
            if (oldPosition >= 0 && oldPosition < 7)
            {

                if (newPositionIndex <= 7)
                {
                    //straight line
                    movementIndex = 1;
                    movementMode = 1;
                }
                else if (newPositionIndex > 7 && newPositionIndex <= 14)
                {
                    //simple overlap
                    movementIndex = 2;
                    movementMode = 2;
                }
                else if (newPositionIndex > 14 && newPositionIndex <= 21)
                {
                    //double overlap
                    movementIndex = 3;
                    movementMode = 3;
                }
            }
            else if (oldPosition >= 7 && oldPosition < 14)
            {

                if (newPositionIndex <= 14)
                {
                    //straight line
                    movementIndex = 1;
                    movementMode = 1;
                }
                else if (newPositionIndex > 14 && newPositionIndex <= 21)
                {
                    //simple overlap
                    movementIndex = 4;
                    movementMode = 2;
                }
                else if (newPositionIndex > 21 && newPositionIndex <= 27)
                {
                    //double overlap
                    movementIndex = 5;
                    movementMode = 3;
                }

            }
            else if (oldPosition >= 14 && oldPosition < 21)
            {

                if (newPositionIndex <= 21)
                {
                    //straight line
                    movementIndex = 1;
                    movementMode = 1;
                }
                else if ((newPositionIndex > 21 && newPositionIndex <= 27))
                {
                    //simple overlap 
                    movementIndex = 6;
                    movementMode = 2;

                }
                else if (newPositionIndex == 0)
                {

                    movementIndex = 6;
                    movementMode = 2;

                }
                else if (newPositionIndex > 0 && newPositionIndex <= 7)
                {

                    movementIndex = 7;
                    movementMode = 3;
                }

            }
            else if (oldPosition >= 21 && oldPosition <= 27)
            {
                if (newPositionIndex > 21 && newPositionIndex <= 27)
                {

                    movementIndex = 1;
                    movementMode = 1;
                }


            }
            myPositionIndex = newPositionIndex;
            Movement(finalTarget, movementMode);
            
        }
        else//new turn
        {

            if (oldPosition >= 14 && oldPosition < 21)
            {
                if (overlap == 0)
                {

                    movementIndex = 6;
                    movementMode = 2;

                }
                else if (overlap > 0 && overlap <= 7)
                {

                    movementIndex = 7;
                    movementMode = 3;
                }
            }
            else if (oldPosition >= 21 && oldPosition <= 27)

            {
                if (overlap == 0)
                {
 
                    movementMode = 1;
                }
                if (overlap > 0 && overlap <= 7)
                {

                    movementIndex = 8;
                    movementMode = 2;
                }
                else if (overlap > 7 && overlap <= 14)
                {

                    movementIndex = 9;
                    movementMode = 3;
                }
            }

            myPositionIndex = overlap;
            Movement(newTurnTarget, movementMode);
            


        }
    }

    //In the movement function depending on the mode, the player will be moved accordingly
    //And if new turn detected, then the new turn logic gets activated
    private void Movement(Transform endTarget, int mode)
    {
        endPoint = endTarget;
        switch (mode)
        {
            case 1:

                while (!doneMoving)
                {
                    var theOffSet = GetOffset(endPoint, transformToMove);
                    theOffSet = theOffSet.normalized * SPEED;
                    transformToMove.LookAt(endPoint);
                    controller.Move(theOffSet * Time.deltaTime);

                    if (Vector3.Distance(endPoint.position, transformToMove.position) < 2f)
                    {

                        Debug.Log("I have arrived!");
                        doneMoving = true;
                    }
                }
                break;
            case 2:

                switch (movementIndex)
                {
                    case 2:
                        halfwayTarget1 = Targets[CORNER_1];
                        break;
                    case 4:
                        halfwayTarget1 = Targets[CORNER_2];
                        break;
                    case 6:
                        halfwayTarget1 = Targets[CORNER_3];
                        break;
                    case 8:
                        halfwayTarget1 = Targets[CORNER_4];
                        break;
                }
                

                while (!step1Complete)
                {
                    Vector3 theOffSet = GetOffset(halfwayTarget1, transformToMove);
                    theOffSet = theOffSet.normalized * SPEED;
                    transformToMove.LookAt(halfwayTarget1);
                    controller.Move(theOffSet * Time.deltaTime);
                    if (Vector3.Distance(halfwayTarget1.position, transformToMove.position) < .5f)
                    {
                        step1Complete = true;
                        Debug.Log("I have arrived!");
                    }
                }
                while (!doneMoving)
                {
                    Vector3 theOffSet = GetOffset(endPoint, transformToMove);
                    theOffSet = theOffSet.normalized * SPEED;
                    transformToMove.LookAt(endPoint);
                    controller.Move(theOffSet * Time.deltaTime);

                    if (Vector3.Distance(endPoint.position, transformToMove.position) < .5f)
                    {
                        step1Complete = false;
                        doneMoving = true;
                        Debug.Log("I have arrived!");
                    }
                }

                break;
            case 3:

                switch (movementIndex)
                {
                    case 3:

                        halfwayTarget1 = Targets[CORNER_1];
                        halfwayTarget2 = Targets[CORNER_2];
                        break;
                    case 5:

                        halfwayTarget1 = Targets[CORNER_2];
                        halfwayTarget2 = Targets[CORNER_3];
                        break;
                    case 7:

                        halfwayTarget1 = Targets[CORNER_3];
                        halfwayTarget2 = Targets[CORNER_4];
                        break;
                    case 9:

                        halfwayTarget1 = Targets[CORNER_4];
                        halfwayTarget2 = Targets[CORNER_1];
                        break;
                }


                while (!step1Complete)
                {
                    Vector3 theOffSet = GetOffset(halfwayTarget1, transformToMove);
                    transformToMove.LookAt(halfwayTarget1);
                    theOffSet = theOffSet.normalized * SPEED;
                    controller.Move(theOffSet * Time.deltaTime);

                    if (Vector3.Distance(halfwayTarget1.position, transformToMove.position) < .5f)
                    {
                        step1Complete = true;

                    }
                }
                while (!step2Complete)
                {
                    Vector3 theOffSet = GetOffset(halfwayTarget2, transformToMove);
                    transformToMove.LookAt(halfwayTarget2);
                    theOffSet = theOffSet.normalized * SPEED;
                    controller.Move(theOffSet * Time.deltaTime);

                    if (Vector3.Distance(halfwayTarget2.position, transformToMove.position) < .5f)
                    {
                        step2Complete = true;
                    }
                }
                while (!doneMoving)
                {
                    Vector3 theOffSet = GetOffset(endPoint, transformToMove);
                    transformToMove.LookAt(endPoint);
                    theOffSet = theOffSet.normalized * SPEED;
                    controller.Move(theOffSet * Time.deltaTime);

                    if (Vector3.Distance(endPoint.position, transformToMove.position) < .5f)
                    {
                        step1Complete = step2Complete = false;
                        doneMoving = true;

                    }
                }
                break;
        }
      
        if (newTurn)
        {
            newTurn = false;
            BoardManager.SetPositionNewTurn(myPositionIndex);
        }
        else
        {
            BoardManager.SetPosition(myPositionIndex);
        }
        doneMoving = false;

    }
}
