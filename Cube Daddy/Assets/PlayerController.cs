using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float axis_horizontal;
    [SerializeField] float axis_vertical;
    [Space]
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



    

    public enum RollType
    {
        flat,
        step_Up,
        step_Down,
        climb_Up,
        climb_Down,
    }



    private void Awake()
    {
        DOTween.Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = cubeTransform.position;

        CameraFollow();

        axis_horizontal = Input.GetAxis("Horizontal");
        axis_vertical = Input.GetAxis("Vertical");

        if (isMoving) return;

        //get camera relative movement vectors

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
    }

    IEnumerator Roll(Vector3 direction)
    {
        isMoving = true;

        //translate relative direction;
        direction = TranslateRelativeDirection(direction);

        //figure out roll type


        //if cannot roll break

        switch (rollType)
        {
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

        rotationAxis = Vector3.Cross(Vector3.up, direction);
        float timer = 0;
        
        while(remainingAngle > 0)
        {
            float rotationAngle = Mathf.Min(rollCurve.Evaluate(timer) * rollSpeed, remainingAngle);
            cubeTransform.RotateAround(rotationAnchor, rotationAxis, rotationAngle);

            remainingAngle -= rotationAngle;
            timer += Time.deltaTime;
            yield return null;
        }

        cubeTransform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
        isMoving = false;
    }

    public Vector3 TranslateRelativeDirection(Vector3 direction)
    {
        //forward
        if(direction == Vector3.forward)
        {
            return RoundToClosestCardinalDirection(vc_transform.forward);
        }
        else if(direction == Vector3.back)
        {
            return RoundToClosestCardinalDirection(-vc_transform.forward);
        }
        else if(direction == Vector3.right)
        {
            return RoundToClosestCardinalDirection (vc_transform.right);
        }
        else if(direction == Vector3.left)
        {
            return RoundToClosestCardinalDirection (-vc_transform.right);
        }
        else
        {
            return Vector3.zero;
        }

       
    }

    public Vector3 RoundToClosestCardinalDirection(Vector3 inputDirection)
    {
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

        Debug.Log(output);
        return output;

    }

    public void CameraFollow()
    {
        if(Mathf.Abs(cameraFollow.position.y) - Mathf.Abs(cubeTransform.position.y) <= 1f)
        {
            cameraFollow.position = Vector3.MoveTowards(cameraFollow.position, cubeTransform.position, Time.deltaTime * cameraTrackingSpeed);
        }
        else
        {
            cameraFollow.position = new Vector3(cubeTransform.position.x, cameraFollow.position.y, cubeTransform.position.z);
        }
    }
}
