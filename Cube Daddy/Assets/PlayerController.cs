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
    [SerializeField] bool isFalling;
    [SerializeField] RollType rollType;
    [Space]
    [SerializeField] float remainingAngle;
    [SerializeField] Vector3 rotationAnchor;
    [SerializeField] Vector3 rotationAxis;
    [Space]
    [SerializeField] float scale;
    [SerializeField] float rollSpeed;
    [SerializeField] AnimationCurve rollCurve;

    [Header("Cube")]
    [SerializeField] Transform cubeTransform;
    [SerializeField] Rigidbody rb;

    [Header("Camera")]
    [SerializeField] Transform vc_transform;
    [SerializeField] CinemachineVirtualCamera[] virtualCameras;
    [SerializeField] CameraFollow cameraFollow;
    [SerializeField] float cameraTrackingSpeed;

    [Header("Larger Cubes")]
    [SerializeField] int cubes_index;
    [SerializeField] List<CubeData> cubeData;

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
        #endregion

        #region Player Input
        //get player input
        axis_horizontal = Input.GetAxis("Horizontal");
        axis_vertical = Input.GetAxis("Vertical");

        #endregion

        #region Falling
        if (isFalling)
        {
            CheckForFallEnd();
        }

        #endregion

        #region Movement
        if (isMoving) return;
        if (isFalling) return;

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


    public void CheckForFallEnd()
    {
        Debug.Log(rb.velocity);
        if(rb.velocity == Vector3.zero)
        {
            isFalling = false;
            rb.isKinematic = true;
            FixFloatingPointErrors();
        }
    }

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
                rotationAnchor = cubeTransform.position + direction * scale / 2 + Vector3.down * scale / 2;
                break;

            //step up
            case RollType.step_Up:
                remainingAngle = 180f;
                rotationAnchor = cubeTransform.position + direction * scale / 2 + Vector3.up * scale / 2;
                break;

            //step down
            case RollType.step_Down:
                remainingAngle = 180f;
                rotationAnchor = cubeTransform.position + direction * scale / 2 + Vector3.down * scale / 2;
                break;

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
        while (remainingAngle > 0)
        {
            //calculate rotation angle
            //uses min so that it goes exactly to the angle specified
            float rotationAngle = Mathf.Min(rollCurve.Evaluate(timer) * rollSpeed / scale, remainingAngle);

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
        FixFloatingPointErrors();
        #endregion

        //falling behaviour
        #region Fall
        //fall
        Debug.DrawRay(cubeTransform.position, Vector3.down, Color.blue, scale);
        if (!Physics.Raycast(cubeTransform.position, Vector3.down, scale))
        {
            rb.isKinematic = false;
            isFalling = true;
            yield return new WaitForSeconds(0.1f);
        }
        #endregion

        //check that cube hasnt been completed
        #region check completed cube
        if (cubeTransform.position == cubeData[cubes_index].missingPosition.position)
        {
            //set current small cubes parent to be large cubes transform
            cubeTransform.gameObject.SetActive(false);

            //set cube transform to large cube transform
            cubeTransform = cubeData[cubes_index].transform;

            //update camera follow transform
            cameraFollow.currentCubeTransform = cubeData[cubes_index].transform;

            //set scale
            scale = cubeData[cubes_index].scale;

            //set active = false incomplete meshes
            cubeData[cubes_index].incompleteMesh.SetActive(false);

            //set active = true complete meshes
            cubeData[cubes_index].completeMesh.SetActive(true);

            //increment index
            if (cubes_index < cubeData.Count - 1)
            {
                cubes_index++;
            }

            //vc
            foreach (CinemachineVirtualCamera vc in virtualCameras)
            {
                vc.Priority = 0;
            }
            //set 
            virtualCameras[cubes_index].Priority = 1;
            vc_transform = virtualCameras[cubes_index].transform;
        }



        #endregion

        //set isMoving to false
        isMoving = false;
    }

    private void FixFloatingPointErrors()
    {
        cubeTransform.rotation = Quaternion.Euler(Mathf.Round(cubeTransform.rotation.eulerAngles.x), Mathf.Round(cubeTransform.rotation.eulerAngles.y), Mathf.Round(cubeTransform.rotation.eulerAngles.z));

        Vector3 positionVector = cubeTransform.position;
        positionVector = new Vector3(positionVector.x * 10, positionVector.y * 10, positionVector.z * 10);
        positionVector = new Vector3(Mathf.Round(positionVector.x), Mathf.Round(positionVector.y), Mathf.Round(positionVector.z));
        positionVector = new Vector3(positionVector.x / 10, positionVector.y / 10, positionVector.z / 10);
        cubeTransform.position = positionVector;
    }

    //calculate roll type
    private RollType CalculateRollType(Vector3 direction)
    {
        //step up
        /*Debug.DrawRay(cubeTransform.position, direction, Color.red, scale);
        Debug.DrawRay(cubeTransform.position + Vector3.up * scale, direction, Color.red, scale);
        Debug.DrawRay(cubeTransform.position, Vector3.up, Color.red, scale);*/
        //step down
        /*Debug.DrawRay(cubeTransform.position, direction, Color.green, scale);
        Debug.DrawRay(cubeTransform.position + direction * scale, Vector3.down, Color.green, scale);*/

        //step up
        //if IS cube forwards 1
        // &&
        //if IS NOT cube forwards 1 + up 1
        // &&
        //if IS NOT cube up 1
        if (Physics.Raycast(cubeTransform.position, direction, scale) &&
            !Physics.Raycast(cubeTransform.position + Vector3.up * scale, direction, scale) &&
            !Physics.Raycast(cubeTransform.position, Vector3.up, scale))
        {
            
            return RollType.step_Up;
        }

        //step down
        //if IS NOT cube forwards 1
        // &&
        //if IS NOT cube forwards 1 + down 1
        else if(!Physics.Raycast(cubeTransform.position, direction, scale) &&
                !Physics.Raycast(cubeTransform.position + direction * scale, Vector3.down, scale))
        {
            
            return RollType.step_Down;
        }

        //bonk
        //if IS cube forward 1
        // && 
        //if IS NOT cube forward 1 + up 1
        // OR
        //if IS cube forward 1
        // &&
        //if IS cube up 1
        else if(Physics.Raycast(cubeTransform.position, direction, scale) && Physics.Raycast(cubeTransform.position + Vector3.up * scale, direction, scale) ||
            Physics.Raycast(cubeTransform.position, direction, scale) && Physics.Raycast(cubeTransform.position, Vector3.up, scale))
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
}
