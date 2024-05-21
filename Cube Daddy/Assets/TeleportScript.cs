using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class TeleportScript : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] float delay;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
    }


    public void Teleport(Transform cubeTransform, Vector3 newPos, float timer)
    {
        player.TeleportStart();
        Tween.Position(cubeTransform, newPos, timer, delay, Tween.EaseInOut, Tween.LoopType.None, TeleportStart, TeleportEnd);
        
    }

    public void TeleportStart()
    {
        
    }

    public void TeleportEnd()
    {
        player.TeleportEnd();
    }
}
