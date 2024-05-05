using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] UnityEvent unityEvent;
    [Space]
    [SerializeField] bool hasBeenTriggered;
    [SerializeField] Transform targetPosition;
    [Space]
    [SerializeField] GameObject button;
    [SerializeField] Transform triggeredButtonPosition;

    public void CheckIfTriggered(Vector3 playerPosition, float scale)
    {
        if (playerPosition == targetPosition.position && 
            scale == transform.localScale.x &&
            !hasBeenTriggered)
        {
            unityEvent.Invoke();
            button.transform.position = triggeredButtonPosition.position;
        }
    }
}
