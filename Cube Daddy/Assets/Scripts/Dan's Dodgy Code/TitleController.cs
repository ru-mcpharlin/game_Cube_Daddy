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
    public CinemachineVirtualCamera[] cams;
    public UnityEvent fallEvent;
    private TitleCube playerCube;
    private bool fallen, shakeDecay;
    private float camShakeIntensity;
    public float shakeIntensity;


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
        StartCoroutine(FallRoutine()); 
    }

    IEnumerator FallRoutine()
    {
        fallen = true;
        controller.PlayAnimation_CannotMove("ShakeLoose");
        camShakeIntensity = shakeIntensity;
        foreach (CinemachineVirtualCamera cam in cams)
        {
            cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = camShakeIntensity;
        }
        shakeDecay = true;

        yield return new WaitForSeconds(1);

        
        Tween.Position(playerCube.transform, startPosition.position, fallTime, 0, fallTween, Tween.LoopType.None, null, FinishFall);

        yield return new WaitForSeconds(1);

        fallEvent.Invoke();
    }

    private void FinishFall()
    {
        playerCube.transform.position = startPosition.position;
        controller.PlayAnimation_CannotMove("Splat");
        camShakeIntensity = shakeIntensity;
        foreach (CinemachineVirtualCamera cam in cams)
        {
            cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = camShakeIntensity;
        }
        shakeDecay = true;
    }

    private void FixedUpdate()
    {
        if (Input.anyKey)
        {
            if (!fallen)
            {
                Fall();
            }
        }

        if (shakeDecay)
        {
            if (camShakeIntensity > 0)
            {
                camShakeIntensity -= 0.1f;
                if (camShakeIntensity < 0)
                {
                    camShakeIntensity = 0;
                    shakeDecay = false;
                }
                foreach (CinemachineVirtualCamera cam in cams)
                {
                    cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = camShakeIntensity;
                }
            }
        }
    }

}
