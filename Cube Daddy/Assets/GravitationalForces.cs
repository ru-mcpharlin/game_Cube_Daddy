using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationalForces : MonoBehaviour
{
    [Header("Rigid Bpdies")]
    [SerializeField] Rigidbody[] rbs;


    // Start is called before the first frame update
    void Start()
    {
        rbs = FindObjectsOfType<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
