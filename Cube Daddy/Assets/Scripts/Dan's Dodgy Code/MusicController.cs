using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class MusicController : MonoBehaviour
{
    public AudioSource[] layer;
    public float[] volume;
    private int activeTrack;
    public AnimationCurve fadeIn;
    public float fadeinDuration;

    public void AddLayer()
    {
        if(activeTrack < layer.Length)
        {
            Tween.Volume(layer[activeTrack], volume[activeTrack], fadeinDuration, 0, fadeIn);
            activeTrack++;
        }
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown("space"))
        {
            AddLayer();
        }
    }
}
