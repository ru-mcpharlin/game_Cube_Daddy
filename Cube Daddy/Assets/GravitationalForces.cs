using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GravitationalForces : MonoBehaviour
{
    [Header("Rigid Bodies")]
    [SerializeField] List<Rigidbody> rbs;
    [SerializeField] Rigidbody player_rb;

    [SerializeField] PlayerController player;

    [SerializeField] float G;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody[] rb_array = FindObjectsOfType<Rigidbody>();
        rbs = rb_array.OfType<Rigidbody>().ToList();

        player_rb = player.cubeDatas[player.cubes_index].GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        for(int i = 0; i < rbs.Count; i++)
        {
            if (rbs[i] == null || rbs[i] == player_rb)
            {
                rbs.Remove(rbs[i]);
                break;
            }

            Debug.Log("Adding force to: " + i);
            ApplyGravity(rbs[i], player_rb);
            ApplyGravity(player_rb, rbs[i]);
        }
    }

    void ApplyGravity(Rigidbody rb_current, Rigidbody rb_target)
    {
        Vector3 direction = rb_target.transform.position - rb_current.transform.position;
        float distance = direction.magnitude;

        // Avoid division by zero
        if (distance == 0f)
        {
            return;
        }

        // Calculate gravitational force magnitude
        float forceMagnitude = G * rb_current.mass * rb_target.mass / (distance * distance);

        // Apply force to the other rigidbody
        rb_current.AddForce(direction.normalized * forceMagnitude);
    }
}

/*// Gravitational constant
public float G = 6.674e-11f;

void FixedUpdate()
{
    Rigidbody[] rigidbodies = FindObjectsOfType<Rigidbody>();

    foreach (Rigidbody rb in rigidbodies)
    {
        if (rb != GetComponent<Rigidbody>())
        {
            ApplyGravity(rb);
        }
    }
}

void ApplyGravity(Rigidbody rb)
{
    Vector3 direction = transform.position - rb.transform.position;
    float distance = direction.magnitude;

    // Avoid division by zero
    if (distance == 0f)
    {
        return;
    }

    // Calculate gravitational force magnitude
    float forceMagnitude = G * rb.mass * GetComponent<Rigidbody>().mass / (distance * distance);

    // Apply force to the other rigidbody
    rb.AddForce(direction.normalized * forceMagnitude);
}*/