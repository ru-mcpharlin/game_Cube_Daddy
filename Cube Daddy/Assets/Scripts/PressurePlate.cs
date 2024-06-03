using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] UnityEvent unityEvent;
    [Space]
    [SerializeField] public float distanceThreshold;
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
    [Space]
    [SerializeField] GameObject activateVFX;
    [SerializeField] Material activatedMaterial;
    [SerializeField] Renderer buttonRenderer;

    public void CheckIfTriggered(Vector3 playerPosition, float scale)
    {
        if (!hasCollectedPellets)
        {
            int index = 0;
            foreach (GameObject pellet in pellets)
            {
                if (pellet != null)
                {
                    index++;
                }
            }

            if (index == 0)
            {
                StartCoroutine(ActivatePressurePad());
            }
        }

        if (Vector3.Distance(playerPosition, targetPosition.position) <= distanceThreshold * scale &&
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

    IEnumerator ActivatePressurePad()
    {
        hasCollectedPellets = true;
        GameObject vfx = Instantiate(activateVFX, new(transform.position.x, transform.position.y + 0.5f, transform.position.z), Quaternion.Euler(new(-90, 0, 0)));
        ParticleSystem[] emmiters = vfx.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem emmit in emmiters)
        {
            emmit.transform.localScale = transform.localScale;
        }
        
        vfx.transform.localScale = transform.localScale;
        yield return new WaitForSeconds(0.5f);
        buttonRenderer.material = activatedMaterial;
    }

    private void TriggerPressurePad()
    {
        hasBeenTriggered = true;
        unityEvent.Invoke();
        button.transform.position = triggeredButtonPosition.position;
    }
}
