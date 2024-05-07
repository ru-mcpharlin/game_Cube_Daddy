using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //**********************************************************************************************************//
    //Variables
    #region Variables
    private PlayerController player;

    [Header("State")]
    [SerializeField] EnemyState enemyState;

    [Header("sleep")]
    [SerializeField] float wakeUpRadius;





    #endregion

    //**********************************************************************************************************//
    //enum
    #region enum
    public enum EnemyState
    {
        sleep,
        patrol,
        chase
    }

    public void SetEnemyState(EnemyState inputState)
    {
        enemyState = inputState;
    }

    #endregion

    //**********************************************************************************************************//
    //start
    #region Start

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();   
    }
    #endregion

    //**********************************************************************************************************//
    //update
    #region Update
    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion

    //**********************************************************************************************************//
    //move
    #region Move

    public void EnemyBehaviour()
    {
        switch (enemyState)
        {
            case EnemyState.sleep:
                Sleep();
                break;

            case EnemyState.patrol:
                break;

            case EnemyState.chase:
                break;
        }
    }

    //**********************************************************************************************************//

    public void Sleep()
    {
        if(wakeUpRadius > Vector3.Distance(transform.position, player.transform.position))
        {
            SetEnemyState(EnemyState.chase);
        }
    }



    #endregion
}
