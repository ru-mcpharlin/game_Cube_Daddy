using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class TeleportScript : MonoBehaviour
{
    [SerializeField] PlayerController player;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
    }


    public void Teleport(Transform cubeTransform, Vector3 newPos, float timer)
    {
        Tween.Position(cubeTransform, newPos, timer, 0, Tween.EaseInOut, Tween.LoopType.None, TeleportStart, TeleportEnd);
        player.TeleportStart();
    }

    public void TeleportStart()
    {
        
    }

    public void TeleportEnd()
    {
        player.TeleportEnd();
    }
}
