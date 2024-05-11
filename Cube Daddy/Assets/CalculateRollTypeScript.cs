using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateRollTypeScript : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] string tag_MagneticEnvironment;

    [SerializeField] bool debug;
    [SerializeField] PlayerController.RollType debugRollType;

    [Space]
    [Space]
    [SerializeField] bool debugBonkFlat1;
    [SerializeField] bool debugBonkFlat2;

    [Space]
    [Space]
    [SerializeField] bool debugStuck1;
    [SerializeField] bool debugStuck2;
    [SerializeField] bool debugStuck3;

    private void Start()
    {
        player = GetComponent<PlayerController>();
    }



    //**********************************************************************************************************//
    //get the corrent roll type
    //calculate roll type
    public PlayerController.RollType CalculateRollType(Vector3 position, Vector3 direction, float scale)
    {
        #region Debug
        if(debug)
        {
            DebugMovemet(debugRollType, position, direction, scale);
        }

        #endregion

        //////////// BOOLS ///////////
        #region bools

        //////////// IS BLOCK //////////
        #region is Block
        //direction 1
        bool isBlock_direction1 = Physics.Raycast(position, direction, out RaycastHit hit_direction1, scale);
        bool isBlock_direction1up1 = Physics.Raycast(position + Vector3.up * scale, direction, scale);
        bool isBlock_direction1up2 = Physics.Raycast(position + Vector3.up * scale * 2, direction, scale);
        bool isBlock_direction1down1 = Physics.Raycast(position + direction * scale, Vector3.down, scale);

        //direction 2
        bool isBlock_direction2 = Physics.Raycast(position + direction * scale, direction, scale);
        bool isBlock_direction2down1 = Physics.Raycast(position + direction * scale * 2, Vector3.down, scale);

        //block 1
        bool isBlock_up1 = Physics.Raycast(position, Vector3.up, out RaycastHit hit_up1, scale);

        //block 2
        bool isBlock_up2 = Physics.Raycast(position + Vector3.up * scale, Vector3.up, scale);

        //direction minus 1
        bool isBlock_directionMinus1 = Physics.Raycast(position, -direction, out RaycastHit hit_directionMinus1, scale);
        bool isBlock_directionMinus1up1 = Physics.Raycast(position + Vector3.up * scale, -direction, scale);
        bool isBlock_directionMinus1down1 = Physics.Raycast(position + Vector3.down * scale, -direction, scale);

        //down 1
        bool isBlock_down1 = Physics.Raycast(position, Vector3.down, out RaycastHit hit_down1, scale);

        //left forward 1
        bool isBlock_leftForward1 = Physics.Raycast(position, -Vector3.Cross(direction, Vector3.up), out RaycastHit hit_leftForward1, scale);
        bool isBlock_leftForward1direction1 = Physics.Raycast(position + direction * scale, -Vector3.Cross(direction, Vector3.up), scale);

        //left forward 2
        bool isBlock_leftForward2direction1 = Physics.Raycast(position + -Vector3.Cross(direction, Vector3.up) * scale * 2, direction, scale);

        //left back 1
        bool isBlock_leftBack1 = Physics.Raycast(position, Vector3.Cross(direction, Vector3.up), scale);
        bool isBlock_leftBack1direction1 = Physics.Raycast(position + Vector3.Cross(direction, Vector3.up) * scale, direction, scale);

        //right forward 1
        bool isBlock_rightForward1 = Physics.Raycast(position, Vector3.Cross(direction, Vector3.up), out RaycastHit hit_rightForward1, scale);
        bool isBlock_rightForward1direction1 = Physics.Raycast(position + direction * scale, Vector3.Cross(direction, Vector3.up), scale);

        //right back 1
        bool isBlock_rightBack1 = Physics.Raycast(position, -Vector3.Cross(direction, Vector3.up), scale);
        bool isBlock_rightBack1direction1 = Physics.Raycast(position + -Vector3.Cross(direction, Vector3.up) * scale, direction, scale);

        //right forward 2
        bool isBlock_rightForward2direction1 = Physics.Raycast(position + Vector3.Cross(direction, Vector3.up) * scale * 2, direction, scale);

        #endregion

        //////////////// IS MAGNETIC ////////////
        #region is Magnetic
        bool isMagnetic_direction1 = false;
        if (isBlock_direction1)
        {
            isMagnetic_direction1 = hit_direction1.collider.gameObject.CompareTag(tag_MagneticEnvironment);
        }

        bool isMagnetic_directionMinus1 = false;
        if(isBlock_directionMinus1)
        {
            isMagnetic_directionMinus1 = hit_directionMinus1.collider.gameObject.CompareTag(tag_MagneticEnvironment);
        }

        bool isMagnetic_up1 = false;
        if (isBlock_up1)
        {
            isMagnetic_up1 = hit_up1.collider.gameObject.CompareTag(tag_MagneticEnvironment);
        }

        bool isMagnetic_down1 = false;
        if (isBlock_down1)
        {
            isMagnetic_down1 = hit_down1.collider.gameObject.CompareTag(tag_MagneticEnvironment);
        }

        bool isMagnetic_leftForward1 = false;
        if (isBlock_leftForward1)
        {
            isMagnetic_leftForward1 = hit_leftForward1.collider.gameObject.CompareTag(tag_MagneticEnvironment);
        }

        bool isMagnetic_rightForward1 = false;
        if (isBlock_rightForward1)
        {
            isMagnetic_rightForward1 = hit_rightForward1.collider.gameObject.CompareTag(tag_MagneticEnvironment);
        }

        #endregion

        #endregion

        //**********************************************************************************************************//
        // STEP
        #region STEP

        // STEP UP
        #region STEP UP
        ////////////// STEP UP //////////////
        // IS cube direction 1
        // &&
        // IS NOT cube direction 1 + up 1 
        // &&
        // IS NOT cube up 1
        // &&
        // IS NOT cube direction -1
        // &&
        // IS NOT cube direction -1 + up 1
        // &&
        // IS NOT cube up + 2
        // &&
        // IS NOT cube direction 1 + up + 2
        if (isBlock_direction1 &&
            !isBlock_direction1up1 &&
            !isBlock_up1 &&
            !isBlock_directionMinus1 &&
            !isBlock_directionMinus1up1 &&
            !isBlock_up2 &&
            !isBlock_direction1up2)
        {

            return PlayerController.RollType.step_Up;
        }

        ////////////// STEP UP 1 BONK //////////////
        // IS cube direction 1
        // &&
        // IS NOT cube direction 1 + up 1 
        // &&
        // IS NOT cube up 1
        // &&
        // IS NOT cube direction -1
        // &&
        // IS cube direction -1 + up 1
        if (isBlock_direction1 &&
            !isBlock_direction1up1 &&
            !isBlock_up1 &&
            !isBlock_directionMinus1 &&
            isBlock_directionMinus1up1)
        {

            return PlayerController.RollType.bonk_stepUp1;
        }

        ////////////// STEP UP 2 BONK //////////////
        // IS cube direction 1
        // &&
        // IS NOT cube direction 1 + up 1 
        // &&
        // IS NOT cube up 1
        // &&
        // IS NOT cube direction -1
        // &&
        // IS NOT cube direction -1 + up 1
        // &&
        // IS cube up + 2

        if (isBlock_direction1 &&
            !isBlock_direction1up1 &&
            !isBlock_up1 &&
            !isBlock_directionMinus1 &&
            !isBlock_directionMinus1up1 &&
            isBlock_up2)
        {

            return PlayerController.RollType.bonk_stepUp2;
        }

        ////////////// STEP UP 3 BONK //////////////
        // IS cube direction 1
        // &&
        // IS NOT cube direction 1 + up 1 
        // &&
        // IS NOT cube up 1
        // &&
        // IS NOT cube direction -1
        // &&
        // IS NOT cube direction -1 + up 1
        // &&
        // IS NOT cube up + 2
        // &&
        // IS cube direction 1 + up + 2
        if (isBlock_direction1 &&
            !isBlock_direction1up1 &&
            !isBlock_up1 &&
            !isBlock_directionMinus1 &&
            !isBlock_directionMinus1up1 &&
            !isBlock_up2 &&
            isBlock_direction1up2)
        {

            return PlayerController.RollType.bonk_stepUp3;
        }

        #endregion

        // STEP DOWN
        #region STEP DOWN
        ////////////// STEP DOWN //////////////
        // IS cube down 1
        // &&
        // IS NOT cube direction 1
        // &&
        // IS NOT cube direction 1 + down 1
        // &&
        // IS NOT cube up 1
        // &&
        // IS NOT cube direction 1 + up 1
        // &&
        // IS NOT cube in direction 2
        // &&
        // IS NOT cube in direction 2 + down 1
        else if (isBlock_down1 &&
                !isBlock_direction1 &&
                !isBlock_direction1down1 &&
                !isBlock_up1 &&
                !isBlock_direction1up1 &&
                !isBlock_direction2 &&
                !isBlock_direction2down1)
        {
            return PlayerController.RollType.step_Down;
        }

        ////////////// STEP DOWN BONK 1 //////////////
        // IS cube down 1
        // &&
        // IS NOT cube direction 1
        // &&
        // IS NOT cube direction 1 + down 1
        // &&
        // IS NOT cube up 1
        // &&
        // IS cube direction 1 + up 1
        else if (isBlock_down1 &&
                !isBlock_direction1 &&
                !isBlock_direction1down1 &&
                !isBlock_up1 &&
                 isBlock_direction1up1)
        {
            return PlayerController.RollType.bonk_stepDown1;
        }

        ////////////// STEP DOWN BONK 2 //////////////
        // IS cube down 1
        // &&
        // IS NOT cube direction 1
        // &&
        // IS NOT cube direction 1 + down 1
        // &&
        // IS NOT cube up 1
        // &&
        // IS NOT cube direction 1 + up 1
        // &&
        // IS cube in direction 2
        else if (isBlock_down1 &&
                !isBlock_direction1 &&
                !isBlock_direction1down1 &&
                !isBlock_up1 &&
                !isBlock_direction1up1 &&
                 isBlock_direction2)
        {
            return PlayerController.RollType.flat;
        }

        ////////////// STEP DOWN BONK 3 //////////////
        // IS cube down 1
        // &&
        // IS NOT cube direction 1
        // &&
        // IS NOT cube direction 1 + down 1
        // &&
        // IS NOT cube up 1
        // &&
        // IS NOT cube direction 1 + up 1
        // &&
        // IS NOT cube in direction 2
        // &&
        // IS cube in direction 2 + down 1
        else if (isBlock_down1 &&
                !isBlock_direction1 &&
                !isBlock_direction1down1 &&
                !isBlock_up1 &&
                !isBlock_direction1up1 &&
                !isBlock_direction2 &&
                 isBlock_direction2down1)
        {
            return PlayerController.RollType.flat;
        }

        #endregion

        //STEP LEFT
        #region Step Left
        ////////////// STEP LEFT //////////////
        // IS cube 'forward' 1 && IS magnetic
        // &&
        // IS NOT cube direction 1
        // &&
        // IS NOT cube direction 1 && 'forward' 1
        // &&
        // IS NOT cube down 1
        // &&
        // IS NOT cube 'forward' -1 
        // && 
        // IS NOT cube 'forward' -1 + direction 1
        // &&
        // IS NOT cube direction 2
        // &&
        // IS NOT cube 'forward' 2 + direction 1
        else if (isBlock_leftForward1 &&
                 isMagnetic_leftForward1 &&
                !isBlock_direction1 &&
                !isBlock_leftForward1direction1 &&
                !isBlock_down1 &&
                !isBlock_leftBack1 &&
                !isBlock_leftBack1direction1 &&
                !isBlock_direction2 &&
                !isBlock_leftForward2direction1)

        {
            return PlayerController.RollType.step_Left;
        }

        ////////////// STEP LEFT BONK 1 //////////////
        // IS cube 'forward' 1 && IS magnetic
        // &&
        // IS NOT cube direction 1
        // &&
        // IS NOT cube direction 1 && 'forward' 1
        // &&
        // IS NOT cube down 1
        // &&
        // IS NOT cube 'forward' -1 
        // && 
        // IS cube 'forward' -1 + direction 1
        else if (isBlock_leftForward1 &&
                 isMagnetic_leftForward1 &&
                !isBlock_direction1 &&
                !isBlock_leftForward1direction1 &&
                !isBlock_down1 &&
                !isBlock_leftBack1 &&
                 isBlock_leftBack1direction1)

        {
            return PlayerController.RollType.bonk_stepLeft1;
        }

        ////////////// STEP LEFT BONK 2//////////////
        // IS cube 'forward' 1 && IS magnetic
        // &&
        // IS NOT cube direction 1
        // &&
        // IS NOT cube direction 1 && 'forward' 1
        // &&
        // IS NOT cube down 1
        // &&
        // IS NOT cube 'forward' -1 
        // && 
        // IS NOT cube 'forward' -1 + direction 1
        // &&
        // IS cube direction 2
        else if (isBlock_leftForward1 &&
                 isMagnetic_leftForward1 &&
                !isBlock_direction1 &&
                !isBlock_leftForward1direction1 &&
                !isBlock_down1 &&
                !isBlock_leftBack1 &&
                !isBlock_leftBack1direction1 &&
                 isBlock_direction2)

        {
            return PlayerController.RollType.bonk_stepLeft2;
        }

        ////////////// STEP LEFT BONK 3 //////////////
        // IS cube 'forward' 1 && IS magnetic
        // &&
        // IS NOT cube direction 1
        // &&
        // IS NOT cube direction 1 && 'forward' 1
        // &&
        // IS NOT cube down 1
        // &&
        // IS NOT cube 'forward' -1 
        // && 
        // IS NOT cube 'forward' -1 + direction 1
        // &&
        // IS NOT cube direction 2
        // &&
        // IS cube 'forward' 1 + direction 2
        else if (isBlock_leftForward1 &&
                 isMagnetic_leftForward1 &&
                !isBlock_direction1 &&
                !isBlock_leftForward1direction1 &&
                !isBlock_down1 &&
                !isBlock_leftBack1 &&
                !isBlock_leftBack1direction1 &&
                !isBlock_direction2 &&
                 isBlock_leftForward2direction1)

        {
            return PlayerController.RollType.bonk_stepLeft3;
        }



        #endregion

        //step right
        #region Step Right
        ////////////// STEP RIGHT //////////////
        // IS cube 'forward' 1 && IS magnetic
        // &&
        // IS NOT cube direction 1
        // &&
        // IS NOT cube direction 1 && forward 1
        // &&
        // IS NOT cube down 1
        // &&
        // IS NOT cube 'forward' -1 
        // && 
        // IS NOT cube 'forward' -1 + direction 1
        // &&
        // IS NOT cube direction 2
        // &&
        // IS NOT cube 'forward' 1 + direction 2
        else if (isBlock_rightForward1 &&
                 isMagnetic_rightForward1 &&
                !isBlock_direction1 &&
                !isBlock_rightForward1direction1 &&
                !isBlock_down1 &&
                !isBlock_rightBack1 &&
                !isBlock_rightBack1direction1 &&
                !isBlock_direction2 &&
                !isBlock_rightForward2direction1)
        {
            return PlayerController.RollType.step_Right;
        }

        ////////////// STEP RIGHT BONK 0 //////////////
        // IS cube 'forward' 1 && IS magnetic
        // &&
        // IS NOT cube direction 1
        // &&
        // IS NOT cube direction 1 && forward 1
        // &&
        // IS NOT cube down 1
        // &&
        // IS cube 'forward' -1 
        else if (isBlock_rightForward1 &&
                 isMagnetic_rightForward1 &&
                !isBlock_direction1 &&
                !isBlock_rightForward1direction1 &&
                !isBlock_down1 &&
                isBlock_rightBack1)
        {
            return PlayerController.RollType.stuck;
        }

        ////////////// STEP RIGHT BONK 1 //////////////
        // IS cube 'forward' 1 && IS magnetic
        // &&
        // IS NOT cube direction 1
        // &&
        // IS NOT cube direction 1 && forward 1
        // &&
        // IS NOT cube down 1
        // &&
        // IS NOT cube 'forward' -1 
        // && 
        // IS cube 'forward' -1 + direction 1
        else if (isBlock_rightForward1 &&
                 isMagnetic_rightForward1 &&
                !isBlock_direction1 &&
                !isBlock_rightForward1direction1 &&
                !isBlock_down1 &&
                !isBlock_rightBack1 &&
                isBlock_rightBack1direction1)
        {
            return PlayerController.RollType.bonk_stepRight1;
        }

        ////////////// STEP RIGHT BONK 2 //////////////
        //if IS cube 'forward' 1 && IS magnetic
        // &&
        //if IS NOT cube direction 1
        // &&
        //if IS NOT cube direction 1 && forward 1
        // &&
        //if IS NOT cube down 1
        // &&
        // IS NOT cube 'forward' -1 
        // && 
        // IS NOT cube 'forward' -1 + direction 1
        // &&
        // IS cube direction 2
        else if (isBlock_rightForward1 &&
                 isMagnetic_rightForward1 &&
                !isBlock_direction1 &&
                !isBlock_rightForward1direction1 &&
                !isBlock_down1 &&
                !isBlock_rightBack1 &&
                !isBlock_rightBack1direction1 &&
                isBlock_direction2)
        {
            return PlayerController.RollType.bonk_stepRight2;
        }

        ////////////// STEP RIGHT BONK 3 //////////////
        //if IS cube 'forward' 1 && IS magnetic
        // &&
        //if IS NOT cube direction 1
        // &&
        //if IS NOT cube direction 1 && forward 1
        // &&
        //if IS NOT cube down 1
        // &&
        // IS NOT cube 'forward' -1 
        // && 
        // IS NOT cube 'forward' -1 + direction 1
        // &&
        // IS NOT cube direction 2
        // &&
        // IS cube 'forward' 1 + direction 2
        else if (isBlock_rightForward1 &&
                 isMagnetic_rightForward1 &&
                !isBlock_direction1 &&
                !isBlock_rightForward1direction1 &&
                !isBlock_down1 &&
                !isBlock_rightBack1 &&
                !isBlock_rightBack1direction1 &&
                !isBlock_direction2 &&
                isBlock_rightForward2direction1)
        {
            return PlayerController.RollType.bonk_stepRight3;
        }

        #endregion

        #endregion

        //**********************************************************************************************************//
        //CLIMB
        #region CLIMB

        #region Climb Up
        ////////////// CLIMB UP //////////////
        // IS cube direction 1 && IS magnetic
        // &&
        // IS cube direction 1 + up 1
        // &&
        // IS NOT cube up 1
        // &&
        // IS NOT cube direction -1
        // &&
        // IS NOT cube direction -1 + up 1
        else if (isBlock_direction1 &&
                 isMagnetic_direction1 &&
                 isBlock_direction1up1 &&
                 !isBlock_up1 &&
                 !isBlock_directionMinus1 &&
                 !isBlock_directionMinus1up1)
        {
            return PlayerController.RollType.climb_Up;
        }

        ////////////// CLIMB UP FLAT BONK //////////////
        // IS cube direction 1 && IS magnetic
        // &&
        // IS cube direction 1 + up 1
        // &&
        // IS cube up 1 && IS magnetic
        // &&
        // IS NOT cube direction -1
        // &&
        // IS NOT cube direction -1 + up 1
        else if (isBlock_direction1 &&
                 isMagnetic_direction1 &&
                 isBlock_direction1up1 &&
                 isBlock_up1 &&
                 isMagnetic_up1 &&
                 !isBlock_directionMinus1)
        {
            return PlayerController.RollType.bonk_climbUp_flat;
        }

        ////////////// CLIMB UP HEAD BONK //////////////
        // IS cube direction 1 && IS magnetic
        // &&
        // IS cube direction 1 + up 1
        // &&
        // IS NOT cube up 1
        // &&
        // IS NOT cube direction -1
        // &&
        // IS cube direction -1 + up 1 && IS NOT magnetic
        else if (isBlock_direction1 &&
                 isMagnetic_direction1 &&
                 isBlock_direction1up1 &&
                 !isBlock_up1 &&
                 !isBlock_directionMinus1 &&
                 isBlock_directionMinus1up1)
        {
            return PlayerController.RollType.bonk_climbUp_head;
        }

        #endregion

        #region climb down
        ////////////// CLIMB DOWN //////////////
        // IS cube direction -1 && IS magnetic
        // &&
        // IS cube direction -1 down 1
        // &&
        // IS NOT cube down 1
        // &&
        // IS NOT cube direction 1
        // &&
        // IS NOT cube direction 1 down 1
        else if (isBlock_directionMinus1 &&
                 isMagnetic_directionMinus1 &&
                 isBlock_directionMinus1down1 &&
                !isBlock_direction1 &&
                !isBlock_direction1down1)
        {
            return PlayerController.RollType.climb_Down;
        }

        ////////////// CLIMB DOWN BONK //////////////
        
        else if (false)
        {
            return PlayerController.RollType.flat;
        }
        #endregion

        #region Climb left
        ////////////// CLIMB LEFT //////////////
        // IS cube 'forward' 1 && is magnetic
        // &&
        // IS cube direction 1 && 'forward' 1
        // &&
        // IS NOT cube down 1
        // &&
        // IS NOT cube direction 1
        // &&
        // IS NOT cube 'forward' -1
        // &&
        // IS NOT cube 'forward' -1 + direction 1

        else if (isBlock_leftForward1 &&
                 isMagnetic_leftForward1 &&
                 isBlock_leftForward1direction1 &&
                !isBlock_down1 &&
                !isBlock_direction1 &&
                !isBlock_leftBack1 &&
                !isBlock_rightForward1direction1)
        {
            return PlayerController.RollType.climb_Left;
        }

        ////////////// CLIMB LEFT BONK FLAT //////////////
        // IS cube 'forward' 1 && is magnetic
        // &&
        // IS cube direction 1 && 'forward' 1
        // &&
        // IS NOT cube down 1
        // &&
        // IS cube direction 1
        // &&
        // IS cube direction 1 + up 1
        // &&
        // IS NOT cube 'forward' -1
        else if (isBlock_leftForward1 &&
                 isMagnetic_leftForward1 &&
                 isBlock_leftForward1direction1 &&
                !isBlock_down1 &&
                 isBlock_direction1 &&
                 isBlock_direction1up1 &&
                !isBlock_leftBack1)
        {
            return PlayerController.RollType.bonk_climbLeft_flat;
        }

        ////////////// CLIMB LEFT BONK HEAD //////////////
        // IS cube 'forward' 1 && is magnetic
        // &&
        // IS cube direction 1 && 'forward' 1
        // &&
        // IS NOT cube down 1
        // &&
        // IS NOT cube direction 1
        // &&
        // IS NOT cube 'forward' -1
        // &&
        // IS cube 'forward' -1 + direction 1
        else if (isBlock_leftForward1 &&
                 isMagnetic_leftForward1 &&
                 isBlock_leftForward1direction1 &&
                !isBlock_down1 &&
                !isBlock_direction1 &&
                !isBlock_leftBack1 &&
                 isBlock_rightForward1direction1)
        {
            return PlayerController.RollType.bonk_climbLeft_head;
        }

        #endregion

        #region Climb Right
        ////////////// CLIMB RIGHT //////////////
        // IS cube 'forward' 1 && is magnetic
        // &&
        // IS cube direction 1 && 'forward' 1
        // &&
        // IS NOT cube down 1
        // &&
        // IS NOT cube direction 1
        // &&
        // IS NOT cube 'forward' -1
        // &&
        // IS NOT cube 'forward' -1 + direction 1
        else if (isBlock_rightForward1 &&
                 isMagnetic_rightForward1 &&
                 isBlock_rightForward1direction1 &&
                !isBlock_down1 &&
                !isBlock_direction1 &&
                !isBlock_rightBack1 &&
                !isBlock_leftForward1direction1)
        {
            return PlayerController.RollType.climb_Right;
        }

        ////////////// CLIMB RIGHT BONK FLAT //////////////
        // IS cube 'forward' 1 && is magnetic
        // &&
        // IS cube direction 1 && 'forward' 1
        // &&
        // IS NOT cube down 1
        // &&
        // IS cube direction 1
        // &&  
        // IS cube direction 1 + up 1
        // &&
        // IS NOT cube 'forward' -1
        else if (isBlock_rightForward1 &&
                 isMagnetic_rightForward1 &&
                 isBlock_rightForward1direction1 &&
                !isBlock_down1 &&
                 isBlock_direction1 &&
                 isBlock_direction1up1 &&
                !isBlock_rightBack1)
        {
            return PlayerController.RollType.bonk_climbRight_flat;
        }

        ////////////// CLIMB RIGHT BONK HEAD //////////////
        // IS cube 'forward' 1 && is magnetic
        // &&
        // IS cube direction 1 && 'forward' 1
        // &&
        // IS NOT cube down 1
        // &&
        // IS NOT cube direction 1
        // &&
        // IS NOT cube 'forward' -1
        // &&
        // IS cube 'forward' -1 + direction 1
        else if (isBlock_rightForward1 &&
                 isMagnetic_rightForward1 &&
                 isBlock_rightForward1direction1 &&
                !isBlock_down1 &&
                !isBlock_direction1 &&
                !isBlock_rightBack1 &&
                 isBlock_leftForward1direction1)
        {
            return PlayerController.RollType.bonk_climbRight_head;
        }

        #endregion

        #endregion

        //**********************************************************************************************************//
        //FLAT
        #region flat
        ////////////// BONK //////////////
        // IS cube direction 1 && IS NOT magnetic
        // && 
        // IS cube direction 1 + up 1
        // &&
        // IS NOT cube direction -1
        //
        // OR
        //
        // IS cube direction 1
        // &&
        // IS cube up 1
        // &&
        // IS NOT cube direction -1
        else if (isBlock_direction1 &&
                !isMagnetic_direction1 &&
                 isBlock_direction1up1 &&
                !isBlock_directionMinus1)
        {
            return PlayerController.RollType.bonk_flat;
        }
        else if (isBlock_direction1 &&
                 isBlock_up1 &&
                !isBlock_directionMinus1)
        {
            return PlayerController.RollType.bonk_flat;
        }

        ////////////// HEAD BONK //////////////
        // IS NOT cube direction 1
        // &&
        // IS cube direction 1 + up 1
        // &&
        // IS NOT cube dirction -1
        // &&
        // IS NOT cube up 1
        else if (!isBlock_direction1 &&
                  isBlock_direction1up1 &&
                 !isBlock_directionMinus1 &&
                 !isBlock_up1)
        {
            return PlayerController.RollType.bonk_head;
        }

        ////////////// STUCK //////////////
        //if cant bonk or head bonk because of cubes around it
        //bonk 1
        //bonk 2
        //headbonk 1
        else if (isBlock_direction1 &&
                 isMagnetic_direction1 &&
                 isBlock_direction1up1 &&
                 isBlock_directionMinus1
                 ||
                 isBlock_direction1 &&
                 isBlock_up1 &&
                 isBlock_directionMinus1
                 ||
                !isBlock_direction1 &&
                 isBlock_direction1up1 &&
                 isBlock_directionMinus1 &&
                 isBlock_up1)
        {
            return PlayerController.RollType.stuck;
        }

        ////////////// FLAT //////////////
        //IS NOT cube direction 1
        // &&
        //IS NOT cube up 1
        // &&
        //IS NOT cube direction 1 + up 1
        // &&
        //IS cube direction 1 + down 1
        else if (!isBlock_direction1 &&
                 !isBlock_up1 &&
                 !isBlock_direction1up1 &&
                  isBlock_direction1down1)
        {
            return PlayerController.RollType.flat;
        }
        else
        {
            return PlayerController.RollType.stuck;
        }

        #endregion

    }

    public void DebugMovemet(PlayerController.RollType inputRollType, Vector3 position, Vector3 direction, float scale)
    {
        Debug.Log("Debugging:" + inputRollType.ToString() + " roll type");

        switch (inputRollType)
        {
            //Debug.DrawRay( );
            //////////////////////////////////////////
            #region Step

            //////////////////////////////////////////
            #region Step Up
            case PlayerController.RollType.step_Up:
                Debug.DrawRay(position, direction, Color.green, scale);
                Debug.DrawRay(position + direction * scale, Vector3.up, Color.red, scale);
                Debug.DrawRay(position, Vector3.up, Color.red, scale);
                Debug.DrawRay(position, -direction, Color.red, scale);
                Debug.DrawRay(position + Vector3.up * scale, -direction, Color.red, scale);
                Debug.DrawRay(position + Vector3.up * scale, Vector3.up, Color.red, scale);
                Debug.DrawRay(position + Vector3.up * scale + direction * scale, Vector3.up, Color.red, scale);
                break;
            
            case PlayerController.RollType.bonk_stepUp1:
                Debug.DrawRay(position, direction, Color.green, scale);
                Debug.DrawRay(position + direction * scale, Vector3.up, Color.red, scale);
                Debug.DrawRay(position, Vector3.up, Color.red, scale);
                Debug.DrawRay(position, -direction, Color.red, scale);
                Debug.DrawRay(position + Vector3.up * scale, -direction, Color.green, scale);
                break;

            case PlayerController.RollType.bonk_stepUp2:
                Debug.DrawRay(position, direction, Color.green, scale);
                Debug.DrawRay(position + direction * scale, Vector3.up, Color.red, scale);
                Debug.DrawRay(position, Vector3.up, Color.red, scale);
                Debug.DrawRay(position, -direction, Color.red, scale);
                Debug.DrawRay(position + Vector3.up * scale, -direction, Color.red, scale);
                Debug.DrawRay(position + Vector3.up * scale, Vector3.up, Color.green, scale);
                break;
            
            case PlayerController.RollType.bonk_stepUp3:
                Debug.DrawRay(position, direction, Color.green, scale);
                Debug.DrawRay(position + direction * scale, Vector3.up, Color.red, scale);
                Debug.DrawRay(position, Vector3.up, Color.red, scale);
                Debug.DrawRay(position, -direction, Color.red, scale);
                Debug.DrawRay(position + Vector3.up * scale, -direction, Color.red, scale);
                Debug.DrawRay(position + Vector3.up * scale, Vector3.up, Color.red, scale);
                Debug.DrawRay(position + Vector3.up * scale + direction * scale, Vector3.up, Color.green, scale);
                break;

            #endregion

            //////////////////////////////////////////
            #region Step Down
            case PlayerController.RollType.step_Down:
                Debug.DrawRay(position, Vector3.down, Color.green, scale);
                Debug.DrawRay(position, direction, Color.red, scale);
                Debug.DrawRay(position + direction * scale, Vector3.down, Color.red, scale);
                Debug.DrawRay(position, Vector3.up, Color.red, scale);
                Debug.DrawRay(position + direction * scale, Vector3.up, Color.red, scale);
                Debug.DrawRay(position + direction * scale, direction, Color.red, scale);
                Debug.DrawRay(position + direction * scale * 2, Vector3.down, Color.red, scale);
                break;

            case PlayerController.RollType.bonk_stepDown1:
                Debug.DrawRay(position, Vector3.down, Color.red, scale);
                Debug.DrawRay(position, direction, Color.red, scale);
                Debug.DrawRay(position + direction * scale, Vector3.down, Color.red, scale);
                Debug.DrawRay(position, Vector3.up, Color.red, scale);
                Debug.DrawRay(position + direction * scale, Vector3.up, Color.green, scale);
                break;

            case PlayerController.RollType.bonk_stepDown2:
                Debug.DrawRay(position, Vector3.down, Color.red, scale);
                Debug.DrawRay(position, direction, Color.red, scale);
                Debug.DrawRay(position + direction * scale, Vector3.down, Color.red, scale);
                Debug.DrawRay(position, Vector3.up, Color.red, scale);
                Debug.DrawRay(position + direction * scale, Vector3.up, Color.red, scale);
                Debug.DrawRay(position + direction * scale, direction, Color.green, scale);
                break;

            case PlayerController.RollType.bonk_stepDown3:
                Debug.DrawRay(position, Vector3.down, Color.red, scale);
                Debug.DrawRay(position, direction, Color.red, scale);
                Debug.DrawRay(position + direction * scale, Vector3.down, Color.red, scale);
                Debug.DrawRay(position, Vector3.up, Color.red, scale);
                Debug.DrawRay(position + direction * scale, Vector3.up, Color.red, scale);
                Debug.DrawRay(position + direction * scale, direction, Color.red, scale);
                Debug.DrawRay(position + direction * scale * 2, Vector3.down, Color.green, scale);
                break;

            #endregion

            //////////////////////////////////////////
            #region Step Left
            case PlayerController.RollType.step_Left:


                Debug.DrawRay(position, direction, Color.red, scale);

                Debug.DrawRay(position, Vector3.down, Color.red, scale);


                Debug.DrawRay(position + direction * scale, direction, Color.red, scale);

                break;

            case PlayerController.RollType.bonk_stepLeft1:

                Debug.DrawRay(position, direction, Color.red, scale);

                Debug.DrawRay(position, Vector3.down, Color.red, scale);


                break;

            case PlayerController.RollType.bonk_stepLeft2:

                Debug.DrawRay(position, direction, Color.red, scale);

                Debug.DrawRay(position, Vector3.down, Color.red, scale);


                Debug.DrawRay(position + direction * scale, direction, Color.green, scale);
                break;

            case PlayerController.RollType.bonk_stepLeft3:

                Debug.DrawRay(position, direction, Color.red, scale);

                Debug.DrawRay(position, Vector3.down, Color.red, scale);


                Debug.DrawRay(position + direction * scale, direction, Color.red, scale);

                break;

            #endregion

            //////////////////////////////////////////
            #region Step Right
            case PlayerController.RollType.step_Right:

                Debug.DrawRay(position, direction, Color.red, scale);

                Debug.DrawRay(position, Vector3.down, Color.red, scale);


                Debug.DrawRay(position + direction * scale, direction, Color.red, scale);

                break;

            case PlayerController.RollType.bonk_stepRight1:

                Debug.DrawRay(position, direction, Color.red, scale);

                Debug.DrawRay(position, Vector3.down, Color.red, scale);


                break;

            case PlayerController.RollType.bonk_stepRight2:

                Debug.DrawRay(position, direction, Color.red, scale);

                Debug.DrawRay(position, Vector3.down, Color.red, scale);


                Debug.DrawRay(position + direction * scale, direction, Color.green, scale);
                break;

            case PlayerController.RollType.bonk_stepRight3:

                Debug.DrawRay(position, direction, Color.red, scale);

                Debug.DrawRay(position, Vector3.down, Color.red, scale);


                Debug.DrawRay(position + direction * scale, direction, Color.red, scale);

                break;

            #endregion

            #endregion

            //////////////////////////////////////////
            #region Climb

            //////////////////////////////////////////
            #region Climb Up
            case PlayerController.RollType.climb_Up:
                Debug.DrawRay(position, direction, Color.blue, scale);
                Debug.DrawRay(position + direction * scale, Vector3.up, Color.green, scale);
                Debug.DrawRay(position, Vector3.up, Color.red, scale);
                Debug.DrawRay(position, -direction, Color.red, scale);
                Debug.DrawRay(position + Vector3.up * scale, -direction, Color.red, scale);
                break;

            case PlayerController.RollType.bonk_climbUp_flat:
                Debug.DrawRay(position, direction, Color.blue, scale);
                Debug.DrawRay(position + direction * scale, Vector3.up, Color.green, scale);
                Debug.DrawRay(position, Vector3.up, Color.blue, scale);
                Debug.DrawRay(position, -direction, Color.red, scale);
                break;

            case PlayerController.RollType.bonk_climbUp_head:
                Debug.DrawRay(position, direction, Color.blue, scale);
                Debug.DrawRay(position + direction * scale, Vector3.up, Color.green, scale);
                Debug.DrawRay(position, Vector3.up, Color.red, scale);
                Debug.DrawRay(position, -direction, Color.red, scale);
                Debug.DrawRay(position + Vector3.up * scale, -direction, Color.red, scale);
                break;

            #endregion

            //////////////////////////////////////////
            #region Climb Down
            case PlayerController.RollType.climb_Down:
                Debug.DrawRay(position, -direction, Color.blue, scale);
                Debug.DrawRay(position + Vector3.down * scale, -direction, Color.green, scale);
                Debug.DrawRay(position, Vector3.down, Color.red, scale);
                Debug.DrawRay(position, direction, Color.red, scale);
                Debug.DrawRay(position + Vector3.down, direction, Color.red, scale);
                break;

            case PlayerController.RollType.bonk_climbDown:

                Debug.DrawRay(position + direction * scale, Vector3.down, Color.green, scale);


                Debug.DrawRay(position, direction, Color.red, scale);
                Debug.DrawRay(position + direction * scale, Vector3.down, Color.green, scale);
                break;

            #endregion

            //////////////////////////////////////////
            #region Climb Left
            case PlayerController.RollType.climb_Left:




                Debug.DrawRay(position, direction, Color.red, scale);


                break;

            case PlayerController.RollType.bonk_climbLeft_flat:




                Debug.DrawRay(position, direction, Color.green, scale);
                Debug.DrawRay(position + direction * scale, Vector3.up, Color.green, scale);

                break;

            case PlayerController.RollType.bonk_climbLeft_head:




                Debug.DrawRay(position, direction, Color.red, scale);


                break;

            #endregion

            //////////////////////////////////////////
            #region Climb Right
            case PlayerController.RollType.climb_Right:




                Debug.DrawRay(position, direction, Color.red, scale);


                break;

            case PlayerController.RollType.bonk_climbRight_flat:




                Debug.DrawRay(position, direction, Color.green, scale);
                Debug.DrawRay(position + direction * scale, Vector3.up, Color.green, scale);

                break;

            case PlayerController.RollType.bonk_climbRight_head:




                Debug.DrawRay(position, direction, Color.red, scale);


                break;

            #endregion

            #endregion

            //////////////////////////////////////////
            #region flat
            case PlayerController.RollType.bonk_flat:
                if (debugBonkFlat1)
                {
                    Debug.DrawRay(position, direction, Color.black, scale);
                    Debug.DrawRay(position + direction * scale, Vector3.up, Color.green, scale);

                    Debug.DrawRay(position, -direction, Color.red, scale);
                }
                else if (debugBonkFlat2)
                {
                    Debug.DrawRay(position, direction, Color.green, scale);
                    Debug.DrawRay(position, Vector3.up, Color.green, scale);
                    Debug.DrawRay(position, -direction, Color.red, scale);
                }
                break;

            case PlayerController.RollType.bonk_head:
                Debug.DrawRay(position, direction, Color.red, scale);
                Debug.DrawRay(position + direction * scale, Vector3.up, Color.green, scale);
                Debug.DrawRay(position, -direction, Color.red, scale);
                Debug.DrawRay(position, Vector3.up, Color.red, scale);
                break;

            case PlayerController.RollType.stuck:
                if(debugStuck1)
                {
                    Debug.DrawRay(position, direction, Color.blue, scale);
                    Debug.DrawRay(position + direction * scale, Vector3.up, Color.green, scale);
                    Debug.DrawRay(position, -direction, Color.green, scale);
                }
                else if(debugStuck2)
                {
                    Debug.DrawRay(position, direction, Color.green, scale);
                    Debug.DrawRay(position, Vector3.up, Color.green, scale);
                    Debug.DrawRay(position, -direction, Color.green, scale);
                }
                else if(debugStuck3)
                {
                    Debug.DrawRay(position, direction, Color.red, scale);
                    Debug.DrawRay(position + direction * scale, Vector3.up, Color.green, scale);
                    Debug.DrawRay(position, -direction, Color.green, scale);
                    Debug.DrawRay(position, Vector3.up, Color.green, scale);
                }
                break;

            case PlayerController.RollType.flat:
                Debug.DrawRay(position, direction, Color.red, scale);
                Debug.DrawRay(position, Vector3.up, Color.red, scale);
                Debug.DrawRay(position + direction * scale, Vector3.up, Color.red, scale);
                Debug.DrawRay(position + direction * scale, Vector3.down, Color.green, scale);
                break;

            #endregion

        }
    }
}



