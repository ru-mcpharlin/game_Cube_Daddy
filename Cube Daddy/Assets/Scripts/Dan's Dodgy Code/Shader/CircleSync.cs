using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class CircleSync : MonoBehaviour
{
    public static int PosID = Shader.PropertyToID("_Position");
    public static int SizeID = Shader.PropertyToID("_Size");
    public float cutoutSize;
    public Material[] wallMaterials;
    public LayerMask mask;
    public Camera cam;

    private float size;


    // Update is called once per frame
    void Update()
    {
        var dir = cam.transform.position - transform.position;
        var ray = new Ray(transform.position, dir.normalized);
        //float size;

        if (Physics.Raycast(ray, 3000, mask))
        {
            //size = iTween.FloatUpdate(size, cutoutSize, 3);
            size = cutoutSize;
        }
        else
        {
            //size = iTween.FloatUpdate(size, 0, 5);
            size = 0;
        }

        var view = cam.WorldToViewportPoint(transform.position);

        foreach (Material mat in wallMaterials)
        {
            mat.SetFloat(SizeID, size);

            mat.SetVector(PosID, view);
        }
        //wallMaterials[i].SetFloat(SizeID, size);

        
        //wallMaterials[i].SetVector(PosID, view);

    }

}
