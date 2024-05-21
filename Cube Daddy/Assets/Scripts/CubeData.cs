using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CubeData : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] SquashCubesScript squash;
    [Space]
    [SerializeField] public bool isCurrentCube;
    [SerializeField] public float scale;
    [Space]
    [SerializeField] public Transform missingPosition;
    [SerializeField] public GameObject incompleteMesh;
    [SerializeField] public GameObject completeMesh;
    [SerializeField] public GameObject teleportParticleSystem;
    [SerializeField] public ParticleSystem.EmissionModule em;
    [Space]
    [SerializeField] public UnityEvent mergeEvents;
    

    private void Awake()
    {
        squash = FindObjectOfType<SquashCubesScript>();
        player = FindObjectOfType<PlayerController>();
        em = teleportParticleSystem.GetComponent<ParticleSystem>().emission;

        SetTeleportParticleSystem(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (isCurrentCube)
        {
            squash.CheckCube(other);

            if(other.gameObject.layer == player.respawnLayer)
            {
                //respawn
                player.teleport.Teleport(player.cubeTransform, player.lastValidPosition, player.respawnTime);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isCurrentCube) squash.CheckCube(other);
    }

    public void SetMeshes(bool inputBool)
    {
        completeMesh.SetActive(inputBool);
    }

    public void SetTeleportParticleSystem(bool inputBool)
    {
        em.enabled = inputBool;
    }
}
