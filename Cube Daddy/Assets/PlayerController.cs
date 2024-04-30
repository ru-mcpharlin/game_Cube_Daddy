using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float axis_horizontal;
    [SerializeField] float axis_vertical;
    [Space]
    [SerializeField] bool isMoving;
    [SerializeField] Vector3 targetVector3;
    [Space]
    [SerializeField] AnimationCurve cubeRotation;
    [SerializeField] AnimationCurve cubePosition_1;
    [SerializeField] AnimationCurve cubePosition_2;

    [SerializeField] AnimationCurve cubePosition_vertical;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        axis_horizontal = Input.GetAxis("Horizontal");
        axis_vertical = Input.GetAxis("Vertical");

        if (axis_vertical == 1 && !isMoving)
        {
            //rotation
            Tween.LocalRotation(transform, Vector3.zero, new Vector3(0, 0, -90), 1, 0, Tween.EaseBounce, Tween.LoopType.None, HandleTweenStart, HandleTweenFinish);
            //horizontal movement
            Tween.LocalPosition(transform, transform.position + -Vector3.left, 0.5f, 0, cubePosition_1);

        }

        if(axis_vertical == -1 && !isMoving)
        {
            //rotation
            Tween.LocalRotation(transform, Vector3.zero, new Vector3(0, 0, 90), 1, 0, cubeRotation, Tween.LoopType.None, HandleTweenStart, HandleTweenFinish);
            //horizontal movement
            Tween.Position(transform, transform.position + (Vector3.left / 2) + (Vector3.up * 0.25f), 0.5f, 0, cubePosition_1);
            Tween.Position(transform, transform.position + (Vector3.left), 0.5f, 0.5f, cubePosition_2);
            /*//vertical movement
            Tween.LocalPosition(transform, transform.position + Vector3.up, 1, 0, cubePosition_vertical);*/
        }

        if(axis_horizontal == 1 && !isMoving)
        {
            //rotation
            Tween.LocalRotation(transform, Vector3.zero, new Vector3(-90, 0, 0), 1, 0, cubeRotation, Tween.LoopType.None, HandleTweenStart, HandleTweenFinish);
            //horizontal movement
            Tween.LocalPosition(transform, transform.position + (-Vector3.forward / 2) + (Vector3.up * 0.25f), 0.5f, 0, cubePosition_1);
            Tween.LocalPosition(transform, transform.position + (-Vector3.forward), 0.5f, 0.5f, cubePosition_2);
            /*//vertical movement
            Tween.LocalPosition(transform, transform.position + Vector3.up, 1, 0, cubePosition_vertical);*/
        }

        if (axis_horizontal == -1 && !isMoving)
        {
            //rotation
            Tween.LocalRotation(transform, Vector3.zero, new Vector3(90, 0, 0), 1, 0, cubeRotation, Tween.LoopType.None, HandleTweenStart, HandleTweenFinish);
            //horizontal movement
            Tween.Position(transform, transform.position + (Vector3.forward / 2) + (Vector3.up * 0.25f), 0.5f, 0, cubePosition_1);
            Tween.Position(transform, transform.position + (Vector3.forward), 0.5f, 0.5f, cubePosition_1);

            /*//vertical movement
            Tween.LocalPosition(transform, transform.position + Vector3.up, 1, 0, cubePosition_vertical);*/
        }


    }

    void HandleTweenStart()
    {
        isMoving = true;
    }

    void HandleTweenFinish()
    {
        Debug.Log("Finish");
        isMoving = false;
    }
}
