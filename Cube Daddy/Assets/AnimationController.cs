using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] string testAnimation;
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
            StartCoroutine(PlayAnimation_cannotMove(testAnimation));
        }
    }


    public IEnumerator PlayAnimation_cannotMove(string animationTrigger)
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


    public void SetPositionAtBase()
    {
        transform.position = player.cubeTransform.position + Vector3.down * player.scale * 1 / 2;
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
