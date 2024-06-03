using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using System.Data;

public class TitleCube : MonoBehaviour
{
    public int orderInTitle;
    public AnimationCurve ActivateTween, TwistTween, BounceTween;

    private Vector3 initialRotation;

    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        initialRotation = transform.rotation.ToEuler();
        transform.rotation = Quaternion.Euler(new(transform.rotation.x, transform.rotation.y - 180, transform.rotation.z));
        Invoke("Activate", orderInTitle * 0.12f);
    }

    private void Activate()
    {
        Tween.LocalScale(transform, Vector3.one, 0.75f, 0, ActivateTween);
        Tween.LocalRotation(transform, initialRotation, 0.75f, 0, ActivateTween, Tween.LoopType.None, null, Twist);
    }

    private void Twist()
    {
        Vector3 rot = new(transform.rotation.x, transform.rotation.y + 180, transform.rotation.z);
        Vector3 pos = new(transform.position.x, transform.position.y + 1, transform.position.z);
        Tween.LocalRotation(transform, rot, 0.4f, 1.5f, TwistTween);
        Tween.Position(transform, pos, 0.4f, 1.5f, BounceTween);
    }
}
