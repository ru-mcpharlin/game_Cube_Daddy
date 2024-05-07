using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class FormingPlatform : MonoBehaviour
{
    public Transform[] segment;
    public Vector3 positionOffset;
    public float segmentDelay;
    public float tweenDuration;
    public AnimationCurve movementTween;
    
    // Start is called before the first frame update
    void OnEnable()
    {
        for (int i = 0; i < segment.Length; i++)
        {
            segment[i].position += positionOffset;
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
            Tween.Position(segment[i], newpos, tweenDuration, i * segmentDelay, movementTween);
            yield return new WaitForSeconds(segmentDelay);
        }
        

    }

}
