using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleCube : MonoBehaviour
{
    [NonSerialized]
    public Vector3 scale;

    private void OnEnable()
    {
        scale = transform.localScale;
        //transform.localScale = Vector3.zero;
    }

}
