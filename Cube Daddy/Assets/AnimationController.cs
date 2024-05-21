using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] string testAnimation_cannotMove;
    [SerializeField] string testAnimation_canMove;
    [SerializeField] Animator animator;
    [SerializeField] public bool isAnimating;
    // Start is called before the first frame update
    void Awake()
    {
        player = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl) && !isAnimating && !player.isMoving)
        {
            StartCoroutine(PlayAnimation_cannotMove_Coroutine(testAnimation_cannotMove));
        }

        if (Input.GetKeyDown(KeyCode.RightControl) && !isAnimating)
        {
            StartCoroutine(PlayAnimation_canMove_Coroutine(testAnimation_canMove));
        }
    }


    public void PlayAnimation_cannotMove_Method(string animationTrigger)
    {
        StartCoroutine(PlayAnimation_cannotMove_Coroutine(animationTrigger));
    }

    public IEnumerator PlayAnimation_cannotMove_Coroutine(string animationTrigger)
    {
        isAnimating = true;

        player.canMove = false;

        SetPositionAtBase();

        ParentPlayerCube(true);

        animator.SetTrigger(animationTrigger);

        while(isAnimating)
        {
            yield return new WaitForEndOfFrame();
        }

        ParentPlayerCube(false);

        player.canMove = true;

    }

    public void PlayAnimation_canMove_Method(string animationTrigger)
    {
        StartCoroutine(PlayAnimation_canMove_Coroutine(animationTrigger));
    }

    public IEnumerator PlayAnimation_canMove_Coroutine(string animationTrigger)
    {
        isAnimating = true;

        SetPositionAtBase();

        ParentPlayerCube(true);

        animator.SetTrigger(animationTrigger);

        while (isAnimating)
        {
            SetPositionAtBase();
            yield return new WaitForEndOfFrame();
        }

        ParentPlayerCube(false);
    }


    public void SetPositionAtBase()
    {
        transform.position = player.cubeTransform.position + Vector3.down * player.scale * 1 / 2;
    }

    public void SetPositionAtCenter()
    {
        transform.position = player.cubeTransform.position;
    }

    public void ParentPlayerCube(bool inputBool)
    {
        if(inputBool)
        {
            player.cubeTransform.SetParent(transform);
        }
        else
        {
            player.cubeTransform.SetParent(null);
        }

    }

    public void FinishAnimation()
    {
        isAnimating = false;
    }
}
