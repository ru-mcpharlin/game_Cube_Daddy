using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using Pixelplacement;
using Cinemachine;

public class TitleController : MonoBehaviour
{
    public TitleCube[] cubes;
    public int maxIndex;
    public PlayerController controller;
    public Transform startPosition;
    public AnimationCurve fallTween;
    public float fallTime;
    public CinemachineVirtualCamera titleCam, fallCam;
    public UnityEvent fallEvent;
    private TitleCube playerCube;
    private bool fallen;

    IEnumerator CubeIn()
    {
        yield return new WaitUntil(() => SceneManager.GetActiveScene().isLoaded);
        
        for (int i = 0; i <= maxIndex; i++)
        {
            foreach (TitleCube cube in cubes)
            {
                if (cube.orderInTitle == i)
                {
                    cube.Activate();
                }

                if (cube.isPlayerCube)
                {
                    playerCube = cube;
                }
            }
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(2);
        //controller.PlayAnimation_CannotMove("Merge");
        
    }

    private void OnEnable()
    {
        StartCoroutine(CubeIn());
    }

    private void Awake()
    {
        controller = FindObjectOfType<PlayerController>();
        controller.canMove = false;
        
    }

    private void Fall()
    {
        Tween.Position(playerCube.transform, startPosition.position, fallTime, 0, fallTween, Tween.LoopType.None, null, FinishFall);
        fallEvent.Invoke();
        //fallCam.Priority = 1;
        //titleCam.Priority = 0;
        fallen = true;
    }

    private void FinishFall()
    {
        playerCube.transform.position = startPosition.position;
    }

    private void Update()
    {
        if (Input.anyKey)
        {
            if (!fallen)
            {
                Fall();
            }
        }
    }

}
