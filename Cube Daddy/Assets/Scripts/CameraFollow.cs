using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] public Transform currentCubeTransform;
    [SerializeField] public float speed;

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, currentCubeTransform.position, Time.deltaTime * speed);
    }
}
