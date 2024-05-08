using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class FormingPlatform : MonoBehaviour
{
    public Transform[] segment;
    public Vector3 positionOffset;
    public float segmentDelay;
    public float tweenDuration, loopTweenDuration;
    public AnimationCurve movementTween, loopTween;
    public AutoMode autoMode;   
    public enum AutoMode
    {
        none,
        loop
    }
    
    // Start is called before the first frame update
    void OnEnable()
    {
        for (int i = 0; i < segment.Length; i++)
        {
            segment[i].position += positionOffset;
        }

        if (autoMode == AutoMode.loop) 
        {
            StartCoroutine(Loop()); 
        }
    }

    IEnumerator Loop()
    {
        for (int i = 0; i < segment.Length; i++)
        {
            Vector3 newpos = segment[i].position - positionOffset;
            Tween.Position(segment[i], newpos, loopTweenDuration, segmentDelay, loopTween, Tween.LoopType.Loop);
            yield return new WaitForSeconds(segmentDelay);
        }
    }

    public void Activate()
    {
        StartCoroutine(ActivateSegments());
    }

    IEnumerator ActivateSegments()
    {

        for (int i = 0; i < segment.Length; i++)
        {
            Vector3 newpos = segment[i].position - positionOffset;
            Tween.Position(segment[i], newpos, tweenDuration, segmentDelay, movementTween);
            yield return new WaitForSeconds(segmentDelay);
        }
    }

}
