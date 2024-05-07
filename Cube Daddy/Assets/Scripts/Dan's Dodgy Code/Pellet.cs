using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Pixelplacement;

public class Pellet : MonoBehaviour
{
    public int orderInGroup;
    public AnimationCurve positionCurve, rotationCurve, scaleCurve;
    private float delay = 0.2f;
    private float animDuration = 0.75f;
    private float animRepeatDelay = 1.75f;
    
    private Animator animator;

    private void Start()
    {
        float wait = orderInGroup * delay;
        Invoke("StartAnimation", wait);
    }

    private void StartAnimation()
    {
        Tween.LocalPosition(transform, new Vector3(0, 1, 0), animDuration, animRepeatDelay, positionCurve, Tween.LoopType.Loop);
        Tween.LocalRotation(transform, Quaternion.Euler(0, 180, 0), animDuration, animRepeatDelay, rotationCurve, Tween.LoopType.Loop);
        Tween.LocalScale(transform, new(0.25f, 2, 0.25f), animDuration, animRepeatDelay, scaleCurve, Tween.LoopType.Loop);
    }
}
