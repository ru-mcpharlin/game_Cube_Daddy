using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class LenseOfTruth : MonoBehaviour
{
    public AnimationCurve revealTween;
    public float revealDuration;
    public float lenseSize;

    private SphereCollider sphere;
    private PlayerController playerController;

    public static int PosID = Shader.PropertyToID("_Position");
    public static int SizeID = Shader.PropertyToID("_Size");
    public float cutoutSize;
    public Camera cam;
    public Material[] wallMaterials;

    private float size;


    private void OnEnable()
    {
        playerController = GameObject.FindAnyObjectByType<PlayerController>();
    }

    public void ActivateLense(float cutoutScale)
    {
        float newscale = lenseSize * playerController.currentScale;
        Tween.LocalScale(transform, new(newscale, newscale, newscale), revealDuration, 0, revealTween);
        Tween.Value(0, cutoutScale, UpdateCutoutScale, revealDuration, 0, revealTween);
    }

    private void UpdateCutoutScale(float cutoutScale)
    {
        cutoutSize = cutoutScale;
    }


    public void DeactivateLense()
    {
        float newscale = lenseSize * playerController.currentScale;
        Tween.LocalScale(transform, Vector3.zero, revealDuration, 0, revealTween);
        Tween.Value(size, 0, UpdateCutoutScale, revealDuration, 0, revealTween);
    }

    private void Update()
    {
        size = cutoutSize;

        var view = cam.WorldToViewportPoint(transform.position);

        foreach (Material mat in wallMaterials)
        {
            mat.SetFloat(SizeID, size);

            mat.SetVector(PosID, view);
        }
    }

}
