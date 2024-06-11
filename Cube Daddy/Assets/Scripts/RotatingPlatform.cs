using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class RotatingPlatform : MonoBehaviour
{
    public AnimationCurve rotateTween;
    


    public void Rotate() 
    {
       Tween.Rotate(transform, new(0, 0, 90), Space.World, 1f, 0, rotateTween);
    }
}
