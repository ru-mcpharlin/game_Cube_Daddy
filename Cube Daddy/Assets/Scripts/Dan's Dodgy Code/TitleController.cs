using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Pixelplacement;

public class TitleController : MonoBehaviour
{
    public TitleCube[] cubes;
    public int maxIndex;
    public PlayerController controller;
    public Transform startPosition;
    public AnimationCurve fallTween;

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
            }
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(2);
        controller.PlayAnimation_CannotMove("Merge");
        Tween.LocalPosition(transform, startPosition.position, 4, 0, fallTween);
        
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

    private void CupeUp()
    {
        StartCoroutine(CubeIn());
    }

}
