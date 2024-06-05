using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidScript : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] Rigidbody rb;
    [SerializeField] string playerTag;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        rb = GetComponent<Rigidbody>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player" && rb != null)
        {
            transform.SetParent(player.cubeDatas[player.cubes_index].completeMesh.transform);
            player.cubeDatas[player.cubes_index].GetComponent<Rigidbody>().mass += rb.mass;
            Destroy(rb);
            transform.tag = "Player";
        }
    }
}
