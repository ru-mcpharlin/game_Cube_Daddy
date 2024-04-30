using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

    public AnimationCurve rollCurve;

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
        
        axis_horizontal = Input.GetAxis("Horizontal");
        axis_vertical = Input.GetAxis("Vertical");

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
    }

    IEnumerator Roll(Vector3 direction)
    {
        isMoving = true;

        //figure out roll type


        //if cannot roll break

        switch (rollType)
        {
            //flat
            case RollType.flat:
                remainingAngle = 90f;
                rotationAnchor = transform.position + direction / 2 + Vector3.down / 2;
                break;

            //step up
            case RollType.step_Up:
                remainingAngle = 180f;
                rotationAnchor = transform.position + direction / 2 + Vector3.up / 2;
                break;
            
            //step down
            case RollType.step_Down:
                remainingAngle = 180f;
                rotationAnchor = transform.position + direction / 2 + Vector3.down / 2;
                break;

            //climb up
            case RollType.climb_Up:
                remainingAngle = 90f;
                rotationAnchor = transform.position + direction / 2 + Vector3.up / 2;
                break;

            //climb down
            case RollType.climb_Down:
                remainingAngle = 90f;
                rotationAnchor = transform.position + -direction / 2 + Vector3.down / 2;
                break;
        }

        rotationAxis = Vector3.Cross(Vector3.up, direction);
        float timer = 0;
        
        

        while(remainingAngle > 0)
        {
            float rotationAngle = Mathf.Min(rollCurve.Evaluate(timer) * rollSpeed, remainingAngle);
            transform.RotateAround(rotationAnchor, rotationAxis, rotationAngle);

            remainingAngle -= rotationAngle;
            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
        isMoving = false;
    }
}
