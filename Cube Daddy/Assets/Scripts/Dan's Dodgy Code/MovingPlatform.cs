using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 newPosition;
    public float moveDuration;
    public AnimationCurve moveCurve;
    private Vector3 startPosition;
    private bool atStartPosition = true, moving = false;

    private void OnEnable()
    {
        startPosition = transform.position;
    }

    public void Activate()
    {
        Debug.Log("Activating");
        if (moving)
        {
            return;
        }

        if (atStartPosition)
        {
            moving = true;
            atStartPosition = false;
            Tween.Position(transform, newPosition, moveDuration, 0, moveCurve);
            Invoke("FinishMove", moveDuration);
        }
        else
        {
            moving = true;
            atStartPosition = true;
            Tween.Position(transform, startPosition, moveDuration, 0, moveCurve);
            Invoke("FinishMove", moveDuration);
        }
    }

    private void FinishMove()
    {
        moving = false;
    }
}
