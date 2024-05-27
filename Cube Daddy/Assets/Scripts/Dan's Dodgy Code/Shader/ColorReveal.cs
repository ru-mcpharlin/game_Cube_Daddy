using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class ColorReveal : MonoBehaviour
{
    public Material mat;
    public AnimationCurve tweenCurve;

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            RevealColor();
        }
    }

    public void RevealColor()
    {
        Debug.Log("Revealing");
        Tween.ShaderFloat(mat, "_Reveal_Amount", 1, 1, 0, tweenCurve);
    }
}
