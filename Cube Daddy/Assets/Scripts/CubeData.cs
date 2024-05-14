using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeData : MonoBehaviour
{
    [SerializeField] SquashCubesScript squash;
    [SerializeField] public bool isCurrentCube;
    [SerializeField] public float scale;
    [SerializeField] public Transform missingPosition;
    [SerializeField] public GameObject incompleteMesh;
    [SerializeField] public GameObject completeMesh;

    private void Start()
    {
        squash = FindObjectOfType<SquashCubesScript>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(isCurrentCube) squash.CheckCube(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (isCurrentCube) squash.CheckCube(other);
    }



}
