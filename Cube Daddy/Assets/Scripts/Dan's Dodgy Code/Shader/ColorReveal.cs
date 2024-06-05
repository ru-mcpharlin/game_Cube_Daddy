using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class ColorReveal : MonoBehaviour
{
    public Material revealMat, completeMat;
    public AnimationCurve tweenCurve;
    public Renderer fullCube;

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            //RevealColor();
        }
    }

    public void RevealColor()
    {
        Tween.ShaderFloat(revealMat, "_Reveal_Amount", 1, 1, 0, tweenCurve, Tween.LoopType.None, null, FinishReveal);
    }

    private void FinishReveal()
    {
        fullCube.materials[0] = completeMat;
        Tween.ShaderFloat(revealMat, "_Reveal_Amount", 0, 0.1f, 0, tweenCurve);
    }
}
