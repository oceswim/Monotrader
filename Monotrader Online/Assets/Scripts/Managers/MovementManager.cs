using Photon.Pun;
using System.Collections;
using UnityEngine;
public class MovementManager : MonoBehaviourPun
{
    private const string PREFDICE = "DiceVal";
    private const int TARGET_LIST_SIZE = 27;
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
    private int myActorNum,myPositionIndex,movementIndex;
    private bool simpleMove, simpleOverlap, complexOverlap,simpleOverlapReady,complexOverlapReady,step1Complete,step2Complete,doneMoving,newTurn;
    private Transform halfwayTarget1, halfwayTarget2;
    private CharacterController controller = null;
    private void Start()
    {


        PlayerPrefs.SetInt(POSITION_INDEX_PREF_KEY, 0);

        transformToMove = FindMyTransform();
        controller = transformToMove.GetComponent<CharacterController>();
        string pathToTargets = "BoardGame/Player"+myActorNum.ToString()+"Spots";
        Targets = new Transform[27];
        InitialiseTargetList(pathToTargets);

        moveMe = simpleOverlapReady = complexOverlapReady = step1Complete = step2Complete =doneMoving= newTurn= false;
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
        Debug.Log("HEY"+myTransf.name);
        return myTransf;
    }
    private void InitialiseTargetList(string path)
    {
        Transform parentTarget = GameObject.Find(path).transform;
        for(int i = 0; i< Targets.Length;i++)
        {
            Targets[i] = parentTarget.GetChild(i);
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
        if (simpleMove)
        {
            var theOffSet = GetOffset(endPoint, transformToMove);
            theOffSet = theOffSet.normalized * SPEED;
            transformToMove.LookAt(endPoint);
            controller.Move(theOffSet * Time.deltaTime);
            Debug.Log(Vector3.Distance(endPoint.position, transformToMove.position));
            if(Vector3.Distance(endPoint.position,transformToMove.position) < 2f)
            {
                simpleMove = false;
                Debug.Log("I have arrived!");
                doneMoving = true;
            }
        }
        else if (simpleOverlap)
        {
            if (!simpleOverlapReady)
            {
                switch (movementIndex)
                {
                    case 2:
                        halfwayTarget1= Targets[CORNER_1];
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
                simpleOverlapReady = true;
            }
            if (!step1Complete)
            {
                Vector3 theOffSet = GetOffset(halfwayTarget1,transformToMove);
                theOffSet = theOffSet.normalized * SPEED;
                transformToMove.LookAt(halfwayTarget1);
                controller.Move(theOffSet * Time.deltaTime);
                Debug.Log(Vector3.Distance(halfwayTarget1.position, transformToMove.position));
                if (Vector3.Distance(halfwayTarget1.position, transformToMove.position) < 2f)
                { 
                    step1Complete = true;
                    Debug.Log("I have arrived!");
                }
            }
            else
            {
                Vector3 theOffSet = GetOffset(endPoint, transformToMove);
                theOffSet = theOffSet.normalized * SPEED;
                transformToMove.LookAt(endPoint);
                controller.Move(theOffSet * Time.deltaTime);
               
                //Debug.Log(Vector3.Distance(endPoint.position, transformToMove.position));

                if (Vector3.Distance(endPoint.position, transformToMove.position) < 2f)
                {
                    step1Complete= simpleOverlap = simpleOverlapReady = false;
                    doneMoving = true;
                    Debug.Log("I have arrived!");
                }
            }
        }
        else if (complexOverlap)
        {
            if (!complexOverlapReady)
            {
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
                complexOverlapReady = true;
            }
            if (!step1Complete)
            {
                Vector3 theOffSet = GetOffset(halfwayTarget1, transformToMove);
                transformToMove.LookAt(halfwayTarget1);
                theOffSet = theOffSet.normalized * SPEED;
                controller.Move(theOffSet * Time.deltaTime);
              
                Debug.Log(controller.transform.position);
                if (theOffSet.magnitude < .1f)
                {
                    step1Complete = true;
                    Debug.Log("I have arrived!");
                }
            }
            else if (!step2Complete)
            {
                Vector3 theOffSet = GetOffset(halfwayTarget2, transformToMove);
                transformToMove.LookAt(halfwayTarget2) ;
                theOffSet = theOffSet.normalized * SPEED;
                controller.Move(theOffSet *Time.deltaTime);
                Debug.Log(controller.transform.position);
                if (theOffSet.magnitude < .1f)
                {
                    step2Complete = true;
                    Debug.Log("I have arrived!");
                }
            }
            else
            {
                Vector3 theOffSet = GetOffset(endPoint, transformToMove);
                transformToMove.LookAt(endPoint);
                theOffSet = theOffSet.normalized * SPEED;
                controller.Move(theOffSet * Time.deltaTime);
                Debug.Log(controller.transform.position);
                if (theOffSet.magnitude < .1f)
                {
                    step1Complete = step2Complete = complexOverlap = complexOverlapReady = false;
                    doneMoving = true;
                    Debug.Log("I have arrived!");
                }
            }
        }

        if(doneMoving)
        {
            doneMoving = false;
            if (newTurn)
            {
                BoardManager.SetPositionNewTurn(myPositionIndex);
            }
            else
            {
                BoardManager.SetPosition(myPositionIndex);
            }
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

        if (newPositionIndex > TARGET_LIST_SIZE)
        {
            newPositionIndex = 0;
        }
        Transform startTarget = Targets[oldPosition];
        Transform finalTarget = Targets[newPositionIndex];
        movementIndex=0;
        int movementMode = 0;
        if(oldPosition>=0 && oldPosition <=7)
        {
            if(newPositionIndex <=7)
            {
                //straight line
                movementIndex = 1;
                movementMode = 1;
            }
            else if(newPositionIndex>7 && newPositionIndex <=14)
            {
                //simple overlap
                movementIndex = 2;
                movementMode = 2;
            }
            else if( newPositionIndex>14 && newPositionIndex<=21)
            {
                //double overlap
                movementIndex = 3;
                movementMode = 3;
            }
        }
        else if(oldPosition >= 7 && oldPosition <= 14)
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
        else if(oldPosition >= 14 && oldPosition <= 21)
        {
            if (newPositionIndex <= 21)
            {
                //straight line
                movementIndex = 1;
                movementMode = 1;
            }
            else if (newPositionIndex > 21 && (newPositionIndex <= 27 || newPositionIndex ==0))
            {
                //simple overlap 
                movementIndex = 6;
                movementMode = 2;
                if (newPositionIndex == 0)
                {
                    //new turn situation
                    newTurn = true;
                }
            }
            else if (newPositionIndex > 0 && newPositionIndex <= 7)
            {
                //double overlap + new turn situation
                newTurn = true;
                movementIndex = 7;
                movementMode = 3;
            }
        }
        else if (oldPosition >= 21 && oldPosition <= 27)
        {
            if (newPositionIndex <= 27 || newPositionIndex == 0)
            {
                //straight line
                if(newPositionIndex==0)
                {
                    newTurn = true;
                }
                movementIndex = 1;
                movementMode = 1;
            }
            else if (newPositionIndex > 0 && newPositionIndex <= 7)
            {
                //simple overlap + new turn 
                newTurn = true;
                movementIndex = 8;
                movementMode = 2;
            }
            else if (newPositionIndex > 7 && newPositionIndex <= 14)
            {
                //double overlap + new turn
                newTurn = true;
                movementIndex = 9;
                movementMode = 3;
            }
        }
        StartProperMovement(movementMode, startTarget, finalTarget);
        myPositionIndex = newPositionIndex;
    }


    private void StartProperMovement(int moveMode,Transform startTarget,Transform finalTarget)
    {
        
      Movement(startTarget, finalTarget,moveMode);

        
    }
    private void Movement(Transform startTarget, Transform endTarget, int mode)
    {
        Debug.Log($"Going from {startTarget.name} To {endTarget.name}");
        Debug.Log($"Going from {startTarget.localPosition} To {endTarget.localPosition}");
        endPoint = endTarget;
        switch (mode)
        {
            case 1:
                simpleMove = true;
                break;
            case 2:
                simpleOverlap = true;
                break;
            case 3:
                complexOverlap = true;
                break;

        }

    
    
    }
}
