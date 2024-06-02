using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarScript : MonoBehaviour
{

    [SerializeField] PlayerController player;
    [SerializeField] CameraController cameraController;

    [SerializeField] int starCamIndex;
    [SerializeField] Rigidbody rb;


    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        cameraController = FindObjectOfType<CameraController>();
        rb = GetComponent<Rigidbody>();

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Star Cam Transition");
            cameraController.SetCamera6Index(starCamIndex);
            player.StarMerge(transform);
            rb.constraints = RigidbodyConstraints.None;
        }
    }
}
