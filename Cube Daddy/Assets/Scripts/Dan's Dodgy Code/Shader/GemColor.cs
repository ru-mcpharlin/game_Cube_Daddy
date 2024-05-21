using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemColor : MonoBehaviour
{
    public static int PlayerPos = Shader.PropertyToID("_PlayerPosition");
    public Material gemMaterial;

    // Update is called once per frame
    void Update()
    {
        gemMaterial.SetVector(PlayerPos, transform.position);
    }
}
