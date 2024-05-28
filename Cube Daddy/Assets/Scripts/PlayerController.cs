using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Cinemachine;
using Pixelplacement;
using System;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using System.Linq;
using static UnityEngine.Rendering.DebugUI;
using DG.Tweening.Core.Enums;

public class PlayerController : MonoBehaviour
{
    //**********************************************************************************************************//
    #region Variables
    [Header("DEBUGGING")]
    [SerializeField] bool debugMovement;


    [Header("PLAYER INPUT")]
    [SerializeField] InputActionAsset inputActions;
    private InputAction move;
    private InputAction camera_Mouse;
    private InputAction camera_Gamepad;
    private Vector2 inputVector;
    [HideInInspector] public Vector2 cameraVector_Mouse;
    [HideInInspector] public Vector2 cameraVector_Gamepad;

    [Space]
    [Space]
    [Header("ROLL MOVEMENT")]
    [SerializeField] public MovementType movementType;
    [SerializeField] public RollType rollType;
    [Header("Movement Bools")]
    [SerializeField] public bool canMove;
    [SerializeField] public bool isMoving;
    [SerializeField] public bool isFalling;
    [SerializeField] public bool isTeleporting;
    [SerializeField] public bool isPopping;
    [Space]
    [SerializeField] bool isMagnetic;
    [SerializeField] public bool onMagneticCube;  
    //ROTATION
    [HideInInspector] float remainingAngle;
    [HideInInspector] Vector3 rotationAnchor;
    [HideInInspector] Vector3 rotationAxis;
    [Header("Movement Speeds")]
    [SerializeField] float rollSpeed;
    [SerializeField] float bonkSpeed;
    [SerializeField] float headBonkSpeed;
    [SerializeField] float step1BonkSpeed;
    [SerializeField] float step2BonkSpeed;
    [SerializeField] float step3BonkSpeed;
    [Header("Movement Curves")]
    [SerializeField] AnimationCurve rollCurve;
    [SerializeField] AnimationCurve bonkCurve;
    [SerializeField] AnimationCurve headBonkCurve;
    [SerializeField] AnimationCurve step1BonkCurve;
    [SerializeField] AnimationCurve step2BonkCurve;
    [SerializeField] AnimationCurve step3BonkCurve;
    [Header("Movement Angles")]
    [SerializeField] float bonkAngle;
    [Header("Fall")]
    [SerializeField] float fallDistance;
    [SerializeField] float fallJourneyLength;
    [SerializeField] AnimationCurve fallCurve;
    [SerializeField] float FALL_MODULATOR;
    [Space]
    [SerializeField] float smallFall_Threshold;
    [SerializeField] float medFall_Threshold;
    [Space]
    [SerializeField] public bool isFallingToDeath;

    [Space]
    [Space]
    [Header("FLY MOVEMENT")]
    [SerializeField] public float FORCE_MODIFIER;
    [SerializeField] public float ROTATION_SPEED;

    [Space]
    [Space]
    [Header("Animations")]
    [SerializeField] UnityEvent onRollContinous;
    [SerializeField] UnityEvent onRollStop;
    [Space]
    [SerializeField] UnityEvent onFall_small;
    [SerializeField] UnityEvent onFall_med;
    [SerializeField] UnityEvent onFall_large;
    [Space]
    [SerializeField] UnityEvent event_teleportStart;
    [SerializeField] UnityEvent event_teleportEnd;

    [Space]
    [Space]
    [Header("Teleport")]
    [SerializeField] float teleportDuration;
    [SerializeField] float teleportDelay;

    [Space]
    [Space]
    [Header("Pop")]

    [SerializeField] float pop_yValue;
    [SerializeField] float popDuration;
    [SerializeField] AnimationCurve popY_Curve;
    [SerializeField] AnimationCurve popXZ_Curve;


    [Space]
    [Space]
    [Header("CUBE VARIABLES")]
    [SerializeField] public CubeData[] cubeDatas;
    [Header("Current Cube")]
    [SerializeField] public float currentScale;
    [SerializeField] public Transform cubeTransform;
    [Header("Larger Cubes")]
    [SerializeField] float mergeDistanceThreshold;
    [SerializeField] public int cubes_index;

    [Space]
    [Space]
    [Header("Respawn")]
    [SerializeField] public int respawnLayer;
    [SerializeField] public Vector3 lastValidPosition;
    [SerializeField] public float respawnTime;


    //NOT SHOWN IN INSPECTOR
    [HideInInspector] CameraController cameraController;
    [SerializeField] public Transform vc_transform;
    [SerializeField] public Rigidbody rb;
    [HideInInspector] public SquashCubesScript squash;
    [HideInInspector] public CalculateRollTypeScript calculateRollTypeScript;
    [HideInInspector] PressurePlate[] pressurePlates;
    [SerializeField] public AnimationController animationController;

    [Space]
    [Header("Tags")]
    [SerializeField] public string tag_Environment;
    [SerializeField] public string tag_MagneticEnvironment;
    [SerializeField] public string tag_player;
    [SerializeField] public string tag_currentPlayer;
    #endregion

    //**********************************************************************************************************//
    #region ENUM
    public enum RollType
    {
        stuck,
        bonk_flat,
        bonk_head,
        bonk_stepUp1,
        bonk_stepUp2,
        bonk_stepUp3,
        bonk_stepDown1,
        bonk_stepDown2,
        bonk_stepDown3,
        bonk_stepLeft1,
        bonk_stepLeft2,
        bonk_stepLeft3,
        bonk_stepRight1,
        bonk_stepRight2,
        bonk_stepRight3,
        bonk_climbUp_flat,
        bonk_climbUp_head,
        bonk_climbLeft_flat,
        bonk_climbLeft_head,
        bonk_climbRight_flat,
        bonk_climbRight_head,
        flat,
        step_Up,
        step_Down,
        step_Left,
        step_Right,
        climb_Up,
        climb_Down,
        climb_Left,
        climb_Right
    }

    public enum MovementType
    {
        roll,
        fly
    }
    #endregion

    //**********************************************************************************************************//
    #region Start
    // Start is called before the first frame update
    void Awake()
    {
        //cubes
        cubeDatas = FindObjectsOfType<CubeData>();
        SortCubeDatasByScale(cubeDatas);

        //cube transform
        cubeTransform = cubeDatas[cubes_index].transform;
        cubeDatas[cubes_index].isCurrentCube = true;

        //input
        inputActions.Enable();
        move = inputActions.FindAction("move");
        move.Enable();

        camera_Mouse = inputActions.FindAction("camera Mouse");
        camera_Mouse.Enable();

        camera_Gamepad = inputActions.FindAction("camera Gamepad");
        camera_Gamepad.Enable();

        //calculate roll type
        calculateRollTypeScript = GetComponent<CalculateRollTypeScript>();

        //rigid body
        rb = cubeTransform.gameObject.GetComponent<Rigidbody>();

        //squash
        squash = GetComponent<SquashCubesScript>();

        //pressure plates
        pressurePlates = FindObjectsOfType<PressurePlate>();

        //camera controller
        cameraController = FindObjectOfType<CameraController>();

        //animation controller
        animationController = cubeDatas[cubes_index].GetComponentInChildren<AnimationController>();

        //set scale
        currentScale = cubeDatas[cubes_index].scale;
    }

    private void Start()
    {
        SetCurrentCube();
    }

    public void SortCubeDatasByScale(CubeData[] cubeDatas)
    {
        // Create a list to hold the cube datas
        List<CubeData> cubeDataList = new List<CubeData>(cubeDatas);

        // Sort the cube datas based on the scale variable
        cubeDataList.Sort((a, b) => a.scale.CompareTo(b.scale));

        // Copy the sorted cube datas back to the original array
        cubeDataList.CopyTo(cubeDatas);
    }

    #endregion

    //**********************************************************************************************************//
    #region Update
    // Update is called once per frame
    void Update()
    {
        //update player game object position
        transform.position = cubeTransform.position;

        //get player input
        #region Player Input
        //get player input
        inputVector = move.ReadValue<Vector2>();
        inputVector.x = Mathf.Round(inputVector.x);
        inputVector.y = Mathf.Round(inputVector.y);

        cameraVector_Mouse = camera_Mouse.ReadValue<Vector2>();
        //cameraVector_Mouse 
        cameraVector_Gamepad = camera_Gamepad.ReadValue<Vector2>();
        #endregion

        //roll movement
        #region Roll Movement
        if (canMove && movementType == MovementType.roll)
        {
            //if current moving or falling do nothing
            if (isMoving) return;
            if (isFalling) return;

            //forward
            if (inputVector.y == 1)
            {
                StartCoroutine(Roll(Vector3.forward));
            }
            //back
            else if (inputVector.y == -1)
            {
                StartCoroutine(Roll(Vector3.back));
            }
            //right
            else if (inputVector.x == 1)
            {
                StartCoroutine(Roll(Vector3.right));
            }
            //left
            else if (inputVector.x == -1)
            {
                StartCoroutine(Roll(Vector3.left));
            }
        }
        #endregion

        


        //quit
        #region quit
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        #endregion
    }


    public void FixedUpdate()
    {
        //fly movement
        #region Fly Movement
        if (movementType == MovementType.fly)
        {
            FlyMovement();
        }
        #endregion
    }
    #endregion

    //**********************************************************************************************************//
    #region Roll Coroutine
    //coroutine
    IEnumerator Roll(Vector3 direction)
    {
        //translate relative direction;
        direction = TranslateRelativeDirection(direction);

        //figure out roll type
        rollType = calculateRollTypeScript.CalculateRollType(cubeTransform.position, direction, currentScale);
        if (debugMovement) calculateRollTypeScript.DebugMovemet(rollType, cubeTransform.position, direction, currentScale);

        //is moving = true
        isMoving = true;
        onMagneticCube = CheckIfOnMagneticCube();

        //set the roll type information
        #region Roll Type
        //get roll information
        switch (rollType)
        {
            #region flat
            //stuck
            case RollType.stuck:
                isMoving = false;
                yield break;

            //bonk
            case RollType.bonk_flat:
                remainingAngle = bonkAngle;
                rotationAnchor = cubeTransform.position - direction * currentScale / 2 + Vector3.down * currentScale / 2;
                break;

            //head bonk
            case RollType.bonk_head:
                remainingAngle = bonkAngle;
                rotationAnchor = cubeTransform.position + direction * currentScale / 2 + Vector3.down * currentScale / 2;
                direction = -direction;
                break;

            //flat
            case RollType.flat:
                remainingAngle = 90f;
                rotationAnchor = cubeTransform.position + direction * currentScale / 2 + Vector3.down * currentScale / 2;
                break;

            #endregion

            #region Step

            #region Step Up
            //step up
            case RollType.step_Up:
                remainingAngle = 180f;
                rotationAnchor = cubeTransform.position + direction * currentScale / 2 + Vector3.up * currentScale / 2;
                break;

            //step up bonk 1
            case RollType.bonk_stepUp1:
                remainingAngle = bonkAngle;
                rotationAnchor = cubeTransform.position + direction * currentScale / 2 + Vector3.up * currentScale / 2;
                direction = -direction;
                break;

            //step up bonk 2
            case RollType.bonk_stepUp2:
                remainingAngle = bonkAngle;
                rotationAnchor = cubeTransform.position + direction * currentScale / 2 + Vector3.up * currentScale / 2;
                direction = -direction;
                break;

            //step up bonk 3
            case RollType.bonk_stepUp3:
                remainingAngle = bonkAngle;
                rotationAnchor = cubeTransform.position + direction * currentScale / 2 + Vector3.up * currentScale / 2;
                direction = -direction;
                break;

            #endregion

            #region Step Down
            //step down
            case RollType.step_Down:
                remainingAngle = 180f;
                rotationAnchor = cubeTransform.position + direction * currentScale / 2 + Vector3.down * currentScale / 2;
                break;

            //step down bonk 1
            case RollType.bonk_stepDown1:
                remainingAngle = bonkAngle;
                rotationAnchor = cubeTransform.position + direction * currentScale / 2 + Vector3.down * currentScale / 2;
                direction = -direction;
                break;

            //step down bonk 2
            case RollType.bonk_stepDown2:
                remainingAngle = bonkAngle;
                rotationAnchor = cubeTransform.position + direction * currentScale / 2 + Vector3.down * currentScale / 2;
                direction = -direction;
                break;

            //step down bonk 3
            case RollType.bonk_stepDown3:
                remainingAngle = bonkAngle;
                rotationAnchor = cubeTransform.position + direction * currentScale / 2 + Vector3.down * currentScale / 2;
                direction = -direction;
                break;

            #endregion

            #region Step Left
            //step left
            case RollType.step_Left:
                remainingAngle = 180f;
                rotationAnchor = cubeTransform.position + direction * currentScale / 2 + -Vector3.Cross(direction, Vector3.up) * currentScale / 2;
                break;

            //step left bonk 1
            case RollType.bonk_stepLeft1:
                remainingAngle = bonkAngle;
                rotationAnchor = cubeTransform.position + direction * currentScale / 2 + -Vector3.Cross(direction, Vector3.up) * currentScale / 2;
                break;

            //step left bonk 2
            case RollType.bonk_stepLeft2:
                remainingAngle = bonkAngle;
                rotationAnchor = cubeTransform.position + direction * currentScale / 2 + -Vector3.Cross(direction, Vector3.up) * currentScale / 2;
                break;

            //step left bonk 2
            case RollType.bonk_stepLeft3:
                remainingAngle = bonkAngle;
                rotationAnchor = cubeTransform.position + direction * currentScale / 2 + -Vector3.Cross(direction, Vector3.up) * currentScale / 2;
                break;

            #endregion

            #region Step Right
            //step right
            case RollType.step_Right:
                remainingAngle = 180f;
                rotationAnchor = cubeTransform.position + direction * currentScale / 2 + Vector3.Cross(direction, Vector3.up) * currentScale / 2;
                break;

            //step right bonk 1
            case RollType.bonk_stepRight1:
                remainingAngle = bonkAngle;
                rotationAnchor = cubeTransform.position + direction * currentScale / 2 + Vector3.Cross(direction, Vector3.up) * currentScale / 2;
                direction = -direction;
                break;

            //step right bonk 2
            case RollType.bonk_stepRight2:
                remainingAngle = bonkAngle;
                rotationAnchor = cubeTransform.position + direction * currentScale / 2 + Vector3.Cross(direction, Vector3.up) * currentScale / 2;
                direction = -direction;
                break;

            //step right bonk 3
            case RollType.bonk_stepRight3:
                remainingAngle = bonkAngle;
                rotationAnchor = cubeTransform.position + direction * currentScale / 2 + Vector3.Cross(direction, Vector3.up) * currentScale / 2;
                direction = -direction;
                break;

            #endregion

            #endregion

            #region Climb

            #region Climb Up
            //climb up
            case RollType.climb_Up:
                remainingAngle = 90f;
                rotationAnchor = cubeTransform.position + direction * currentScale / 2 + Vector3.up * currentScale / 2;
                break;

            //climb up bonk flat
            case RollType.bonk_climbUp_flat:
                remainingAngle = bonkAngle;
                rotationAnchor = cubeTransform.position + direction * currentScale / 2 + Vector3.down * currentScale / 2;
                break;

            //climb up bonk head
            case RollType.bonk_climbUp_head:
                remainingAngle = bonkAngle;
                rotationAnchor = cubeTransform.position + direction * currentScale / 2 + Vector3.up * currentScale / 2;
                direction = -direction;
                break;

            #endregion

            #region Climb Down
            //climb down
            case RollType.climb_Down:
                remainingAngle = 90f;
                rotationAnchor = cubeTransform.position + -direction * currentScale / 2 + Vector3.down * currentScale / 2;
                break;

            #endregion

            #region Climb Left
            //climb left
            case RollType.climb_Left:
                remainingAngle = 90f;
                rotationAnchor = cubeTransform.position + direction * currentScale / 2 + -Vector3.Cross(direction, Vector3.up) * currentScale / 2;
                break;

            //climb left bonk flat
            case RollType.bonk_climbLeft_flat:
                remainingAngle = bonkAngle;
                rotationAnchor = cubeTransform.position - direction * currentScale / 2 + -Vector3.Cross(direction, Vector3.up) * currentScale / 2;
                break;

            //climb left bonk head
            case RollType.bonk_climbLeft_head:
                remainingAngle = bonkAngle;
                rotationAnchor = cubeTransform.position + direction * currentScale / 2 + -Vector3.Cross(direction, Vector3.up) * currentScale / 2;
                break;

            #endregion

            #region Climb Right
            //climb right
            case RollType.climb_Right:
                remainingAngle = 90f;
                rotationAnchor = cubeTransform.position + direction * currentScale / 2 + Vector3.Cross(direction, Vector3.up) * currentScale / 2;
                break;

            //climb right bonk flat
            case RollType.bonk_climbRight_flat:
                remainingAngle = bonkAngle;
                rotationAnchor = cubeTransform.position - direction * currentScale / 2 + Vector3.Cross(direction, Vector3.up) * currentScale / 2;
                break;

            //climb right bonk flat
            case RollType.bonk_climbRight_head:
                remainingAngle = bonkAngle;
                rotationAnchor = cubeTransform.position + direction * currentScale / 2 + Vector3.Cross(direction, Vector3.up) * currentScale / 2;
                break;

                #endregion

                #endregion
        }
        #endregion

        //execute roll rotation
        #region Rotation
        //////////////////////////////////////////////////////////////////////////////

        //find roll axis
        //LEFT
        if (rollType == RollType.climb_Left ||
            rollType == RollType.step_Left ||
            rollType == RollType.bonk_climbLeft_flat)
        {
            rotationAxis = Vector3.up;
        }
        else if (rollType == RollType.bonk_climbLeft_head)
        {
            rotationAxis = Vector3.down;
        }
        else if (rollType == RollType.bonk_stepLeft1 ||
                rollType == RollType.bonk_stepLeft2 ||
                rollType == RollType.bonk_stepLeft3)
        {
            rotationAxis = Vector3.down;
        }

        //RIGHT
        else if (rollType == RollType.climb_Right ||
                 rollType == RollType.step_Right ||
                 rollType == RollType.bonk_climbRight_flat)
        {
            rotationAxis = Vector3.down;
        }
        else if (rollType == RollType.bonk_climbRight_head)
        {
            rotationAxis = Vector3.up;
        }
        else if (rollType == RollType.bonk_stepRight1 ||
                rollType == RollType.bonk_stepRight2 ||
                rollType == RollType.bonk_stepRight3)
        {
            rotationAxis = Vector3.up;
        }

        //NORMAL
        else
        {
            rotationAxis = Vector3.Cross(Vector3.up, direction);
        }
        //reset timer
        float timer = 0;

        //while there is still angle to go
        while (remainingAngle > 0)
        {
            float rotationAngle;
            //calculate rotation angle
            //uses min so that it goes exactly to the angle specified
            //bonk flat
            if (rollType == RollType.bonk_flat || rollType == RollType.bonk_climbUp_flat || rollType == RollType.bonk_climbLeft_flat || rollType == RollType.bonk_climbRight_flat)
            {
                rotationAngle = Mathf.Min(bonkCurve.Evaluate(timer) * bonkSpeed * Time.deltaTime, remainingAngle);
            }
            //head bonk
            else if (rollType == RollType.bonk_head || rollType == RollType.bonk_climbUp_head || rollType == RollType.bonk_climbLeft_head || rollType == RollType.bonk_climbRight_head)
            {
                rotationAngle = Mathf.Min(headBonkCurve.Evaluate(timer) * headBonkSpeed * Time.deltaTime, remainingAngle);
            }
            //bonk step 1
            else if (rollType == RollType.bonk_stepUp1 || rollType == RollType.bonk_stepDown1 || rollType == RollType.bonk_stepLeft1 || rollType == RollType.bonk_stepRight1)
            {
                rotationAngle = Mathf.Min(step1BonkCurve.Evaluate(timer) * step1BonkSpeed * Time.deltaTime, remainingAngle);
            }
            //bonk step 2
            else if (rollType == RollType.bonk_stepUp2 || rollType == RollType.bonk_stepDown2 || rollType == RollType.bonk_stepLeft2 || rollType == RollType.bonk_stepRight2)
            {
                rotationAngle = Mathf.Min(step2BonkCurve.Evaluate(timer) * step2BonkSpeed * Time.deltaTime, remainingAngle);
            }
            //bonk step 3
            else if (rollType == RollType.bonk_stepUp3 || rollType == RollType.bonk_stepDown3 || rollType == RollType.bonk_stepLeft3 || rollType == RollType.bonk_stepRight3)
            {
                rotationAngle = Mathf.Min(step3BonkCurve.Evaluate(timer) * step3BonkSpeed * Time.deltaTime, remainingAngle);
            }
            //normal
            else
            {
                rotationAngle = Mathf.Min(rollCurve.Evaluate(timer) * rollSpeed * Time.deltaTime, remainingAngle);
            }

            //rotation
            cubeTransform.RotateAround(rotationAnchor, rotationAxis, rotationAngle);

            //decrement remaing angle by the amount rotated
            remainingAngle -= rotationAngle;

            //increment timer
            timer += Time.deltaTime;
            yield return null;
        }

        if (rollType == RollType.bonk_flat || rollType == RollType.bonk_climbUp_flat || rollType == RollType.bonk_climbLeft_flat || rollType == RollType.bonk_climbRight_flat)
        {
            cubeTransform.RotateAround(rotationAnchor, rotationAxis, -bonkAngle);
        }
        else if (rollType == RollType.bonk_head || rollType == RollType.bonk_climbUp_head || rollType == RollType.bonk_climbLeft_head || rollType == RollType.bonk_climbRight_head)
        {
            cubeTransform.RotateAround(rotationAnchor, rotationAxis, -bonkAngle);
        }
        else if (rollType == RollType.bonk_stepUp1 || rollType == RollType.bonk_stepDown1 || rollType == RollType.bonk_stepLeft1 || rollType == RollType.bonk_stepRight1)
        {
            cubeTransform.RotateAround(rotationAnchor, rotationAxis, -bonkAngle);
        }
        else if (rollType == RollType.bonk_stepUp2 || rollType == RollType.bonk_stepDown2 || rollType == RollType.bonk_stepLeft2 || rollType == RollType.bonk_stepRight2)
        {
            cubeTransform.RotateAround(rotationAnchor, rotationAxis, -bonkAngle);
        }
        else if (rollType == RollType.bonk_stepUp3 || rollType == RollType.bonk_stepDown3 || rollType == RollType.bonk_stepLeft3 || rollType == RollType.bonk_stepRight3)
        {
            cubeTransform.RotateAround(rotationAnchor, rotationAxis, -bonkAngle);
        }


        ///////////////////////////////////////////////////////////////////////////////
        #endregion

        //fix round errors
        #region Round Errors
        //FixFloatingPointErrors();
        #endregion

        //falling behaviour
        #region Fall
        //fall
        //if there is nothing below at the end of a move
        if (!Physics.Raycast(cubeTransform.position, Vector3.down, currentScale, calculateRollTypeScript.rollLayerMask) && !CheckIfOnMagneticCube())
        {
            canMove = false;

            //draw a debug ray down
            //Debug.DrawRay(cubeTransform.position, Vector3.down * Mathf.Infinity, Color.white, scale);

            //raycast straight down
            Physics.Raycast(cubeTransform.position + Vector3.down * currentScale / 2, Vector3.down, out RaycastHit hit_fall, Mathf.Infinity, calculateRollTypeScript.rollLayerMask);

            //get distance
            fallDistance = hit_fall.distance;

            //falling to death check
            isFallingToDeath = hit_fall.collider.gameObject.layer == respawnLayer;

            //move cube down 
            Tween.Position(cubeTransform, cubeTransform.position + Vector3.down * fallDistance, fallDistance / currentScale / FALL_MODULATOR, 0, fallCurve, Tween.LoopType.None, HandleStartFallTween, HandleEndFallTween);

            //set falling to true
            isFalling = true;
        }
        else
        {
            //call animations
            #region Call Animations
            //roll continous
            if (inputVector.magnitude != 0f)
            {
                onRollContinous.Invoke();
            }
            //roll stop
            else
            {
                onRollStop.Invoke();
            }
            #endregion
        }

        while (isFalling)
        {
            yield return new WaitForEndOfFrame();
        }
        #endregion

        //final checks to end movement
        #region end movement

        //pressure plates
        CheckAllPressurePlates();

        //check if on magnetic cube
        onMagneticCube = CheckIfOnMagneticCube();

        //check that cube hasnt been completed
        #region check completed cube
        if (cubeDatas.Count() > 0 && cubes_index < cubeDatas.Count() - 1)
        {
            //if the distance between the current cube and the next cubes missing transform is less than or equal to the merge distance threshold
            if (Vector3.Distance(cubeDatas[cubes_index].transform.position, cubeDatas[cubes_index + 1].missingPosition.position) <= mergeDistanceThreshold * currentScale &&
                cubeDatas[cubes_index + 1].canMerge)
            {
                //merge cube
                MergeCube_method();
            }
        }

        #endregion

        //last valid move transform
        #region Respawn
        if (Physics.Raycast(cubeTransform.position, Vector3.down, out RaycastHit hit_down, currentScale) && hit_down.collider.gameObject.layer != respawnLayer)
        {
            lastValidPosition = cubeTransform.position;
        }
        #endregion

        Debug.Log("End movement");
        //set isMoving to false
        EndRoll();

        #endregion
    }

    private void EndRoll()
    {
        isMoving = false;
        isFalling = false;
        isTeleporting = false;
        isFallingToDeath = false;
    }


    //**********************************************************************************************************//
    //methods that assist the roll coroutine
    #region Roll Helper Methods

    //**********************************************************************************************************//
    //methods that help with magnetic cube calculations
    #region Magnetic Methods
    public bool CheckIfOnMagneticCube()
    {
        /*Debug.DrawRay(cubeTransform.position, Vector3.forward * scale, Color.black, scale);
        Debug.DrawRay(cubeTransform.position, Vector3.back * scale, Color.black, scale);
        Debug.DrawRay(cubeTransform.position, Vector3.left * scale, Color.black, scale);
        Debug.DrawRay(cubeTransform.position, Vector3.right * scale, Color.black, scale);
        Debug.DrawRay(cubeTransform.position, Vector3.up * scale, Color.black, scale);
        Debug.DrawRay(cubeTransform.position, Vector3.down * scale, Color.black, scale);*/

        /*Debug.DrawRay(cubeTransform.position + Vector3.up * scale, Vector3.forward, Color.gray, scale);
        Debug.DrawRay(cubeTransform.position + Vector3.down * scale, Vector3.forward, Color.gray, scale);
        Debug.DrawRay(cubeTransform.position + Vector3.left * scale, Vector3.forward, Color.gray, scale);
        Debug.DrawRay(cubeTransform.position + Vector3.right * scale, Vector3.forward, Color.gray, scale);

        Debug.DrawRay(cubeTransform.position + Vector3.up * scale, Vector3.back, Color.gray, scale);
        Debug.DrawRay(cubeTransform.position + Vector3.down * scale, Vector3.back, Color.gray, scale);
        Debug.DrawRay(cubeTransform.position + Vector3.left * scale, Vector3.back, Color.gray, scale);
        Debug.DrawRay(cubeTransform.position + Vector3.right * scale, Vector3.back, Color.gray, scale);

        Debug.DrawRay(cubeTransform.position + Vector3.up * scale, Vector3.left, Color.gray, scale);
        Debug.DrawRay(cubeTransform.position + Vector3.down * scale, Vector3.left, Color.gray, scale);
        Debug.DrawRay(cubeTransform.position + Vector3.left * scale, Vector3.left, Color.gray, scale);
        Debug.DrawRay(cubeTransform.position + Vector3.right * scale, Vector3.left, Color.gray, scale);

        Debug.DrawRay(cubeTransform.position + Vector3.up * scale, Vector3.right, Color.gray, scale);
        Debug.DrawRay(cubeTransform.position + Vector3.down * scale, Vector3.right, Color.gray, scale);
        Debug.DrawRay(cubeTransform.position + Vector3.left * scale, Vector3.right, Color.gray, scale);
        Debug.DrawRay(cubeTransform.position + Vector3.right * scale, Vector3.right, Color.gray, scale);

        Debug.DrawRay(cubeTransform.position + Vector3.up * scale, Vector3.up, Color.gray, scale);
        Debug.DrawRay(cubeTransform.position + Vector3.down * scale, Vector3.up, Color.gray, scale);
        Debug.DrawRay(cubeTransform.position + Vector3.left * scale, Vector3.up, Color.gray, scale);
        Debug.DrawRay(cubeTransform.position + Vector3.right * scale, Vector3.up, Color.gray, scale);

        Debug.DrawRay(cubeTransform.position + Vector3.up * scale, Vector3.down, Color.gray, scale);
        Debug.DrawRay(cubeTransform.position + Vector3.down * scale, Vector3.down, Color.gray, scale);
        Debug.DrawRay(cubeTransform.position + Vector3.left * scale, Vector3.down, Color.gray, scale);
        Debug.DrawRay(cubeTransform.position + Vector3.right * scale, Vector3.down, Color.gray, scale);*/

        #region flat
        //forward
        if (Physics.Raycast(cubeTransform.position, Vector3.forward, out RaycastHit hitForward, currentScale) &&
            hitForward.collider.gameObject.CompareTag(tag_MagneticEnvironment))
        {
            //Debug.Log("forward: " + true);
            return true;
        }
        //Debug.Log("forward: " + false);

        //back
        if(Physics.Raycast(cubeTransform.position, Vector3.back, out RaycastHit hitBackward, currentScale) &&
           hitBackward.collider.gameObject.CompareTag(tag_MagneticEnvironment))
        {
            //Debug.Log("back: " + true);
            return true;
        }
        //Debug.Log("back: " + false);
        
        //left
        if (Physics.Raycast(cubeTransform.position, Vector3.left, out RaycastHit hitLeft, currentScale) &&
                 hitLeft.collider.gameObject.CompareTag(tag_MagneticEnvironment))
        {
            //Debug.Log("left: " + true);
            return true;
        }
        //Debug.Log("left: " + false);
        
        //right
        if (Physics.Raycast(cubeTransform.position, Vector3.right, out RaycastHit hitRight, currentScale) &&
            hitRight.collider.gameObject.CompareTag(tag_MagneticEnvironment))
        {
            //Debug.Log("right: " + true);
            return true;
        }
        //Debug.Log("right: " + false);

        //down
        if (Physics.Raycast(cubeTransform.position, Vector3.down, out RaycastHit hitDown, currentScale) &&
            hitDown.collider.gameObject.CompareTag(tag_MagneticEnvironment))
        {

            //Debug.Log("down: " + true);
            return true;
        }
        //Debug.Log("down: " + false);

        //up
        if (Physics.Raycast(cubeTransform.position, Vector3.up, out RaycastHit hitUp, currentScale) &&
            hitUp.collider.gameObject.CompareTag(tag_MagneticEnvironment))
        {
           //Debug.Log("up: " + true);
            return true;
        }
        //Debug.Log("up: " + false);

        #endregion

        #region edge
        int numEdges = 0;

        //forward up
        if (Physics.Raycast(cubeTransform.position + Vector3.up * currentScale, Vector3.forward, out RaycastHit hitForwardUp, currentScale))
        {
            if (hitForwardUp.collider.gameObject.CompareTag(tag_MagneticEnvironment))
            {
                numEdges++;
            }
        }

        //forward down
        if (Physics.Raycast(cubeTransform.position + Vector3.down * currentScale, Vector3.forward, out RaycastHit hitForwardDown, currentScale))
        {
            if (hitForwardDown.collider.gameObject.CompareTag(tag_MagneticEnvironment))
            {
                numEdges++;
            }
        }

        //forward left
        if (Physics.Raycast(cubeTransform.position + Vector3.left * currentScale, Vector3.forward, out RaycastHit hitForwardLeft, currentScale))
        {
            if (hitForwardLeft.collider.gameObject.CompareTag(tag_MagneticEnvironment))
            {
                numEdges++;
            }
        }

        //forward right
        if (Physics.Raycast(cubeTransform.position + Vector3.right * currentScale, Vector3.forward, out RaycastHit hitForwardRight, currentScale))
        {
            if (hitForwardRight.collider.gameObject.CompareTag(tag_MagneticEnvironment))
            {
                numEdges++;
            }
        }

        //Back up
        if (Physics.Raycast(cubeTransform.position + Vector3.up * currentScale, Vector3.back, out RaycastHit hitBackUp, currentScale))
        {
            if (hitBackUp.collider.gameObject.CompareTag(tag_MagneticEnvironment))
            {
                numEdges++;
            }
        }


        //Back down
        if (Physics.Raycast(cubeTransform.position + Vector3.down * currentScale, Vector3.back, out RaycastHit hitBackDown, currentScale))
        {
            if (hitBackDown.collider.gameObject.CompareTag(tag_MagneticEnvironment))
            {
                numEdges++;
            }
        }

        //back left
        if (Physics.Raycast(cubeTransform.position + Vector3.left * currentScale, Vector3.back, out RaycastHit hitbackLeft, currentScale))
        {
            if (hitbackLeft.collider.gameObject.CompareTag(tag_MagneticEnvironment))
            {
                numEdges++;
            }
        }

        //back right
        if (Physics.Raycast(cubeTransform.position + Vector3.right * currentScale, Vector3.back, out RaycastHit hitbackRight, currentScale))
        {
            if (hitbackRight.collider.gameObject.CompareTag(tag_MagneticEnvironment))
            {
                numEdges++;
            }
        }

        //left up
        else if (Physics.Raycast(cubeTransform.position + Vector3.up * currentScale, Vector3.left, out RaycastHit hitleftUp, currentScale))
        {
            if (hitleftUp.collider.gameObject.CompareTag(tag_MagneticEnvironment))
            {
                numEdges++;
            }
        }

        //left down
        if (Physics.Raycast(cubeTransform.position + Vector3.down * currentScale, Vector3.left, out RaycastHit hitleftDown, currentScale))
        {
            if (hitleftDown.collider.gameObject.CompareTag(tag_MagneticEnvironment))
            {
                numEdges++;
            }
        }

        //right up
        else if (Physics.Raycast(cubeTransform.position + Vector3.up * currentScale, Vector3.right, out RaycastHit hitrightUp, currentScale))
        {
            if (hitrightUp.collider.gameObject.CompareTag(tag_MagneticEnvironment))
            {
                numEdges++;
            }
        }

        //right down
        if (Physics.Raycast(cubeTransform.position + Vector3.down * currentScale, Vector3.right, out RaycastHit hitrightDown, currentScale))
        {
            if (hitrightDown.collider.gameObject.CompareTag(tag_MagneticEnvironment))
            {
                numEdges++;
            }
        }

        //
        
        if (numEdges >= 2)
        {
            return true;
        }
        else
        {
            return false;
        }
        #endregion
    }

    #endregion

    //**********************************************************************************************************//
    //pressure plate methods
    #region Pressure Plates
    public void CheckAllPressurePlates()
    {
        foreach (PressurePlate pressurePlate in pressurePlates)
        {
            pressurePlate.CheckIfTriggered(cubeTransform.position, currentScale);
        }
    }


    #endregion

    //**********************************************************************************************************//
    //methods that control what happens at the start and end of the fall tween
    #region Fall Tween Handlers
    public void HandleStartFallTween()
    {

    }


    public void HandleEndFallTween()
    {
        isFalling = false;

        if (!isFallingToDeath)
        {
            if (fallDistance < smallFall_Threshold * currentScale)
            {
                Debug.Log("Small: " + fallDistance);
                onFall_small.Invoke();
            }
            else if (fallDistance < medFall_Threshold * currentScale)
            {
                Debug.Log("Med: " + fallDistance);
                onFall_med.Invoke();
            }
            else
            {
                Debug.Log("Large: " + fallDistance);
                onFall_large.Invoke();
            }
        }
        else
        {
            StartCoroutine(Teleport(lastValidPosition, teleportDuration, teleportDelay));
        }
    }
    #endregion

    //**********************************************************************************************************//
    //fix floating point errors made by the computer
    #region Precision
    private void FixFloatingPointErrors()
    {
        //rotation ~ ROUND TO NEAREST INTEGER
        cubeTransform.rotation = Quaternion.Euler(Mathf.Round(cubeTransform.rotation.eulerAngles.x), Mathf.Round(cubeTransform.rotation.eulerAngles.y), Mathf.Round(cubeTransform.rotation.eulerAngles.z));

        //position ~ ROUND TO NEAREST TENTH
        Vector3 positionVector = cubeTransform.position;
        positionVector = new Vector3(positionVector.x * 10, positionVector.y * 10, positionVector.z * 10);
        positionVector = new Vector3(Mathf.Round(positionVector.x), Mathf.Round(positionVector.y), Mathf.Round(positionVector.z));
        positionVector = new Vector3(positionVector.x / 10, positionVector.y / 10, positionVector.z / 10);
        cubeTransform.position = positionVector;
    }
    #endregion

    //calcualate the direction vector based on the camera
    #region Camera Relative Direction
    public Vector3 TranslateRelativeDirection(Vector3 direction)
    {
        //forward
        if (direction == Vector3.forward)
        {
            return RoundToClosestCardinalDirection(vc_transform.forward);
        }
        //back
        else if (direction == Vector3.back)
        {
            return RoundToClosestCardinalDirection(-vc_transform.forward);
        }
        //right
        else if (direction == Vector3.right)
        {
            return RoundToClosestCardinalDirection(vc_transform.right);
        }
        //left
        else if (direction == Vector3.left)
        {
            return RoundToClosestCardinalDirection(-vc_transform.right);
        }
        //oopsie
        else
        {
            return Vector3.zero;
        }
    }

    public Vector3 RoundToClosestCardinalDirection(Vector3 inputDirection)
    {
        //Debug.Log(inputDirection);
        Vector3 output = inputDirection;
        output.y = 0;
        output.x = Mathf.Round(output.x);
        output.z = Mathf.Round(output.z);

        if (Mathf.Abs(inputDirection.x) > Mathf.Abs(inputDirection.z))
        {
            output.z = 0;
        }
        else
        {
            output.x = 0;
        }

        //Debug.Log(output);
        return output;
    }
    #endregion

    #endregion

    #endregion

    //**********************************************************************************************************//
    #region Fly Movement
    public void FlyMovement()
    {
        //set the space variables, should not be done here
        if(rb.isKinematic)
        {
            rb.isKinematic = false;
            cubeDatas[cubes_index].SetTeleportParticleSystem(true);
        }

        //if there is input
        if(inputVector.magnitude > 0)
        {
            Vector3 force = vc_transform.forward * inputVector.y + vc_transform.right * inputVector.x;
            force *= currentScale * FORCE_MODIFIER * rb.mass;
            rb.AddForce(force, ForceMode.Force);
        }

        Debug.DrawRay(cubeTransform.position, rb.velocity.normalized, Color.red, 0.1f);
        cubeTransform.rotation = Quaternion.RotateTowards(cubeTransform.rotation, Quaternion.Euler(rb.velocity.normalized), ROTATION_SPEED);
    }

    #endregion

    //**********************************************************************************************************//
    #region Merge

    public void MergeCube_method()
    {
        StartCoroutine(MergeCube());
    }

    //merge cube
    private IEnumerator MergeCube()
    {
        //set movement off
        canMove = false;

        //set current small cubes parent to be large cubes transform
        cubeDatas[cubes_index].gameObject.SetActive(false);
        cubeDatas[cubes_index].isCurrentCube = false;

        //turn on squashable cubes
        squash.MakeCubesSquashable(cubes_index);

        //set cube transform to large cube transform
        cubeTransform = cubeDatas[cubes_index + 1].transform;

        //set scale
        currentScale = cubeDatas[cubes_index + 1].scale;

        //set active = false incomplete meshes
        cubeDatas[cubes_index + 1].incompleteMesh.SetActive(false);

        //set active = true complete meshes
        cubeDatas[cubes_index + 1].completeMesh.SetActive(true);

        //set next current cube in cube data
        cubeDatas[cubes_index + 1].isCurrentCube = true;

        //update camera size
        StartCoroutine(cameraController.StartCameraScale(cubeDatas[cubes_index].scale, cubeDatas[cubes_index + 1].scale));

        //update camera follow transform
        cameraController.cameraFollow.currentCubeTransform = cubeDatas[cubes_index + 1].transform;

        //merge events
        cubeDatas[cubes_index + 1].mergeEvents.Invoke();

        //animation
        animationController = cubeDatas[cubes_index + 1].GetComponentInChildren<AnimationController>();

        //rigid body
        rb = cubeDatas[cubes_index + 1].GetComponent<Rigidbody>();

        //increment index
        if (cubes_index < cubeDatas.Length - 1)
        {
            cubes_index++;
        }

        yield return null;

        //set can move
        canMove = true;
    }

    public void Demerge_method(Transform targetPosition)
    {
        StartCoroutine(Demerge(targetPosition));
    }

    public IEnumerator Demerge(Transform targetTransform)
    {

        Vector3 demerge_direction = targetTransform.position - cubeDatas[cubes_index].transform.position;

        Quaternion cube_Direction = Quaternion.identity;

        Vector3 roll_Direction = Vector3.zero;

        if(Mathf.Abs(demerge_direction.x) > Mathf.Abs(demerge_direction.z))
        {
            if (demerge_direction.x > 0)
            {
                //Debug.Log("Direction 1");
                cube_Direction = Quaternion.Euler(new Vector3(0, 270f, 0));
                roll_Direction = Vector3.left;
            }
            else
            {
                //Debug.Log("Direction 3");
                cube_Direction = Quaternion.Euler(new Vector3(0, 90f, 0));
                roll_Direction = Vector3.right;
            }
        }
        else
        {
            if (demerge_direction.z > 0)
            {
                //Debug.Log("Direction 4");
                cube_Direction = Quaternion.Euler(new Vector3(0, 180f, 0));
                roll_Direction = Vector3.back;
            }
            else
            {
                //Debug.Log("Direction 2");
                cube_Direction = Quaternion.identity;
                roll_Direction = Vector3.forward;
            }
        }

        //set movement off
        canMove = false;

        //////////////////////////// CURRENT CUBE ////////////////////////////

        //reset transform
        cubeDatas[cubes_index].transform.rotation = cube_Direction;

        //set active = true incomplete meshes
        cubeDatas[cubes_index].incompleteMesh.SetActive(true);

        //set active = false complete meshes
        cubeDatas[cubes_index].completeMesh.SetActive(false);

        //set current small cubes parent to be large cubes transform
        cubeDatas[cubes_index].isCurrentCube = false;

        //set can merge to false
        cubeDatas[cubes_index].canMerge = false;

        //////////////////////////// smaller CUBE ////////////////////////////
        //set cube transform to large cube transform
        cubeTransform = cubeDatas[cubes_index - 1].transform;

        //set scale
        currentScale = cubeDatas[cubes_index - 1].scale;

        //set position of smaller cube
        cubeDatas[cubes_index - 1].transform.position = cubeDatas[cubes_index].missingPosition.position;

        //reset rotation
        cubeDatas[cubes_index - 1].transform.rotation = Quaternion.identity;

        //set active = false incomplete meshes
        cubeDatas[cubes_index - 1].incompleteMesh.SetActive(false);

        //set active = true complete meshes
        cubeDatas[cubes_index - 1].completeMesh.SetActive(true);

        //turn it on
        cubeDatas[cubes_index - 1].gameObject.SetActive(true);

        //set next current cube in cube data
        cubeDatas[cubes_index - 1].isCurrentCube = true;


        //////////////////////////// Camera ////////////////////////////
        //update camera size
        StartCoroutine(cameraController.StartCameraScale(cubeDatas[cubes_index].scale, cubeDatas[cubes_index - 1].scale));

        //update camera follow transform
        cameraController.cameraFollow.currentCubeTransform = cubeDatas[cubes_index - 1].transform;

        //animation
        animationController = cubeDatas[cubes_index - 1].GetComponentInChildren<AnimationController>();

        //rigid body
        rb = cubeDatas[cubes_index - 1].GetComponent<Rigidbody>();

        //turn on squashable cubes
        squash.MakeCubesHard(cubes_index - 1);

        //merge events
        cubeDatas[cubes_index - 1].mergeEvents.Invoke();

        //increment index
        if (cubes_index > 0)
        {
            cubes_index--;
        }

        //do colour stuff

        //Debug.DrawRay(cubeTransform.position, roll_Direction, Color.blue, Mathf.Infinity);

        yield return new WaitForSeconds(0.1f);

        //take a step
        StartCoroutine(Roll(roll_Direction));

        yield return null;

        //set can move
        canMove = true;
    }

    private void SetCurrentCube()
    {
        //set movement off
        canMove = false;

        if(cubes_index > 0)
        {
            //turn on squashable cubes
            squash.MakeCubesSquashable(cubes_index - 1);
        }
        
        //set cube transform to large cube transform
        cubeTransform = cubeDatas[cubes_index].transform;

        //set scale
        currentScale = cubeDatas[cubes_index].scale;

        //set active = false incomplete meshes
        cubeDatas[cubes_index].incompleteMesh.SetActive(false);

        //set active = true complete meshes
        cubeDatas[cubes_index].completeMesh.SetActive(true);

        //set next current cube in cube data
        cubeDatas[cubes_index].isCurrentCube = true;

        //update camera size
        StartCoroutine(cameraController.StartCameraScale(cubeDatas[cubes_index].scale, cubeDatas[cubes_index].scale));

        //update camera follow transform
        cameraController.cameraFollow.currentCubeTransform = cubeDatas[cubes_index].transform;

        //animation
        animationController = cubeDatas[cubes_index].GetComponentInChildren<AnimationController>();

        //rigid body
        rb = cubeDatas[cubes_index].GetComponent<Rigidbody>();

        //set can move
        canMove = true;
    }


    #endregion

    //**********************************************************************************************************//
    #region Teleport
    public void Teleport_Method(Vector3 teleportPos, float duration, float delay)
    {
        StartCoroutine(Teleport(teleportPos, duration, delay));
    }



    public IEnumerator Teleport(Vector3 teleportPos, float duration, float delay)
    {
        isFallingToDeath = false;
        cubeTransform.GetComponent<CubeData>().SetTeleportParticleSystem(true);
        event_teleportStart.Invoke();
        Tween.Position(cubeTransform, teleportPos, duration, delay, Tween.EaseInOut, Tween.LoopType.None, TeleportStartTween, TeleportEndTween);
        yield return new WaitForEndOfFrame();
    }

    public void TeleportStartTween()
    {
        
    }

    public void TeleportEndTween()
    {
        cubeTransform.GetComponent<CubeData>().SetTeleportParticleSystem(false);
        event_teleportEnd.Invoke();
    }


    #endregion

    //**********************************************************************************************************//
    #region Animation

    public void PlayAnimation_CannotMove(string animationTrigger)
    {
        animationController.PlayAnimation_cannotMove_Method(animationTrigger);
    }

    public void PlayAnimation_CanMove(string animationTrigger)
    {
        animationController.PlayAnimation_canMove_Method(animationTrigger);
    }


    #endregion
}