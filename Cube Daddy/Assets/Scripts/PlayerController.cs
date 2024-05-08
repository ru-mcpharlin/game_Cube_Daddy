using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using Pixelplacement;
using System;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    //**********************************************************************************************************//
    #region Variables
    [Header("Player Input")]
    [SerializeField] InputActionAsset inputActions;
    private InputAction move;
    private InputAction camera_Mouse;
    private InputAction camera_Gamepad;
    [Space]
    [SerializeField] public Vector2 inputVector;
    [SerializeField] public Vector2 cameraVector_Mouse;
    [SerializeField] public Vector2 cameraVector_Gamepad;

    [Space]
    [Space]
    [Header("Tags")]
    [SerializeField] string tag_Environment;
    [SerializeField] string tag_MagenticEnvironment;
    [SerializeField] string tag_player;
    [SerializeField] string tag_currentPlayer;

    [Space]
    [Space]
    [Header("Movement")]
    [SerializeField] bool isMoving;
    [SerializeField] bool isFalling;
    [Space]
    [SerializeField] bool isMagnetic;
    [SerializeField] bool onMagneticCube;
    [Space]
    [SerializeField] RollType rollType;
    [Header("Rotation")]
    [SerializeField] float remainingAngle;
    [SerializeField] Vector3 rotationAnchor;
    [SerializeField] Vector3 rotationAxis;
    [Space]
    [Header("Cube variables")]
    [SerializeField] float scale;
    [SerializeField] float rollSpeed;
    [SerializeField] AnimationCurve rollCurve;
    [Space]
    [SerializeField] float bonkAngle;
    [SerializeField] float bonkSpeed;
    [SerializeField] AnimationCurve bonkCurve;
    [Space]
    [SerializeField] float headBonkAngle;
    [SerializeField] float headBonkSpeed;
    [SerializeField] AnimationCurve headBonkCurve;
    [Space]
    [SerializeField] float step1BonkAngle;
    [SerializeField] float step1BonkSpeed;
    [SerializeField] AnimationCurve step1BonkCurve;
    [Space]
    [SerializeField] float step2BonkAngle;
    [SerializeField] float step2BonkSpeed;
    [SerializeField] AnimationCurve step2BonkCurve;
    [Space]
    [SerializeField] float step3BonkAngle;
    [SerializeField] float step3BonkSpeed;
    [SerializeField] AnimationCurve step3BonkCurve;
    [Space]
    
    [Header("Fall")]
    [SerializeField] float fallDistance;
    [SerializeField] float fallJourneyLength;
    [SerializeField] AnimationCurve fallCurve;
    [SerializeField] RaycastHit hit;

    [Space]
    [Space]
    [Header("Cube")]
    [SerializeField] Transform cubeTransform;

    [Space]
    [Space]
    [Header("Larger Cubes")]
    [SerializeField] int cubes_index;
    [SerializeField] CubeData[] cubeDatas;

    [Space]
    [Space]
    [Header("Camera")]
    [SerializeField] CameraController cameraController;
    [SerializeField] public Transform vc_transform;
    [SerializeField] CameraFollow cameraFollow;
    [SerializeField] float cameraTrackingSpeed;

    [Space]
    [Space]
    [Header("Pressure Plates")]
    [SerializeField] PressurePlate[] pressurePlates;

    [Space]
    [Space]
    [Header("Enemies")]
    [SerializeField] EnemyController[] enemies;
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
    #endregion

    //**********************************************************************************************************//
    #region Start
    // Start is called before the first frame update
    void Start()
    {
        //pressure plates
        pressurePlates = FindObjectsOfType<PressurePlate>();

        //eneies
        enemies = FindObjectsOfType<EnemyController>();

        //cube transform
        cubeTransform = GameObject.FindGameObjectWithTag(tag_currentPlayer).transform;

        //camera controller
        cameraController = FindObjectOfType<CameraController>();

        //camera follow
        cameraFollow = FindObjectOfType<CameraFollow>();
        cameraFollow.currentCubeTransform = cubeTransform;

        //cubes
        cubeDatas = FindObjectsOfType<CubeData>();
        SortCubeDatasByScale(cubeDatas);

        //input
        inputActions.Enable();
        move = inputActions.FindAction("move");
        move.Enable();

        camera_Mouse = inputActions.FindAction("camera Mouse");
        camera_Mouse.Enable();

        camera_Gamepad = inputActions.FindAction("camera Gamepad");
        camera_Gamepad.Enable();
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
        cameraVector_Gamepad = camera_Gamepad.ReadValue<Vector2>();
        #endregion

        //camera 3 debugging
        #region camera debug
        //camera control
        if (cameraController.cameraState == CameraController.CameraState.camera3_DynamicIsometric_Unlocked)
        {
            if (Mathf.Abs(cameraVector_Gamepad.x) >= cameraController.camera3_gamepadThreshold)
            {
                if(cameraVector_Gamepad.x > 0)
                {
                    cameraController.DecreaseCamera3Index();
                }
                else
                {
                    cameraController.IncreaseCamera3Index();
                }
            }
            else if(Mathf.Abs(cameraVector_Mouse.x) >= cameraController.camera3_mouseThreshold)
            {
                if (cameraVector_Mouse.x > 0)
                {
                    cameraController.DecreaseCamera3Index();
                }
                else
                {
                    cameraController.IncreaseCamera3Index();
                }
            }
        }

        #endregion

        //control movement
        #region Movement
        //if current moving or falling do nothing
        if (isMoving) return;
        if (isFalling) return;

        //forward
        if (inputVector.y == 1)
        {
            StartCoroutine(Roll(Vector3.forward));
        }
        //back
        else if(inputVector.y == -1)
        {
            StartCoroutine(Roll(Vector3.back));
        }
        //right
        else if(inputVector.x == 1)
        {
            StartCoroutine(Roll(Vector3.right));
        }
        //left
        else if (inputVector.x == -1)
        {
            StartCoroutine(Roll(Vector3.left));
        }
        #endregion

        //quit
        #region quit
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
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
        rollType = CalculateRollType(direction, cubeTransform.position, scale);

        //is moving = true
        isMoving = true;
        onMagneticCube = false;

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
                rotationAnchor = cubeTransform.position - direction * scale / 2 + Vector3.down * scale / 2;
                break;

            //head bonk
            case RollType.bonk_head:
                remainingAngle = headBonkAngle;
                rotationAnchor = cubeTransform.position + direction * scale / 2 + Vector3.down * scale / 2;
                direction = -direction;
                break;

            //flat
            case RollType.flat:
                remainingAngle = 90f;
                rotationAnchor = cubeTransform.position + direction * scale / 2 + Vector3.down * scale / 2;
                break;

            #endregion

            #region Step
            //step up
            case RollType.step_Up:
                remainingAngle = 180f;
                rotationAnchor = cubeTransform.position + direction * scale / 2 + Vector3.up * scale / 2;
                break;

            //step up bonk 1
            case RollType.bonk_stepUp1:
                remainingAngle = headBonkAngle;
                rotationAnchor = cubeTransform.position + direction * scale / 2 + Vector3.up * scale / 2;
                direction = -direction;
                break;

            //step up bonk 2
            case RollType.bonk_stepUp2:
                remainingAngle = step2BonkAngle;
                rotationAnchor = cubeTransform.position + direction * scale / 2 + Vector3.up * scale / 2;
                direction = -direction;
                break;

            //step up bonk 3
            case RollType.bonk_stepUp3:
                remainingAngle = step3BonkAngle;
                rotationAnchor = cubeTransform.position + direction * scale / 2 + Vector3.up * scale / 2;
                direction = -direction;
                break;

            //step down
            case RollType.step_Down:
                remainingAngle = 180f;
                rotationAnchor = cubeTransform.position + direction * scale / 2 + Vector3.down * scale / 2;
                break;

            //step down bonk 1
            case RollType.bonk_stepDown1:
                remainingAngle = step1BonkAngle;
                rotationAnchor = cubeTransform.position + direction * scale / 2 + Vector3.down * scale / 2;
                direction = -direction;
                break;

            //step down bonk 2
            case RollType.bonk_stepDown2:
                remainingAngle = step2BonkAngle;
                rotationAnchor = cubeTransform.position + direction * scale / 2 + Vector3.down * scale / 2;
                direction = -direction;
                break;

            //step down bonk 3
            case RollType.bonk_stepDown3:
                remainingAngle = step3BonkAngle;
                rotationAnchor = cubeTransform.position + direction * scale / 2 + Vector3.down * scale / 2;
                direction = -direction;
                break;

            //step left
            case RollType.step_Left:
                remainingAngle = 180f;
                rotationAnchor = cubeTransform.position + direction * scale / 2 + -Vector3.Cross(direction, Vector3.up) * scale / 2;
                break;

            //step right
            case RollType.step_Right:
                remainingAngle = 180f;
                rotationAnchor = cubeTransform.position + direction * scale / 2 + Vector3.Cross(direction, Vector3.up) * scale / 2;
                break;

            #endregion

            #region Climb

            //climb up
            case RollType.climb_Up:
                remainingAngle = 90f;
                rotationAnchor = cubeTransform.position + direction * scale / 2 + Vector3.up * scale / 2;
                break;

            //climb down
            case RollType.climb_Down:
                remainingAngle = 90f;
                rotationAnchor = cubeTransform.position + -direction * scale / 2 + Vector3.down * scale / 2;
                break;

            //climb left
            case RollType.climb_Left:
                remainingAngle = 90f;
                rotationAnchor = cubeTransform.position + direction * scale / 2 + -Vector3.Cross(direction, Vector3.up) * scale / 2;
                break;

            case RollType.climb_Right:
                remainingAngle = 90f;
                rotationAnchor = cubeTransform.position + direction * scale / 2 + Vector3.Cross(direction, Vector3.up) * scale / 2;
                break;

            #endregion
        }
        #endregion

        //execute roll rotation
        #region Rotation
        //////////////////////////////////////////////////////////////////////////////

        //find roll axis
        if (rollType == RollType.climb_Left || rollType == RollType.step_Left)
        {
            rotationAxis = Vector3.up;
        }
        else if(rollType == RollType.climb_Right || rollType == RollType.step_Right)
        {
            rotationAxis = Vector3.down;
        }
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
            if(rollType == RollType.bonk_flat)
            {
                rotationAngle = Mathf.Min(bonkCurve.Evaluate(timer) * bonkSpeed * Time.deltaTime, remainingAngle);
            }
            else if(rollType == RollType.bonk_head)
            {
                rotationAngle = Mathf.Min(headBonkCurve.Evaluate(timer) * headBonkSpeed * Time.deltaTime, remainingAngle);
            }
            else if (rollType == RollType.bonk_stepUp1 || rollType == RollType.bonk_stepDown1)
            {
                rotationAngle = Mathf.Min(step1BonkCurve.Evaluate(timer) * step1BonkSpeed * Time.deltaTime, remainingAngle);
            }
            else if (rollType == RollType.bonk_stepUp2 || rollType == RollType.bonk_stepDown2)
            {
                rotationAngle = Mathf.Min(step2BonkCurve.Evaluate(timer) * step2BonkSpeed * Time.deltaTime, remainingAngle);
            }
            else if (rollType == RollType.bonk_stepUp3 || rollType == RollType.bonk_stepDown3)
            {
                rotationAngle = Mathf.Min(step3BonkCurve.Evaluate(timer) * step3BonkSpeed * Time.deltaTime, remainingAngle);
            }
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

        if (rollType == RollType.bonk_flat)
        {
            cubeTransform.RotateAround(rotationAnchor, rotationAxis, -bonkAngle);
        }
        else if (rollType == RollType.bonk_head)
        {
            cubeTransform.RotateAround(rotationAnchor, rotationAxis, -headBonkAngle);
        }
        else if(rollType == RollType.bonk_stepUp1 || rollType == RollType.bonk_stepDown1)
        {
            cubeTransform.RotateAround(rotationAnchor, rotationAxis, -step1BonkAngle);
        }
        else if (rollType == RollType.bonk_stepUp2 || rollType == RollType.bonk_stepDown2)
        {
            cubeTransform.RotateAround(rotationAnchor, rotationAxis, -step2BonkAngle);
        }
        else if (rollType == RollType.bonk_stepUp3 || rollType == RollType.bonk_stepDown3)
        {
            cubeTransform.RotateAround(rotationAnchor, rotationAxis, -step3BonkAngle);
        }


        ///////////////////////////////////////////////////////////////////////////////
        #endregion

        //fix round errors
        #region Round Errors
        FixFloatingPointErrors();
        #endregion

        //falling behaviour
        #region Fall
        //fall
        //if there is nothing below at the end of a move
        if (!Physics.Raycast(cubeTransform.position, Vector3.down, scale))
        {
            //check not on a magnetic tile
            CheckIfOnMagneticCube();

            if (!onMagneticCube)
            {
                //draw a debug ray down
                Debug.DrawRay(cubeTransform.position, Vector3.down, Color.white, Mathf.Infinity);

                //raycast straight down
                Physics.Raycast(cubeTransform.position + Vector3.down * scale / 2, Vector3.down, out hit, Mathf.Infinity);

                //get distance
                fallDistance = hit.distance;

                //move cube down 
                Tween.Position(cubeTransform, cubeTransform.position + Vector3.down * fallDistance, fallJourneyLength * fallDistance / scale, 0, fallCurve, Tween.LoopType.None, HandleStartFallTween, HandleEndFallTween);

                //set falling to true
                isFalling = true;
            }
        }
        #endregion

        //final checks to end movement
        #region end movement
        //if is falling do nothing
        if (isFalling)
        {
            yield return new WaitForEndOfFrame();
        }
        else
        {
            //pressure plates
            CheckAllPressurePlates();

            //check if on magnetic cube
            CheckIfOnMagneticCube();

            //enemy moves
            CheckEnemyMovement();

            //check that cube hasnt been completed
            #region check completed cube
            if (cubeDatas.Count() > 0)
            {
                if (cubeTransform.position == cubeDatas[cubes_index].missingPosition.position)
                {
                    //set current small cubes parent to be large cubes transform
                    cubeTransform.gameObject.SetActive(false);

                    //set cube transform to large cube transform
                    cubeTransform = cubeDatas[cubes_index].transform;

                    //update camera follow transform
                    cameraFollow.currentCubeTransform = cubeDatas[cubes_index].transform;

                    //set scale
                    scale = cubeDatas[cubes_index].scale;

                    //set active = false incomplete meshes
                    cubeDatas[cubes_index].incompleteMesh.SetActive(false);

                    //set active = true complete meshes
                    cubeDatas[cubes_index].completeMesh.SetActive(true);

                    //increment index
                    if (cubes_index < cubeDatas.Length - 1)
                    {
                        cubes_index++;
                    }
                }
            }

            #endregion

            //set isMoving to false
            isMoving = false;
        }

        #endregion
    }

    //**********************************************************************************************************//
    //methods that assist the roll coroutine
    #region Roll Helper Methods

    //**********************************************************************************************************//
    //methods that help with magnetic cube calculations
    #region Magnetic Methods
    private void CheckIfOnMagneticCube()
    {
        #region Debug
        /*Debug.DrawRay(cubeTransform.position, Vector3.forward, Color.black, scale);
        Debug.Log("Forward" + Physics.Raycast(cubeTransform.position, Vector3.forward, scale));
        Debug.DrawRay(cubeTransform.position, Vector3.back, Color.black, scale);
        Debug.Log("back" + Physics.Raycast(cubeTransform.position, Vector3.back, scale));
        Debug.DrawRay(cubeTransform.position, Vector3.left, Color.black, scale);
        Debug.Log("left" + Physics.Raycast(cubeTransform.position, Vector3.left, scale));
        Debug.DrawRay(cubeTransform.position, Vector3.right, Color.black, scale);
        Debug.Log("right" + Physics.Raycast(cubeTransform.position, Vector3.right, scale));*/
        #endregion

        //forward
        RaycastHit hitForward;
        if (Physics.Raycast(cubeTransform.position, Vector3.forward, out hitForward, scale))
        {
            if (hitForward.collider.gameObject.tag == tag_MagenticEnvironment)
            {
                onMagneticCube = true;
            }
        }

        //back
        RaycastHit hitBackward;
        if (Physics.Raycast(cubeTransform.position, Vector3.back, out hitBackward, scale))
        {
            if (hitBackward.collider.gameObject.tag == tag_MagenticEnvironment)
            {
                onMagneticCube = true;
            }
        }

        //left
        RaycastHit hitLeft;
        if (Physics.Raycast(cubeTransform.position, Vector3.left, out hitLeft, scale))
        {
            if (hitLeft.collider.gameObject.tag == tag_MagenticEnvironment)
            {
                onMagneticCube = true;
            }
        }

        //right
        RaycastHit hitRight;
        if (Physics.Raycast(cubeTransform.position, Vector3.right, out hitRight, scale))
        {
            if (hitRight.collider.gameObject.tag == tag_MagenticEnvironment)
            {
                onMagneticCube = true;
            }
        }

        //down
        RaycastHit hitDown;
        if (Physics.Raycast(cubeTransform.position, Vector3.down, out hitDown, scale))
        {
            if (hitDown.collider.gameObject.tag == tag_MagenticEnvironment)
            {
                onMagneticCube = true;
            }
        }

        //up
        RaycastHit hitUp;
        if (Physics.Raycast(cubeTransform.position, Vector3.up, out hitUp, scale))
        {
            if (hitUp.collider.gameObject.tag == tag_MagenticEnvironment)
            {
                onMagneticCube = true;
            }
        }
    }

    #endregion

    //**********************************************************************************************************//
    //pressure plate methods
    #region Pressure Plates
    public void CheckAllPressurePlates()
    {
        foreach(PressurePlate pressurePlate in pressurePlates)
        {
            pressurePlate.CheckIfTriggered(cubeTransform.position, scale);
        }
    }


    #endregion

    //**********************************************************************************************************//
    //enemy
    #region Enemy Handlers

    public void CheckEnemyMovement()
    {
        foreach(EnemyController enemy in enemies)
        {
            enemy.EnemyBehaviour();
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
        isMoving = false;
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

    //**********************************************************************************************************//
    //get the corrent roll type
    #region Roll Type Calculation
    //calculate roll type
    private RollType CalculateRollType(Vector3 direction, Vector3 position, float scale)
    {
        #region Debug

        //step up
        /*Debug.DrawRay(cubeTransform.position, direction, Color.red, scale);
        Debug.DrawRay(cubeTransform.position + Vector3.up * scale, direction, Color.red, scale);
        Debug.DrawRay(cubeTransform.position, Vector3.up, Color.red, scale);*/
        //step down
        /*Debug.DrawRay(cubeTransform.position, direction, Color.green, scale);
        Debug.DrawRay(cubeTransform.position + direction * scale, Vector3.down, Color.green, scale);*/
        //climb left
        /* Debug.DrawRay(cubeTransform.position, -Vector3.Cross(direction, Vector3.up), Color.red, scale);
         Debug.DrawRay(cubeTransform.position + direction * scale, -Vector3.Cross(direction, Vector3.up), Color.red, scale);*/
        #endregion

        //step left
        RaycastHit hit_stepLeft;

        //step right
        RaycastHit hit_stepRight;

        //climb up
        RaycastHit hit_climbUp;

        //climb down
        RaycastHit hit_climbDown;

        //climb left
        RaycastHit hit_climbLeft;

        //climb right
        RaycastHit hit_climbRight;

        //bonk
        RaycastHit hit_bonk;


        //**********************************************************************************************************//
        // STEP
        #region STEP

        // STEP UP
        #region STEP UP
        ////////////// STEP UP //////////////
        // IS cube direction 1
        // &&
        // IS NOT cube direction 1 + up 1 
        // &&
        // IS NOT cube up 1
        // &&
        // IS NOT cube direction -1
        // &&
        // IS NOT cube direction -1 + up 1
        // &&
        // IS NOT cube up + 2
        // &&
        // IS NOT cube direction 1 + up + 2
        if (Physics.Raycast(position, direction, scale) &&
            !Physics.Raycast(position + Vector3.up * scale, direction, scale) &&
            !Physics.Raycast(position, Vector3.up, scale) &&
            !Physics.Raycast(position, -direction, scale) &&
            !Physics.Raycast(position + Vector3.up * scale, -direction, scale) &&
            !Physics.Raycast(position + Vector3.up * scale, Vector3.up, scale) &&
            !Physics.Raycast(position + Vector3.up * scale * 2, direction, scale))
        {

            return RollType.step_Up;
        }

        ////////////// STEP UP 1 BONK //////////////
        // IS cube direction 1
        // &&
        // IS NOT cube direction 1 + up 1 
        // &&
        // IS NOT cube up 1
        // &&
        // IS NOT cube direction -1
        // &&
        // IS cube direction -1 + up 1
        if (Physics.Raycast(position, direction, scale) &&
            !Physics.Raycast(position + Vector3.up * scale, direction, scale) &&
            !Physics.Raycast(position, Vector3.up, scale) &&
            !Physics.Raycast(position, -direction, scale) &&
            Physics.Raycast(position + Vector3.up * scale, -direction, scale))
        {

            return RollType.bonk_stepUp1;
        }

        ////////////// STEP UP 2 BONK //////////////
        // IS cube direction 1
        // &&
        // IS NOT cube direction 1 + up 1 
        // &&
        // IS NOT cube up 1
        // &&
        // IS NOT cube direction -1
        // &&
        // IS NOT cube direction -1 + up 1
        // &&
        // IS cube up + 2

        if (Physics.Raycast(position, direction, scale) &&
            !Physics.Raycast(position + Vector3.up * scale, direction, scale) &&
            !Physics.Raycast(position, Vector3.up, scale) &&
            !Physics.Raycast(position, -direction, scale) &&
            !Physics.Raycast(position + Vector3.up * scale, -direction, scale) &&
            Physics.Raycast(position + Vector3.up * scale, Vector3.up, scale))
        {

            return RollType.bonk_stepUp2;
        }

        ////////////// STEP UP 3 BONK //////////////
        // IS cube direction 1
        // &&
        // IS NOT cube direction 1 + up 1 
        // &&
        // IS NOT cube up 1
        // &&
        // IS NOT cube direction -1
        // &&
        // IS NOT cube direction -1 + up 1
        // &&
        // IS NOT cube up + 2
        // &&
        // IS cube direction 1 + up + 2
        if (Physics.Raycast(position, direction, scale) &&
            !Physics.Raycast(position + Vector3.up * scale, direction, scale) &&
            !Physics.Raycast(position, Vector3.up, scale) &&
            !Physics.Raycast(position, -direction, scale) &&
            !Physics.Raycast(position + Vector3.up * scale, -direction, scale) &&
            !Physics.Raycast(position + Vector3.up * scale, Vector3.up, scale) &&
            Physics.Raycast(position + Vector3.up * scale * 2, direction, scale))
        {

            return RollType.bonk_stepUp3;
        }

        #endregion

        // STEP DOWN
        #region STEP DOWN
        ////////////// STEP DOWN //////////////
        // IS cube down 1
        // &&
        // IS NOT cube direction 1
        // &&
        // IS NOT cube direction 1 + down 1
        // &&
        // IS NOT cube up 1
        // &&
        // IS NOT cube direction 1 + up 1
        // &&
        // IS NOT cube in direction 2
        // &&
        // IS NOT cube in direction 2 + down 1
        else if (Physics.Raycast(position, Vector3.down, scale) &&
                 !Physics.Raycast(position, direction, scale) &&
                 !Physics.Raycast(position + direction * scale, Vector3.down, scale) &&
                 !Physics.Raycast(position, Vector3.up, scale) &&
                 !Physics.Raycast(position + Vector3.up * scale, direction, scale) &&
                 !Physics.Raycast(position + direction * scale, direction, scale) &&
                 !Physics.Raycast(position + direction * scale * 2, Vector3.down, scale))
        {
            return RollType.step_Down;
        }

        ////////////// STEP DOWN BONK 1 //////////////
        // IS cube down 1
        // &&
        // IS NOT cube direction 1
        // &&
        // IS NOT cube direction 1 + down 1
        // &&
        // IS NOT cube up 1
        // &&
        // IS cube direction 1 + up 1
        else if (Physics.Raycast(position, Vector3.down, scale) &&
                 !Physics.Raycast(position, direction, scale) &&
                 !Physics.Raycast(position + direction * scale, Vector3.down, scale) &&
                 !Physics.Raycast(position, Vector3.up, scale) &&
                 Physics.Raycast(position + Vector3.up * scale, direction, scale))
        {
            return RollType.bonk_stepDown1;
        }

        ////////////// STEP DOWN BONK 2 //////////////
        // IS cube down 1
        // &&
        // IS NOT cube direction 1
        // &&
        // IS NOT cube direction 1 + down 1
        // &&
        // IS NOT cube up 1
        // &&
        // IS NOT cube direction 1 + up 1
        // &&
        // IS cube in direction 2
        else if (Physics.Raycast(position, Vector3.down, scale) &&
                 !Physics.Raycast(position, direction, scale) &&
                 !Physics.Raycast(position + direction * scale, Vector3.down, scale) &&
                 !Physics.Raycast(position, Vector3.up, scale) &&
                 !Physics.Raycast(position + Vector3.up * scale, direction, scale) &&
                 Physics.Raycast(position + direction * scale, direction, scale))
        {
            return RollType.flat;
        }

        ////////////// STEP DOWN BONK 3 //////////////
        // IS cube down 1
        // &&
        // IS NOT cube direction 1
        // &&
        // IS NOT cube direction 1 + down 1
        // &&
        // IS NOT cube up 1
        // &&
        // IS NOT cube direction 1 + up 1
        // &&
        // IS NOT cube in direction 2
        // &&
        // IS cube in direction 2 + down 1
        else if (Physics.Raycast(position, Vector3.down, scale) &&
                 !Physics.Raycast(position, direction, scale) &&
                 !Physics.Raycast(position + direction * scale, Vector3.down, scale) &&
                 !Physics.Raycast(position, Vector3.up, scale) &&
                 !Physics.Raycast(position + Vector3.up * scale, direction, scale) &&
                 !Physics.Raycast(position + direction * scale, direction, scale) &&
                 Physics.Raycast(position + direction * scale * 2, Vector3.down, scale))
        {
            return RollType.flat;
        }

        ////////////// STEP LEFT //////////////
        //if IS cube 'forward' 1 && IS magnetic
        // &&
        //if IS NOT cube direction 1
        // &&
        //if IS NOT cube direction 1 && forward 1
        // &&
        //if IS NOT cube down 1
        else if (Physics.Raycast(position, -Vector3.Cross(direction, Vector3.up), out hit_stepLeft, scale) &&
                hit_stepLeft.collider.gameObject.tag == tag_MagenticEnvironment &&
                !Physics.Raycast(position, direction, scale) &&
                !Physics.Raycast(position + direction * scale, -Vector3.Cross(direction, Vector3.up), scale)) /*&&
                !Physics.Raycast(cubeTransform.position, Vector3.down, scale))*/
        {
            return RollType.step_Left;
        }

        ////////////// STEP RIGHT //////////////
        //if IS cube 'forward' 1 && IS magnetic
        // &&
        //if IS NOT cube direction 1
        // &&
        //if IS NOT cube direction 1 && forward 1
        // &&
        //if IS NOT cube down 1
        else if (Physics.Raycast(position, Vector3.Cross(direction, Vector3.up), out hit_stepRight, scale) &&
                hit_stepRight.collider.gameObject.tag == tag_MagenticEnvironment &&
                !Physics.Raycast(position, direction, scale) &&
                !Physics.Raycast(position + direction * scale, Vector3.Cross(direction, Vector3.up), scale)) /*&&
                !Physics.Raycast(cubeTransform.position, Vector3.down, scale))*/
        {
            return RollType.step_Right;
        }

        #endregion

        #endregion

        //**********************************************************************************************************//
        //CLIMB
        #region CLIMB
        ////////////// CLIMB UP //////////////
        //if IS cube direction 1 && IS magnetic
        // &&
        //if IS cube direction 1 + up 1
        // &&
        //if IS NOT cube up 1
        else if (Physics.Raycast(position, direction, out hit_climbUp, scale) &&
                 hit_climbUp.collider.gameObject.tag == tag_MagenticEnvironment &&
                 Physics.Raycast(position + Vector3.up * scale, direction, scale) &&
                 !Physics.Raycast(position, Vector3.up, scale))
        {
            return RollType.climb_Up;
        }

        ////////////// CLIMB DOWN //////////////
        //if IS NOT cube down 1
        // &&
        //if IS cube back 1 && IS magnetic
        else if (!Physics.Raycast(position, Vector3.down, scale) &&
                 Physics.Raycast(position, -direction, out hit_climbDown, scale) &&
                 hit_climbDown.collider.gameObject.tag == tag_MagenticEnvironment)
        {
            return RollType.climb_Down;
        }

        ////////////// CLIMB LEFT //////////////
        //if IS cube 'forward' 1 && is magnetic
        // &&
        //if IS cube left 1 && 'forward' 1
        // &&
        //if IS NOT cube down 1
        // &&
        //if IS NOT cube direction 1
        else if (Physics.Raycast(position, -Vector3.Cross(direction, Vector3.up), out hit_climbLeft, scale) &&
                 hit_climbLeft.collider.gameObject.tag == tag_MagenticEnvironment &&
                 Physics.Raycast(position + direction * scale, -Vector3.Cross(direction, Vector3.up), scale) &&
                 !Physics.Raycast(position, Vector3.down, scale) &&
                 !Physics.Raycast(position, direction, scale)) 
        {
            return RollType.climb_Left;
        }

        ////////////// CLIMB RIGHT //////////////
        //if IS cube 'forward' 1 && is magnetic
        // &&
        //if IS cube right 1 && 'forward' 1
        // &&
        //if IS NOT cube down 1      
        // &&
        //if IS NOT cube direction 1
        if (Physics.Raycast(position, Vector3.Cross(direction, Vector3.up), out hit_climbRight, scale) &&
                 hit_climbRight.collider.gameObject.tag == tag_MagenticEnvironment &&
                 Physics.Raycast(position + direction * scale, Vector3.Cross(direction, Vector3.up), scale) &&
                 !Physics.Raycast(position, Vector3.down, scale) &&
                 !Physics.Raycast(position, direction, scale))
        {
            return RollType.climb_Right;
        }

        #endregion

        //**********************************************************************************************************//
        //FLAT
        #region
        ////////////// BONK //////////////
        // IS cube direction 1 && IS NOT magnetic
        // && 
        // IS cube direction 1 + up 1
        // &&
        // IS NOT cube direction -1
        //
        // OR
        //
        // IS cube direction 1
        // &&
        // IS cube up 1
        // &&
        // IS NOT cube direction -1
        else if (Physics.Raycast(position, direction, out hit_bonk, scale) &&
                 Physics.Raycast(position + Vector3.up * scale, direction, scale) &&
                 hit_bonk.collider.gameObject.tag != tag_MagenticEnvironment &&
                 !Physics.Raycast(position, -direction, scale))
        {
            return RollType.bonk_flat;
        }
        else if(Physics.Raycast(position, direction, scale) && 
                Physics.Raycast(position, Vector3.up, scale) &&
                !Physics.Raycast(position, -direction, scale))
        {
            return RollType.bonk_flat;
        }

        ////////////// HEAD BONK //////////////
        // IS NOT cube direction 1
        // &&
        // IS cube direction 1 + up 1
        // &&
        // IS NOT cube dirction -1
        // &&
        // IS NOT cube up 1
        else if (!Physics.Raycast(position, direction, scale) && 
                 Physics.Raycast(position + Vector3.up * scale, direction, scale) &&
                 !Physics.Raycast(position, -direction, scale) &&
                 !Physics.Raycast(position, Vector3.up, scale))
        {
            return RollType.bonk_head;
        }

        ////////////// STUCK //////////////
        //if cant bonk or head bonk because of cubes around it
        //bonk 1
        //bonk 2
        //headbonk 1
        else if (Physics.Raycast(position, direction, out hit_bonk, scale) &&
                 Physics.Raycast(position + Vector3.up * scale, direction, scale) &&
                 hit_bonk.collider.gameObject.tag != tag_MagenticEnvironment &&
                 Physics.Raycast(position, -direction, scale)
                 ||
                 Physics.Raycast(position, direction, scale) &&
                 Physics.Raycast(position, Vector3.up, scale) &&
                 Physics.Raycast(position, -direction, scale)
                 ||
                 !Physics.Raycast(position, direction, scale) &&
                 Physics.Raycast(position + Vector3.up * scale, direction, scale) &&
                 Physics.Raycast(position, -direction, scale) &&
                 Physics.Raycast(position, Vector3.up, scale))
        {
            return RollType.stuck;
        }

        ////////////// FLAT //////////////
        //IS NOT cube direction 1
        // &&
        //IS NOT cube up 1
        // &&
        //IS NOT cube direction 1 + up 1
        // &&
        //IS cube direction 1 + down 1
        else if(!Physics.Raycast(position, direction, scale) &&
                !Physics.Raycast(position, Vector3.up, scale) &&
                !Physics.Raycast(position + direction * scale, Vector3.up, scale) &&
                Physics.Raycast(position + direction * scale, Vector3.down, scale))
        {
            return RollType.flat;
        }
        else
        {
            return RollType.stuck;
        }

        #endregion

    }

    //calcualate the direction vector based on the camera
    #region Camera Relative Direction
    public Vector3 TranslateRelativeDirection(Vector3 direction)
    {
        //forward
        if(direction == Vector3.forward)
        {
            return RoundToClosestCardinalDirection(vc_transform.forward);
        }
        //back
        else if(direction == Vector3.back)
        {
            return RoundToClosestCardinalDirection(-vc_transform.forward);
        }
        //right
        else if(direction == Vector3.right)
        {
            return RoundToClosestCardinalDirection (vc_transform.right);
        }
        //left
        else if(direction == Vector3.left)
        {
            return RoundToClosestCardinalDirection (-vc_transform.right);
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

    #endregion
}
