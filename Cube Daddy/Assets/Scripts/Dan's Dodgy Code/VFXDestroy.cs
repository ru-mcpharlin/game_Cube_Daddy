using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXDestroy : MonoBehaviour
{
    public float lifeLength;

    private void OnEnable()
    {
        Invoke("DestroySelf", lifeLength);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

}
