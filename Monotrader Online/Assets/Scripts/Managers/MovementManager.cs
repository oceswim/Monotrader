using Photon.Pun;
using System.Collections;
using UnityEngine;
public class MovementManager : MonoBehaviourPun
{
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
    public static bool moveMe;
    private Transform[] Targets;
    private Transform transformToMove, endPoint;
    private int myActorNum, myPositionIndex, movementIndex;
    private bool simpleMove, simpleOverlap, complexOverlap, simpleOverlapReady, complexOverlapReady, step1Complete, step2Complete, doneMoving, newTurn;
    private Transform halfwayTarget1, halfwayTarget2;
    private CharacterController controller = null;
    private void Start()
    {


        PlayerPrefs.SetInt(POSITION_INDEX_PREF_KEY, 0);

        transformToMove = FindMyTransform();
        controller = transformToMove.GetComponent<CharacterController>();
        string pathToTargets = "BoardGame/Player" + myActorNum.ToString() + "Spots";
        Targets = new Transform[TARGET_LIST_SIZE];
        InitialiseTargetList(pathToTargets);

        moveMe = simpleOverlapReady = complexOverlapReady = step1Complete = step2Complete = doneMoving = newTurn = false;
        movementIndex = 0;
    }
    private Transform FindMyTransform()
    {
        Transform myTransf;
        myPositionIndex = PlayerPrefs.GetInt(POSITION_INDEX_PREF_KEY);
        string myName = PlayerPrefs.GetString(PLAYER_NAME_PREF_KEY);
        myActorNum = PhotonNetwork.LocalPlayer.ActorNumber;
        string spawnPath = "StartP" + myActorNum.ToString();
        string fullPath = "BoardGame/Spawners/" + spawnPath + "/" + myName;
        myTransf = GameObject.Find(fullPath).transform;
        Debug.Log("HEY" + myTransf.name);
        return myTransf;
    }
    private void InitialiseTargetList(string path)
    {
        Transform parentTarget = GameObject.Find(path).transform;
        for (int i = 0; i < Targets.Length; i++)
        {
            Targets[i] = parentTarget.GetChild(i);
            Debug.Log(Targets[i].name);
        }
    }
    private void Update()
    {
        if (moveMe)
        {
            moveMe = false;
            int diceVal = PlayerPrefs.GetInt(PREFDICE);
            Debug.Log("Move script : v1 = " + diceVal);
            Movement(diceVal);
        }


    }

    private Vector3 GetOffset(Transform p1, Transform p2)
    {
        // Debug.Log(p1.position + "P1" + p2.position + " P2");
        Vector3 offset = p1.position - p2.position;

        return offset;

    }

    private void Movement(int value)
    {
        int oldPosition = myPositionIndex;
        int newPositionIndex = oldPosition + value;
        int overlap = -1;
        Debug.Log(oldPosition + " old pos and NEW POSITION before: " + newPositionIndex);
        if (newPositionIndex > TARGET_AMOUNT)
        {
            int temp = newPositionIndex - TARGET_AMOUNT;
            overlap = temp - 1;
            Debug.Log("IT;S A NEW TURN SITUATION1");
            newTurn = true;
        }
        Transform startTarget = Targets[oldPosition];
        Debug.Log("NEW POSITION after: " + newPositionIndex);
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
                Debug.Log("G1");
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
                Debug.Log("G2");
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
                Debug.Log("THE MOVEMENT MODE in g2:" + movementMode);
            }
            else if (oldPosition >= 14 && oldPosition < 21)
            {
                Debug.Log("G3");
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
                Debug.Log("THE MOVEMENT MODE in g3:" + movementMode);
            }
            else if (oldPosition >= 21 && oldPosition <= 27)
            {
                if (newPositionIndex > 21 && newPositionIndex <= 27)
                {

                    movementIndex = 1;
                    movementMode = 1;
                }

                Debug.Log("G4" + newPositionIndex + " " + movementIndex + " " + movementMode);
            }
            myPositionIndex = newPositionIndex;
            Movement(startTarget, finalTarget, movementMode);
            
        }
        else//new turn
        {
            Debug.Log("The OVERLAP = " + overlap);
            if (oldPosition >= 14 && oldPosition < 21)
            {
                if (overlap == 0)
                {
                    Debug.Log("NEW TURN IN STRAIGHT overlap");
                    movementIndex = 6;
                    movementMode = 2;

                }
                else if (overlap > 0 && overlap <= 7)
                {
                    Debug.Log("NEW TURN IN complex overlap");
                    movementIndex = 7;
                    movementMode = 3;
                }
            }
            else if (oldPosition >= 21 && oldPosition <= 27)

            {
                if (overlap == 0)
                {
                    Debug.Log("NEW TURN IN STRAIGHT LINE");
                    movementMode = 1;
                }
                if (overlap > 0 && overlap <= 7)
                {
                    Debug.Log("NEW TURN IN overlap");
                    movementIndex = 8;
                    movementMode = 2;
                }
                else if (overlap > 7 && overlap <= 14)
                {
                    Debug.Log("NEW TURN IN complex overlap LINE");
                    movementIndex = 9;
                    movementMode = 3;
                }
            }

            Debug.Log($"at {startTarget.name} and going to {newTurnTarget.name} and movement mode : {movementMode}");
            myPositionIndex = overlap;
            Movement(startTarget, newTurnTarget, movementMode);
            


        }
    }
    private void Movement(Transform startTarget, Transform endTarget, int mode)
    {
        Debug.Log($"Going from {startTarget.name} To {endTarget.name}");
        Debug.Log($"Going from {startTarget.localPosition} To {endTarget.localPosition}");
        Debug.Log("MODE:" + mode);
        endPoint = endTarget;
        switch (mode)
        {
            case 1:
                Debug.Log("In case 1");
                while (!doneMoving)
                {
                    var theOffSet = GetOffset(endPoint, transformToMove);
                    theOffSet = theOffSet.normalized * SPEED;
                    transformToMove.LookAt(endPoint);
                    controller.Move(theOffSet * Time.deltaTime);
                    //Debug.Log(Vector3.Distance(endPoint.position, transformToMove.position));
                    if (Vector3.Distance(endPoint.position, transformToMove.position) < 2f)
                    {

                        Debug.Log("I have arrived!");
                        doneMoving = true;
                    }
                }
                break;
            case 2:
                Debug.Log("In case 2"+movementIndex);
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
                    //Debug.Log(Vector3.Distance(halfwayTarget1.position, transformToMove.position));
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

                    //Debug.Log(Vector3.Distance(endPoint.position, transformToMove.position));

                    if (Vector3.Distance(endPoint.position, transformToMove.position) < .5f)
                    {
                        step1Complete = simpleOverlap = simpleOverlapReady = false;
                        doneMoving = true;
                        Debug.Log("I have arrived!");
                    }
                }

                break;
            case 3:
                Debug.Log("In case 3");
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
                        Debug.Log("I have arrived!");
                    }
                }
                while (!step2Complete)
                {
                    Vector3 theOffSet = GetOffset(halfwayTarget2, transformToMove);
                    transformToMove.LookAt(halfwayTarget2);
                    theOffSet = theOffSet.normalized * SPEED;
                    controller.Move(theOffSet * Time.deltaTime);
                    Debug.Log(theOffSet.magnitude);
                    if (Vector3.Distance(halfwayTarget2.position, transformToMove.position) < .5f)
                    {
                        step2Complete = true;
                        Debug.Log("I have arrived!");
                    }
                }
                while (!doneMoving)
                {
                    Vector3 theOffSet = GetOffset(endPoint, transformToMove);
                    transformToMove.LookAt(endPoint);
                    theOffSet = theOffSet.normalized * SPEED;
                    controller.Move(theOffSet * Time.deltaTime);
                    Debug.Log(theOffSet.magnitude);
                    if (Vector3.Distance(endPoint.position, transformToMove.position) < .5f)
                    {
                        step1Complete = step2Complete = complexOverlap = complexOverlapReady = false;
                        doneMoving = true;
                        Debug.Log("I have arrived!");
                    }
                }

                break;

        }

        if (newTurn)
        {
            newTurn = false;
            
            
            BoardManager.SetPositionNewTurn(myPositionIndex);
            GameManager.instance.TurnManager();
        }
        else
        {
            
            BoardManager.SetPosition(myPositionIndex);
            
        }
        doneMoving = false;

    }
}
