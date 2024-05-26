using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Pixelplacement;
using Unity.IO.LowLevel.Unsafe;

public class PPGlobalControl : MonoBehaviour
{
    public Volume[] volumes;
    public AnimationCurve blendTween;
    public float blendDuration;
    public int currentVolumeIndex;
    public int newVolumeIndex;

    private void OnEnable()
    {
        volumes = GetComponentsInChildren<Volume>();
        foreach (Volume vol in volumes)
        {
            vol.priority = 0;
        }
    }



    public void ActivateVolume(int volumeIndex)
    {
        Debug.Log("Volume Change");
        newVolumeIndex = volumeIndex;
        foreach (Volume vol in volumes)
        {
            vol.priority = 0;
        }
        volumes[newVolumeIndex].weight = 1;
        volumes[newVolumeIndex].priority = 1;

        //Tween.Value(0f, 1f, WeightChangeUpdate, blendDuration, 0f, blendTween, Tween.LoopType.None, null , WeightChangeComplete);
    }

    private void WeightChangeUpdate(float weight)
    {
        volumes[newVolumeIndex].weight = weight;
        /*
        if (currentVolumeIndex != -1)
        {
            volumes[currentVolumeIndex].weight = 1 - weight;
        }
        */
        
    }

    private void WeightChangeComplete()
    {
        volumes[currentVolumeIndex].priority = 0;
        currentVolumeIndex = newVolumeIndex;
        newVolumeIndex = -1;
    }
}
