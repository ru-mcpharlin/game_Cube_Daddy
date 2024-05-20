using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CubeData : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] SquashCubesScript squash;
    [SerializeField] public bool isCurrentCube;
    [SerializeField] public float scale;
    [SerializeField] public Transform missingPosition;
    [SerializeField] public GameObject incompleteMesh;
    [SerializeField] public GameObject completeMesh;
    [SerializeField] public UnityEvent mergeEvents;
    

    private void Awake()
    {
        squash = FindObjectOfType<SquashCubesScript>();
        player = FindObjectOfType<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCurrentCube)
        {
            squash.CheckCube(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isCurrentCube) squash.CheckCube(other);
    }



    private void OnCollision(Collision collision)
    {
        if (collision.gameObject.layer == player.respawnLayer)
        {
            Debug.Log("Respawn");
        }
    }



}
