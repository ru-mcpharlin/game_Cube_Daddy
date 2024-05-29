using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] PlayerController player;
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
    }


    public void PlayAnimation_cannotMove_Method(string animationTrigger)
    {
        if (!isAnimating)
        {
            StartCoroutine(PlayAnimation_cannotMove_Coroutine(animationTrigger));
        }
    }

    public IEnumerator PlayAnimation_cannotMove_Coroutine(string animationTrigger)
    {
        Debug.Log("Calling: " + animationTrigger);

        isAnimating = true;

        player.canMove = false;

        //set cube rotation to identitiy
        player.cubeDatas[player.cubes_index].gameObject.transform.rotation = Quaternion.identity;

        SetPositionAtBase();
        
        ParentPlayerCube(true);

        animator.SetTrigger(animationTrigger);

        while(isAnimating)
        {
            yield return new WaitForEndOfFrame();
        }

        ParentPlayerCube(false);

        player.canMove = true;

        Debug.Log("Animation " + animationTrigger + " has ended");
    }

    //CAN MOVE
    #region Can Move
    public void PlayAnimation_canMove_Method(string animationTrigger)
    {
        if (!isAnimating)
        {
            StartCoroutine(PlayAnimation_canMove_Coroutine(animationTrigger));
        }
    }

    public IEnumerator PlayAnimation_canMove_Coroutine(string animationTrigger)
    {
        isAnimating = true;

        SetPositionAtCenter();

        ParentPlayerCube(true);

        animator.SetTrigger(animationTrigger);

        while (isAnimating)
        {
            SetPositionAtCenter();
            yield return new WaitForEndOfFrame();
        }

        ParentPlayerCube(false);
    }
    #endregion

    public void SetPositionAtBase()
    {
        transform.position = player.cubeTransform.position + 0.5f * player.currentScale * Vector3.down;
    }

    public void SetPositionAtCenter()
    {
        transform.position = player.cubeTransform.position;
    }

    public void ParentPlayerCube(bool inputBool)
    {
        if(inputBool)
        {
            player.cubeDatas[player.cubes_index].completeMesh.transform.SetParent(transform);
        }
        else
        {
            player.cubeDatas[player.cubes_index].completeMesh.transform.SetParent(player.cubeDatas[player.cubes_index].transform);
        }
    }

    public void FinishAnimation()
    {
        isAnimating = false;
    }

    public void StartTeleport_stuff()
    {
        player.cubeTransform.GetComponent<CubeData>().StartTeleport_stuff();
    }

    public void EndTeleport_stuff()
    {
        player.cubeTransform.GetComponent<CubeData>().EndTeleport_stuff();
    }
}
