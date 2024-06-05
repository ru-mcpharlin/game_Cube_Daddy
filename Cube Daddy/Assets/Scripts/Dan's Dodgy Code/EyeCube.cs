using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeCube : MonoBehaviour
{
    public Material mat;
    public Texture blinkingSpriteSheet, lookingSpriteSheet;
    public float changeTime;
    public GameObject plane;

    public void Activate(float delay)
    {
        Invoke("OpenEye", delay);
    }

    private void OpenEye()
    {
        plane.SetActive(true);
        mat.SetVector("_Cells", new(7, 1));
        mat.SetTexture("_SpriteSheet", blinkingSpriteSheet);

        Invoke("LookAround", changeTime);    
    }

    private void LookAround()
    {
        mat.SetVector("_Cells", new(8, 1));
        mat.SetTexture("_SpriteSheet", lookingSpriteSheet);
    }
}
