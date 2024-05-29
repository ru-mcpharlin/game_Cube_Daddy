using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Pixelplacement;

public class Pellet : MonoBehaviour
{
    public int orderInGroup;
    public AnimationCurve positionCurve, rotationCurve, scaleCurve;
    public GameObject collectVFX;
    private float delay = 0.2f;
    private float animDuration = 0.75f;
    private float animRepeatDelay = 1.75f;
    private Vector3 scale;

    private void Start()
    {
        scale = transform.parent.localScale;
        float wait = orderInGroup * delay;
        Invoke("StartAnimation", wait);
    }

    private void StartAnimation()
    {
        Tween.LocalPosition(transform, new Vector3(0, 1, 0), animDuration, animRepeatDelay, positionCurve, Tween.LoopType.Loop);
        Tween.LocalRotation(transform, Quaternion.Euler(0, 180, 0), animDuration, animRepeatDelay, rotationCurve, Tween.LoopType.Loop);
        Tween.LocalScale(transform, new(0.25f, 1.5f, 0.25f), animDuration, animRepeatDelay, scaleCurve, Tween.LoopType.Loop);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Current Cube")
        {
            CollectPellet();
        }
    }

    private void CollectPellet()
    {
        GameObject vfx = Instantiate(collectVFX, transform.position, Quaternion.Euler(-90, 0, 0));
        vfx.transform.localScale = scale;
        var part = vfx.GetComponent<ParticleSystem>();
        var main = part.main;
        main.gravityModifier = scale.x * 5f;
        Destroy(transform.parent.gameObject);
    }
}
