using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerBox : MonoBehaviour
{
    public UnityEvent action;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Current Cube")
        {
            action.Invoke();
        }
    }
}
