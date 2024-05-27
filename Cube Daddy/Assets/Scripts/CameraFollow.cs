using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] public PlayerController player;
    [SerializeField] public Transform currentCubeTransform;
    [SerializeField] public float speed;
    [SerializeField] public Vector3 velocity1;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
    }


    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, currentCubeTransform.position, ref velocity1, speed);
    }
}
