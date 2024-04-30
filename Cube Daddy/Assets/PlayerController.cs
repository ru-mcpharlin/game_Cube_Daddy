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
    [SerializeField] float scale;
    [SerializeField] float rollSpeed;

    public AnimationCurve rollCurve;


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
            StartCoroutine(Roll(Vector3.forward, 90f));
        }

        //back
        if(axis_vertical == -1)
        {
            StartCoroutine(Roll(Vector3.back, 90f));
        }

        //right
        if(axis_horizontal == 1)
        {
            StartCoroutine(Roll(Vector3.right, 90f));
        }

        //left
        if (axis_horizontal == -1)
        {
            StartCoroutine(Roll(Vector3.left, 90f));
        }
    }

    IEnumerator Roll(Vector3 direction, float totalAngle)
    {
        isMoving = true;

        float remainingAngle = totalAngle;
        float timer = 0;
        Vector3 rotationAnchor = transform.position + direction / 2 + Vector3.down / 2;
        Vector3 rotationAxis = Vector3.Cross(Vector3.up, direction);

        while(remainingAngle > 0)
        {

            float rotationAngle = Mathf.Min(rollCurve.Evaluate(timer) * rollSpeed, remainingAngle);
            transform.RotateAround(rotationAnchor, rotationAxis, rotationAngle);

            remainingAngle -= rotationAngle;
            timer += Time.deltaTime;
            yield return null;
        }

        isMoving = false;
    }
}
