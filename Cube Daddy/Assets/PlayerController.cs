using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    #region Variables
    [Header("Player Input")]
    [SerializeField] float axis_horizontal;
    [SerializeField] float axis_vertical;

    [Header("Movement")]
    [SerializeField] bool isMoving;
    [SerializeField] RollType rollType;
    [Space]
    [SerializeField] float remainingAngle;
    [SerializeField] Vector3 rotationAnchor;
    [SerializeField] Vector3 rotationAxis;
    [Space]
    [SerializeField] float scale;
    [SerializeField] float rollSpeed;
    [SerializeField] AnimationCurve rollCurve;

    [Header("Mesh")]
    [SerializeField] Transform cubeTransform;

    [Header("Camera")]
    [SerializeField] Transform vc_transform;
    [SerializeField] CinemachineVirtualCamera vc;
    [SerializeField] Transform cameraFollow;
    [SerializeField] float cameraTrackingSpeed;
    #endregion

    #region ENUM
    public enum RollType
    {
        bonk,
        head_bonk,
        flat,
        step_Up,
        step_Down,
        climb_Up,
        climb_Down,
    }
    #endregion

    #region Start
    // Start is called before the first frame update
    void Start()
    {
        
    }
    #endregion

    #region Update
    // Update is called once per frame
    void Update()
    {
        #region Player and Camera Position
        //update player game object position
        transform.position = cubeTransform.position;

        //camera position
        CameraFollow();
        #endregion

        #region Player Input
        //get player input
        axis_horizontal = Input.GetAxis("Horizontal");
        axis_vertical = Input.GetAxis("Vertical");
        #endregion

        #region Movement
        if (isMoving) return;

        //forward
        if (axis_vertical == 1)
        {
            StartCoroutine(Roll(Vector3.forward));
        }
        //back
        else if(axis_vertical == -1)
        {
            StartCoroutine(Roll(Vector3.back));
        }
        //right
        else if(axis_horizontal == 1)
        {
            StartCoroutine(Roll(Vector3.right));
        }
        //left
        else if (axis_horizontal == -1)
        {
            StartCoroutine(Roll(Vector3.left));
        }

        #endregion
    }
    #endregion

    #region Roll Coroutine
    //coroutine
    IEnumerator Roll(Vector3 direction)
    {
        //is moving = true
        isMoving = true;

        //translate relative direction;
        direction = TranslateRelativeDirection(direction);

        //figure out roll type
        rollType = CalculateRollType(direction);

        //set the roll type information
        #region Roll Type
        //get roll information
        switch (rollType)
        {
            //bonk
            case RollType.bonk:
                isMoving = false;
                yield break;

            //head bonk
            case RollType.head_bonk:
                isMoving = false;
                yield break;


            //flat
            case RollType.flat:
                remainingAngle = 90f;
                rotationAnchor = cubeTransform.position + direction / 2 + Vector3.down / 2;
                break;

            //step up
            case RollType.step_Up:
                remainingAngle = 180f;
                rotationAnchor = cubeTransform.position + direction / 2 + Vector3.up / 2;
                break;
            
            //step down
            case RollType.step_Down:
                remainingAngle = 180f;
                rotationAnchor = cubeTransform.position + direction / 2 + Vector3.down / 2;
                break;

            //climb up
            case RollType.climb_Up:
                remainingAngle = 90f;
                rotationAnchor = cubeTransform.position + direction / 2 + Vector3.up / 2;
                break;

            //climb down
            case RollType.climb_Down:
                remainingAngle = 90f;
                rotationAnchor = cubeTransform.position + -direction / 2 + Vector3.down / 2;
                break;
        }
        #endregion

        //execute roll rotation
        #region Rotation
        //////////////////////////////////////////////////////////////////////////////
        //find roll axis
        rotationAxis = Vector3.Cross(Vector3.up, direction);
        //reset timer
        float timer = 0;
        
        //while there is still angle to go
        while(remainingAngle > 0)
        {
            //calculate rotation angle
            //uses min so that it goes exactly to the angle specified
            float rotationAngle = Mathf.Min(rollCurve.Evaluate(timer) * rollSpeed, remainingAngle);

            //rotation
            cubeTransform.RotateAround(rotationAnchor, rotationAxis, rotationAngle);

            //decrement remaing angle by the amount rotated
            remainingAngle -= rotationAngle;

            //increment timer
            timer += Time.deltaTime;
            yield return null;
        }
        ///////////////////////////////////////////////////////////////////////////////
        #endregion

        //fix round errors
        #region Round Errors
        cubeTransform.rotation = Quaternion.Euler(Mathf.Round(cubeTransform.rotation.eulerAngles.x), Mathf.Round(cubeTransform.rotation.eulerAngles.y), Mathf.Round(cubeTransform.rotation.eulerAngles.z));
        cubeTransform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
        #endregion

        #region fall
        //fall
        if(!Physics.Raycast(cubeTransform.position, Vector3.down, scale))
        {
            timer = 0;

            while(!Physics.Raycast(cubeTransform.position, Vector3.down, scale))
            {
                cubeTransform.position += Vector3.down * scale;
            }
        }




        #endregion


        //set isMoving to false
        isMoving = false;
    }

    //calculate roll type
    private RollType CalculateRollType(Vector3 direction)
    {
        //step up
        if (Physics.Raycast(cubeTransform.position, direction, scale) &&
            !Physics.Raycast(cubeTransform.position + Vector3.up * scale, direction, scale) &&
            !Physics.Raycast(cubeTransform.position, Vector3.up * scale, scale))
        {
            return RollType.step_Up;
        }
        //step down
        else if(!Physics.Raycast(cubeTransform.position, direction, scale) &&
            !Physics.Raycast(cubeTransform.position + Vector3.down * scale, direction, scale))
        {
            return RollType.step_Down;
        }
        //bonk
        else if(Physics.Raycast(cubeTransform.position, direction, scale) && Physics.Raycast(cubeTransform.position + Vector3.up * scale, direction, scale) ||
            Physics.Raycast(cubeTransform.position, direction, scale) && Physics.Raycast(cubeTransform.position, Vector3.up * scale, scale))
        {
            return RollType.bonk;
        }
        //head bonk
        else if(!Physics.Raycast(cubeTransform.position, direction, scale) && Physics.Raycast(cubeTransform.position + Vector3.up * scale, direction, scale))
        {
            return RollType.head_bonk;
        }
        //flat
        else
        {
            return RollType.flat;
        }


        
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

    //camera methods
    #region Camera
    public void CameraFollow()
    {
        if(Mathf.Abs(cameraFollow.position.y) - Mathf.Abs(cubeTransform.position.y) >= 1f)
        {
            cameraFollow.position = Vector3.MoveTowards(cameraFollow.position, cubeTransform.position, Time.deltaTime * cameraTrackingSpeed);
        }
        else
        {
            cameraFollow.position = new Vector3(cubeTransform.position.x, cameraFollow.position.y, cubeTransform.position.z);
        }
    }
    #endregion
}
