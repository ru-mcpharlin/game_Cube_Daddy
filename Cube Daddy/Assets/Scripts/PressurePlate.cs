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
    [SerializeField] bool needToHaveCollectedPellets;
    [SerializeField] bool hasCollectedPellets;
    [SerializeField] List<GameObject> pellets;
    [Space]
    [SerializeField] GameObject button;
    [SerializeField] Transform triggeredButtonPosition;

    public void CheckIfTriggered(Vector3 playerPosition, float scale)
    {
        int index = 0;
        foreach(GameObject pellet in pellets)
        {
            if(pellet != null)
            {
                index++;
            }
        }    

        if(index == 0)
        {
            hasCollectedPellets = true;
        }



        if (playerPosition == targetPosition.position && 
            scale == transform.localScale.x &&
            !hasBeenTriggered)
        {
            if (needToHaveCollectedPellets)
            {
                if (hasCollectedPellets)
                {
                    TriggerPressurePad();
                }
            }
            else
            {
                TriggerPressurePad();
            }

            
        }
    }

    private void TriggerPressurePad()
    {
        hasBeenTriggered = true;
        unityEvent.Invoke();
        button.transform.position = triggeredButtonPosition.position;
    }
}
