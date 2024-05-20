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
    [SerializeField] public LayerMask rollLayerMask;

    [Space]
    [Space]
    [SerializeField] Color color_isCube = Color.green;
    [SerializeField] Color color_noCube = Color.red;
    [SerializeField] Color color_isMagneticCube = Color.blue;
    [SerializeField] Color color_noMagneticCube = Color.black;

    [Space]
    [Space]
    [SerializeField] bool debugClimbDown1;
    [SerializeField] bool debugClimbDown2;
    [SerializeField] bool debugClimbDown3;
    [SerializeField] bool debugClimbDown4;

    [Space]
    [Space]
    [SerializeField] bool debugClimbUp1;
    [SerializeField] bool debugClimbUp2;
    [SerializeField] bool debugClimbUp3;

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
        #region is Cube
        //direction 1
        bool isCube_direction1 = Physics.Raycast(position, direction, out RaycastHit hit_direction1, scale, rollLayerMask);
        bool isCube_direction1up1 = Physics.Raycast(position + Vector3.up * scale, direction, out RaycastHit hit_direction1up1, scale, rollLayerMask);
        bool isCube_direction1up2 = Physics.Raycast(position + Vector3.up * scale * 2, direction, out RaycastHit hit_direction1up2, scale, rollLayerMask);
        bool isCube_direction1down1 = Physics.Raycast(position + direction * scale, Vector3.down, scale, rollLayerMask);

        //direction 2
        bool isCube_direction2 = Physics.Raycast(position + direction * scale, direction, scale, rollLayerMask);
        bool isCube_direction2down1 = Physics.Raycast(position + direction * scale * 2, Vector3.down, scale, rollLayerMask);

        //block 1
        bool isCube_up1 = Physics.Raycast(position, Vector3.up, out RaycastHit hit_up1, scale, rollLayerMask);

        //block 2
        bool isCube_up2 = Physics.Raycast(position + Vector3.up * scale, Vector3.up, scale, rollLayerMask);

        //direction minus 1
        bool isCube_directionMinus1 = Physics.Raycast(position, -direction, out RaycastHit hit_directionMinus1, scale, rollLayerMask);
        bool isCube_directionMinus1up1 = Physics.Raycast(position + Vector3.up * scale, -direction, scale, rollLayerMask);
        bool isCube_directionMinus1down1 = Physics.Raycast(position + Vector3.down * scale, -direction, out RaycastHit hit_directionMinus1down1, scale, rollLayerMask);
        bool isCube_directionMinus1down2 = Physics.Raycast(position + Vector3.down * scale - direction * scale, Vector3.down, out RaycastHit hit_directionMinus1down2, scale, rollLayerMask);

        //down 1
        bool isCube_down1 = Physics.Raycast(position, Vector3.down, out RaycastHit hit_down1, scale, rollLayerMask);

        //down 2
        bool isCube_down2 = Physics.Raycast(position + Vector3.down * scale, Vector3.down, scale, rollLayerMask);

        //left forward 1
        bool isCube_leftForward1 = Physics.Raycast(position, -Vector3.Cross(direction, Vector3.up), out RaycastHit hit_leftForward1, scale, rollLayerMask);
        bool isCube_leftForward1direction1 = Physics.Raycast(position + direction * scale, -Vector3.Cross(direction, Vector3.up), out RaycastHit hit_leftForward1direction1, scale, rollLayerMask);
        bool isCube_leftForward1direction2 = Physics.Raycast(position + direction * scale * 2, -Vector3.Cross(direction, Vector3.up), out RaycastHit hit_leftForward1direction2, scale, rollLayerMask);

        //left back 1
        bool isCube_leftBack1 = Physics.Raycast(position, Vector3.Cross(direction, Vector3.up), scale, rollLayerMask);
        bool isCube_leftBack1direction1 = Physics.Raycast(position + Vector3.Cross(direction, Vector3.up) * scale, direction, scale, rollLayerMask);

        //right forward 1
        bool isCube_rightForward1 = Physics.Raycast(position, Vector3.Cross(direction, Vector3.up), out RaycastHit hit_rightForward1, scale, rollLayerMask);
        bool isCube_rightForward1direction1 = Physics.Raycast(position + direction * scale, Vector3.Cross(direction, Vector3.up), out RaycastHit hit_rightForward1direction1, scale, rollLayerMask);
        bool isCube_rightForward1direction2 = Physics.Raycast(position + direction * scale * 2, Vector3.Cross(direction, Vector3.up), out RaycastHit hit_rightForward1direction2, scale, rollLayerMask);

        //right back 1
        bool isCube_rightBack1 = Physics.Raycast(position, -Vector3.Cross(direction, Vector3.up), scale, rollLayerMask);
        bool isCube_rightBack1direction1 = Physics.Raycast(position + -Vector3.Cross(direction, Vector3.up) * scale, direction, scale, rollLayerMask);


        #endregion

        //////////////// IS MAGNETIC ////////////
        #region is Magnetic
        #region direction 1
        bool isMagnetic_direction1 = false;
        if (isCube_direction1)
        {
            isMagnetic_direction1 = hit_direction1.collider.gameObject.CompareTag(tag_MagneticEnvironment);
        }

        bool isMagnetic_direction1up1 = false;
        if (isCube_direction1up1)
        {
            isMagnetic_direction1up1 = hit_direction1up1.collider.gameObject.CompareTag(tag_MagneticEnvironment);
        }

        bool isMagnetic_direction1up2 = false;
        if (isCube_direction1up2)
        {
            isMagnetic_direction1up2 = hit_direction1up2.collider.gameObject.CompareTag(tag_MagneticEnvironment);
        }

        #endregion



        bool isMagnetic_directionMinus1 = false;
        if(isCube_directionMinus1)
        {
            isMagnetic_directionMinus1 = hit_directionMinus1.collider.gameObject.CompareTag(tag_MagneticEnvironment);
        }

        bool isMagnetic_directionMinus1down1 = false;
        if (isCube_directionMinus1down1)
        {
            isMagnetic_directionMinus1down1 = hit_directionMinus1down1.collider.gameObject.CompareTag(tag_MagneticEnvironment);
        }

        bool isMagnetic_directionMinus1down2 = false;
        if (isCube_directionMinus1down2)
        {
            isMagnetic_directionMinus1down2 = hit_directionMinus1down2.collider.gameObject.CompareTag(tag_MagneticEnvironment);
        }


        bool isMagnetic_up1 = false;
        if (isCube_up1)
        {
            isMagnetic_up1 = hit_up1.collider.gameObject.CompareTag(tag_MagneticEnvironment);
        }

        bool isMagnetic_down1 = false;
        if (isCube_down1)
        {
            isMagnetic_down1 = hit_down1.collider.gameObject.CompareTag(tag_MagneticEnvironment);
        }

        #region left forward 1

        bool isMagnetic_leftForward1 = false;
        if (isCube_leftForward1)
        {
            isMagnetic_leftForward1 = hit_leftForward1.collider.gameObject.CompareTag(tag_MagneticEnvironment);
        }

        bool isMagnetic_leftForward1direction1 = false;
        if (isCube_leftForward1direction1)
        {
            isMagnetic_leftForward1direction1 = hit_leftForward1direction1.collider.gameObject.CompareTag(tag_MagneticEnvironment);
        }

        bool isMagnetic_leftForward1direction2 = false;
        if (isCube_leftForward1direction2)
        {
            isMagnetic_leftForward1direction2 = hit_leftForward1direction2.collider.gameObject.CompareTag(tag_MagneticEnvironment);
        }

        #endregion

        #region Right Forward 1
        bool isMagnetic_rightForward1 = false;
        if (isCube_rightForward1)
        {
            isMagnetic_rightForward1 = hit_rightForward1.collider.gameObject.CompareTag(tag_MagneticEnvironment);
        }

        bool isMagnetic_rightForward1direction1 = false;
        if (isCube_rightForward1direction1)
        {
            isMagnetic_rightForward1direction1 = hit_rightForward1direction1.collider.gameObject.CompareTag(tag_MagneticEnvironment);
        }

        bool isMagnetic_rightForward1direction2 = false;
        if (isCube_rightForward1direction2)
        {
            isMagnetic_rightForward1direction2 = hit_rightForward1direction2.collider.gameObject.CompareTag(tag_MagneticEnvironment);
        }

        #endregion

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
        if (isCube_direction1 &&
           !isCube_direction1up1 &&
           !isCube_up1 &&
           !isCube_directionMinus1 &&
           !isCube_directionMinus1up1 &&
           !isCube_up2 &&
           !isCube_direction1up2)
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
        else if (isCube_direction1 &&
                !isCube_direction1up1 &&
                !isCube_up1 &&
                !isCube_directionMinus1 &&
                 isCube_directionMinus1up1)
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

        if (isCube_direction1 &&
            !isCube_direction1up1 &&
            !isCube_up1 &&
            !isCube_directionMinus1 &&
            !isCube_directionMinus1up1 &&
            isCube_up2)
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
        if (isCube_direction1 &&
            !isCube_direction1up1 &&
            !isCube_up1 &&
            !isCube_directionMinus1 &&
            !isCube_directionMinus1up1 &&
            !isCube_up2 &&
            isCube_direction1up2 &&
           !isMagnetic_direction1up2)
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
        else if (isCube_down1 &&
                !isCube_direction1 &&
                !isCube_direction1down1 &&
                !isCube_up1 &&
                !isCube_direction1up1 &&
                !isCube_direction2 &&
                !isCube_direction2down1)
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
        else if (isCube_down1 &&
                !isCube_direction1 &&
                !isCube_direction1down1 &&
                !isCube_up1 &&
                 isCube_direction1up1)
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
        else if (isCube_down1 &&
                !isCube_direction1 &&
                !isCube_direction1down1 &&
                !isCube_up1 &&
                !isCube_direction1up1 &&
                 isCube_direction2)
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
        else if (isCube_down1 &&
                !isCube_direction1 &&
                !isCube_direction1down1 &&
                !isCube_up1 &&
                !isCube_direction1up1 &&
                !isCube_direction2 &&
                 isCube_direction2down1)
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
        else if (isCube_leftForward1 &&
                 isMagnetic_leftForward1 &&
                !isCube_direction1 &&
                !isCube_leftForward1direction1 &&
                !isCube_down1 &&
                !isCube_leftBack1 &&
                !isCube_leftBack1direction1 &&
                !isCube_direction2 &&
                !isCube_leftForward1direction2)

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
        else if (isCube_leftForward1 &&
                 isMagnetic_leftForward1 &&
                !isCube_direction1 &&
                !isCube_leftForward1direction1 &&
                !isCube_down1 &&
                !isCube_leftBack1 &&
                 isCube_leftBack1direction1)

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
        else if (isCube_leftForward1 &&
                 isMagnetic_leftForward1 &&
                !isCube_direction1 &&
                !isCube_leftForward1direction1 &&
                !isCube_down1 &&
                !isCube_leftBack1 &&
                !isCube_leftBack1direction1 &&
                 isCube_direction2)

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
        else if (isCube_leftForward1 &&
                 isMagnetic_leftForward1 &&
                !isCube_direction1 &&
                !isCube_leftForward1direction1 &&
                !isCube_down1 &&
                !isCube_leftBack1 &&
                !isCube_leftBack1direction1 &&
                !isCube_direction2 &&
                 isCube_leftForward1direction2 &&
                !isMagnetic_leftForward1direction2)

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
        else if (isCube_rightForward1 &&
                 isMagnetic_rightForward1 &&
                !isCube_direction1 &&
                !isCube_rightForward1direction1 &&
                !isCube_down1 &&
                !isCube_rightBack1 &&
                !isCube_rightBack1direction1 &&
                !isCube_direction2 &&
                !isCube_rightForward1direction2)
        {
            return PlayerController.RollType.step_Right;
        }

        /*////////////// STEP RIGHT BONK 0 //////////////
        // IS cube 'forward' 1 && IS magnetic
        // &&
        // IS NOT cube direction 1
        // &&
        // IS NOT cube direction 1 && forward 1
        // &&
        // IS NOT cube down 1
        // &&
        // IS cube 'forward' -1 
        else if (isCube_rightForward1 &&
                 isMagnetic_rightForward1 &&
                !isCube_direction1 &&
                !isCube_rightForward1direction1 &&
                !isCube_down1 &&
                isCube_rightBack1)
        {
            return PlayerController.RollType.stuck;
        }*/

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
        else if (isCube_rightForward1 &&
                 isMagnetic_rightForward1 &&
                !isCube_direction1 &&
                !isCube_rightForward1direction1 &&
                !isCube_down1 &&
                !isCube_rightBack1 &&
                isCube_rightBack1direction1)
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
        else if (isCube_rightForward1 &&
                 isMagnetic_rightForward1 &&
                !isCube_direction1 &&
                !isCube_rightForward1direction1 &&
                !isCube_down1 &&
                !isCube_rightBack1 &&
                !isCube_rightBack1direction1 &&
                isCube_direction2)
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
        else if (isCube_rightForward1 &&
                 isMagnetic_rightForward1 &&
                !isCube_direction1 &&
                !isCube_rightForward1direction1 &&
                !isCube_down1 &&
                !isCube_rightBack1 &&
                !isCube_rightBack1direction1 &&
                !isCube_direction2 &&
                 isCube_rightForward1direction2 &&
                !isMagnetic_rightForward1direction2)
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
        else if (isCube_direction1 &&
                 isMagnetic_direction1 &&
                 !isCube_up1 &&
                 !isCube_directionMinus1 &&
                 !isCube_directionMinus1up1
                 ||
                 isCube_direction1up1 &&
                 isMagnetic_direction1up1 &&
                !isCube_up1 &&
                !isCube_directionMinus1up1 &&
                !isCube_directionMinus1
                 ||
                 isCube_direction1 &&
                 isMagnetic_direction1 &&
                !isCube_direction1up1 &&
                !isCube_up1 &&
                !isCube_directionMinus1up1 &&
                !isCube_directionMinus1 &&
                 isCube_direction1up2 &&
                 isMagnetic_direction1up2)
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
        else if (isCube_direction1 &&
                 isMagnetic_direction1 &&
                 isCube_up1 &&
                !isCube_directionMinus1 &&
                !isCube_directionMinus1down1 &&
                !isCube_down1)
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
        else if (isCube_direction1 &&
                 isMagnetic_direction1 &&
                 isCube_direction1up1 &&
                 !isCube_up1 &&
                 !isCube_directionMinus1 &&
                 isCube_directionMinus1up1)
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
        else if (isCube_directionMinus1 &&
                 isMagnetic_directionMinus1 &&
                 isCube_directionMinus1down1 &&
                !isCube_direction1 &&
                !isCube_direction1down1 
                 ||
                 isCube_directionMinus1 &&
                 isMagnetic_directionMinus1 &&
                !isCube_directionMinus1down1 &&
                !isCube_direction1 &&
                !isCube_direction1down1 &&
                 isCube_down2 
                 ||
                 //move onto a gap
                  player.onMagneticCube &&
                !isCube_directionMinus1down1 &&
                !isCube_direction1 &&
                !isCube_direction1down1 &&
                 isCube_directionMinus1down2 &&
                 isMagnetic_directionMinus1down2
                 ||
                 //move off a gap
                  player.onMagneticCube &&
                 isMagnetic_directionMinus1down1 &&
                !isCube_down1 &&
                !isCube_direction1 &&
                !isCube_direction1down1)
        {
            return PlayerController.RollType.climb_Down;
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

        else if (isCube_leftForward1 &&
                 isMagnetic_leftForward1 &&
                 isCube_leftForward1direction1 &&
                !isCube_down1 &&
                !isCube_direction1 &&
                !isCube_leftBack1 &&
                !isCube_leftBack1direction1
                 ||
                //onto gap
                 isCube_leftForward1 &&
                 isMagnetic_leftForward1 &&
                !isCube_leftForward1direction1 &&
                !isCube_down1 &&
                !isCube_direction1 &&
                !isCube_leftBack1 &&
                 isCube_leftForward1direction2 &&
                 isMagnetic_leftForward1direction2
                 //off gap
                 ||
                 player.CheckIfOnMagneticCube() &&
                !isCube_leftForward1 &&
                 isCube_leftForward1direction1 &&
                 isMagnetic_leftForward1direction1 &&
                !isCube_down1 &&
                !isCube_direction1 &&
                !isCube_leftBack1 &&
                !isCube_leftBack1direction1)
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
        else if (isCube_leftForward1 &&
                 isMagnetic_leftForward1 &&
                 isCube_leftForward1direction1 &&
                !isCube_down1 &&
                 isCube_direction1 &&
                 isCube_direction1up1 &&
                !isCube_leftBack1)
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
        else if (isCube_leftForward1 &&
                 isMagnetic_leftForward1 &&
                 isCube_leftForward1direction1 &&
                !isCube_down1 &&
                !isCube_direction1 &&
                !isCube_leftBack1 &&
                 isCube_leftBack1direction1)
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
        else if (isCube_rightForward1 &&
                 isMagnetic_rightForward1 &&
                 isCube_rightForward1direction1 &&
                !isCube_down1 &&
                !isCube_direction1 &&
                !isCube_rightBack1 &&
                !isCube_rightBack1direction1
                 ||
                 //onto gap
                 isCube_rightForward1 &&
                 isMagnetic_rightForward1 &&
                !isCube_rightForward1direction1 &&
                !isCube_down1 &&
                !isCube_direction1 &&
                !isCube_rightBack1 &&
                 isCube_rightForward1direction2 &&
                 isMagnetic_rightForward1direction2
                 //off gap
                 ||
                 player.CheckIfOnMagneticCube() &&
                !isCube_rightForward1 &&
                 isCube_rightForward1direction1 &&
                 isMagnetic_rightForward1direction1 &&
                !isCube_down1 &&
                !isCube_direction1 &&
                !isCube_rightBack1 &&
                !isCube_rightBack1direction1)
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
        else if (isCube_rightForward1 &&
                 isMagnetic_rightForward1 &&
                 isCube_rightForward1direction1 &&
                !isCube_down1 &&
                 isCube_direction1 &&
                 isCube_direction1up1 &&
                !isCube_rightBack1)
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
        else if (isCube_rightForward1 &&
                 isMagnetic_rightForward1 &&
                 isCube_rightForward1direction1 &&
                !isCube_down1 &&
                !isCube_direction1 &&
                !isCube_rightBack1 &&
                 isCube_rightBack1direction1)
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
        else if (isCube_direction1 &&
                !isMagnetic_direction1 &&
                 isCube_direction1up1 &&
                !isCube_directionMinus1 &&
                !isCube_directionMinus1up1)
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
        else if (!isCube_direction1 &&
                  isCube_direction1up1 &&
                 !isCube_up1)
        {
            return PlayerController.RollType.bonk_head;
        }

        ////////////// STUCK //////////////
        //if cant bonk or head bonk because of cubes around it
        //bonk 1
        //bonk 2
        //headbonk 1
        else if (isCube_direction1 &&
                 isMagnetic_direction1 &&
                 isCube_direction1up1 &&
                 isCube_directionMinus1
                 ||
                 isCube_direction1 &&
                 isCube_up1 &&
                 isCube_directionMinus1
                 ||
                !isCube_direction1 &&
                 isCube_direction1up1 &&
                 isCube_directionMinus1 &&
                 isCube_up1)
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
        else if (!isCube_direction1 &&
                 !isCube_up1 &&
                 !isCube_direction1up1 &&
                  isCube_direction1down1)
        {
            return PlayerController.RollType.flat;
        }
        else
        {
            Debug.Log("Umm wtf");
            return PlayerController.RollType.stuck;
        }

        #endregion

    }

    public void DebugMovemet(PlayerController.RollType inputRollType, Vector3 position, Vector3 direction, float scale)
    {
        Debug.Log("Debugging:" + inputRollType.ToString() + " roll type");

        switch (inputRollType)
        {
            //////////////////////////////////////////
            #region Step
            
            //////////////////////////////////////////
            #region Step Up
            case PlayerController.RollType.step_Up:
                DebugRay_direction1(color_isCube, position, direction, scale);
                DebugRay_direction1up1(color_noCube, position, direction, scale);
                DebugRay_up1(color_noCube, position, direction, scale);
                DebugRay_directionMinus1(color_noCube, position, direction, scale); 
                DebugRay_directionMinus1up1(color_noCube, position, direction, scale);
                DebugRay_up2(color_noCube, position, direction, scale);
                DebugRay_direction1up2(color_noCube, position, direction, scale);
                break;
            
            case PlayerController.RollType.bonk_stepUp1:
                DebugRay_direction1(color_isCube, position, direction, scale);
                DebugRay_direction1up1(color_noCube, position, direction, scale);
                DebugRay_up1(color_noCube, position, direction, scale);
                DebugRay_directionMinus1(color_noCube, position, direction, scale);
                DebugRay_directionMinus1up1(color_isCube, position, direction, scale);
                break;

            case PlayerController.RollType.bonk_stepUp2:
                DebugRay_direction1(color_isCube, position, direction, scale);
                DebugRay_direction1up1(color_noCube, position, direction, scale);
                DebugRay_up1(color_noCube, position, direction, scale);
                DebugRay_directionMinus1(color_noCube, position, direction, scale); 
                DebugRay_directionMinus1up1(color_noCube, position, direction, scale);
                DebugRay_up2(color_isCube, position, direction, scale);
                break;
            
            case PlayerController.RollType.bonk_stepUp3:
                DebugRay_direction1(color_isCube, position, direction, scale);
                DebugRay_direction1up1(color_noCube, position, direction, scale);
                DebugRay_up1(color_noCube, position, direction, scale);
                DebugRay_directionMinus1(color_noCube, position, direction, scale); 
                DebugRay_directionMinus1up1(color_noCube, position, direction, scale);
                DebugRay_up2(color_noCube, position, direction, scale);
                DebugRay_direction1up2(color_isCube, position, direction, scale);
                break;

            #endregion

            //////////////////////////////////////////
            #region Step Down
            case PlayerController.RollType.step_Down:
                DebugRay_down1(color_isCube, position, direction, scale);
                DebugRay_direction1(color_noCube, position, direction, scale);
                DebugRay_direction1down1(color_noCube, position, direction, scale);
                DebugRay_up1(color_noCube, position, direction, scale);
                DebugRay_direction1up1(color_noCube, position, direction, scale);
                DebugRay_direction2(color_noCube, position, direction, scale);
                DebugRay_direction2down1(color_noCube, position, direction, scale);
                break;

            case PlayerController.RollType.bonk_stepDown1:
                DebugRay_down1(color_noCube, position, direction, scale);
                DebugRay_direction1(color_noCube, position, direction, scale);
                DebugRay_direction1down1(color_noCube, position, direction, scale);
                DebugRay_up1(color_noCube, position, direction, scale);
                DebugRay_direction1up1(color_isCube, position, direction, scale);
                break;

            case PlayerController.RollType.bonk_stepDown2:
                DebugRay_down1(color_noCube, position, direction, scale);
                DebugRay_direction1(color_noCube, position, direction, scale);
                DebugRay_direction1down1(color_noCube, position, direction, scale);
                DebugRay_up1(color_noCube, position, direction, scale);
                DebugRay_direction1up1(color_noCube, position, direction, scale);
                DebugRay_direction2(color_noCube, position, direction, scale);
                break;

            case PlayerController.RollType.bonk_stepDown3:
                DebugRay_down1(color_noCube, position, direction, scale);
                DebugRay_direction1(color_noCube, position, direction, scale);
                DebugRay_direction1down1(color_noCube, position, direction, scale);
                DebugRay_up1(color_noCube, position, direction, scale);
                DebugRay_direction1up1(color_noCube, position, direction, scale);
                DebugRay_direction2(color_noCube, position, direction, scale);
                DebugRay_direction2down1(color_isCube, position, direction, scale);;
                break;

            #endregion

            //////////////////////////////////////////
            #region Step Left
            case PlayerController.RollType.step_Left:
                DebugRay_leftForward1(color_isMagneticCube, position, direction, scale);
                DebugRay_direction1(color_noCube, position, direction, scale);
                DebugRay_leftForward1direction1(color_noCube, position, direction, scale);
                DebugRay_down1(color_noCube, position, direction, scale);
                DebugRay_leftBack1(color_noCube, position, direction, scale);
                DebugRay_leftBack1direction1(color_noCube, position, direction, scale);
                DebugRay_direction2(color_noCube, position, direction, scale);
                DebugRay_leftForward1direction2(color_noCube, position, direction, scale);
                break;

            case PlayerController.RollType.bonk_stepLeft1:
                DebugRay_leftForward1(color_isMagneticCube, position, direction, scale);
                DebugRay_direction1(color_noCube, position, direction, scale);
                DebugRay_leftForward1direction1(color_noCube, position, direction, scale);
                DebugRay_down1(color_noCube, position, direction, scale);
                DebugRay_leftBack1(color_noCube, position, direction, scale);
                DebugRay_leftBack1direction1(color_isCube, position, direction, scale);
                break;

            case PlayerController.RollType.bonk_stepLeft2:
                DebugRay_leftForward1(color_isMagneticCube, position, direction, scale);
                DebugRay_direction1(color_noCube, position, direction, scale);
                DebugRay_leftForward1direction1(color_noCube, position, direction, scale);
                DebugRay_down1(color_noCube, position, direction, scale);
                DebugRay_leftBack1(color_noCube, position, direction, scale);
                DebugRay_leftBack1direction1(color_noCube, position, direction, scale);
                DebugRay_direction2(color_isCube, position, direction, scale);
                break;

            case PlayerController.RollType.bonk_stepLeft3:
                DebugRay_leftForward1(color_isMagneticCube, position, direction, scale);
                DebugRay_direction1(color_noCube, position, direction, scale);
                DebugRay_leftForward1direction1(color_noCube, position, direction, scale);
                DebugRay_down1(color_noCube, position, direction, scale);
                DebugRay_leftBack1(color_noCube, position, direction, scale);
                DebugRay_leftBack1direction1(color_noCube, position, direction, scale);
                DebugRay_direction2(color_noCube, position, direction, scale);
                DebugRay_leftForward1direction2(color_noMagneticCube, position, direction, scale);
                break;

            #endregion

            //////////////////////////////////////////
            #region Step Right
            case PlayerController.RollType.step_Right:
                DebugRay_rightForward1(color_isMagneticCube, position, direction, scale);
                DebugRay_direction1(color_noCube, position, direction, scale);
                DebugRay_rightForward1direction1(color_noCube, position, direction, scale);
                DebugRay_down1(color_noCube, position, direction, scale);
                DebugRay_rightBack1(color_noCube, position, direction, scale);
                DebugRay_rightBack1direction1(color_noCube, position, direction, scale);
                DebugRay_direction2(color_noCube, position, direction, scale);
                DebugRay_rightForward1direction2(color_noCube, position, direction, scale);
                break;

            case PlayerController.RollType.bonk_stepRight1:
                DebugRay_rightForward1(color_isMagneticCube, position, direction, scale);
                DebugRay_direction1(color_noCube, position, direction, scale);
                DebugRay_rightForward1direction1(color_noCube, position, direction, scale);
                DebugRay_down1(color_noCube, position, direction, scale);
                DebugRay_rightBack1(color_noCube, position, direction, scale);
                DebugRay_rightBack1direction1(color_isCube, position, direction, scale);
                break;

            case PlayerController.RollType.bonk_stepRight2:
                DebugRay_rightForward1(color_isMagneticCube, position, direction, scale);
                DebugRay_direction1(color_noCube, position, direction, scale);
                DebugRay_rightForward1direction1(color_noCube, position, direction, scale);
                DebugRay_down1(color_noCube, position, direction, scale);
                DebugRay_rightBack1(color_noCube, position, direction, scale);
                DebugRay_rightBack1direction1(color_noCube, position, direction, scale);
                DebugRay_direction2(color_isCube, position, direction, scale);
                break;

            case PlayerController.RollType.bonk_stepRight3:
                DebugRay_rightForward1(color_isMagneticCube, position, direction, scale);
                DebugRay_direction1(color_noCube, position, direction, scale);
                DebugRay_rightForward1direction1(color_noCube, position, direction, scale);
                DebugRay_down1(color_noCube, position, direction, scale);
                DebugRay_rightBack1(color_noCube, position, direction, scale);
                DebugRay_rightBack1direction1(color_noCube, position, direction, scale);
                DebugRay_direction2(color_noCube, position, direction, scale);
                DebugRay_rightForward1direction2(color_noMagneticCube, position, direction, scale);
                break;

            #endregion

            #endregion

            //////////////////////////////////////////
            #region Climb

            //////////////////////////////////////////
            #region Climb Up
            case PlayerController.RollType.climb_Up:
                if (debugClimbUp1)
                {
                    DebugRay_direction1(color_isMagneticCube, position, direction, scale);
                    DebugRay_up1(color_noCube, position, direction, scale);
                    DebugRay_directionMinus1(color_noCube, position, direction, scale);
                    DebugRay_directionMinus1up1(color_noCube, position, direction, scale);
                }
                else if (debugClimbUp2)
                {
                    DebugRay_direction1up1(color_isMagneticCube, position, direction, scale);
                    DebugRay_up1(color_noCube, position, direction, scale);
                    DebugRay_directionMinus1up1(color_noCube, position, direction, scale);
                    DebugRay_directionMinus1(color_noCube, position, direction, scale);
                }
                else if(debugClimbUp3)
                {
                    DebugRay_direction1(color_isMagneticCube, position, direction, scale);
                    DebugRay_direction1up1(color_noCube, position, direction, scale);
                    DebugRay_up1(color_noCube, position, direction, scale);
                    DebugRay_directionMinus1up1(color_noCube, position, direction, scale);
                    DebugRay_directionMinus1(color_noCube, position, direction, scale);
                    DebugRay_direction1up2(color_isMagneticCube, position, direction, scale);
                }


                break;

            case PlayerController.RollType.bonk_climbUp_flat:
                DebugRay_direction1(color_isMagneticCube, position, direction, scale);
                DebugRay_up1(color_isCube, position, direction, scale);
                DebugRay_directionMinus1(color_noCube, position, direction, scale);
                DebugRay_directionMinus1down1(color_noCube, position, direction, scale);
                DebugRay_down1(color_noCube, position, direction, scale);
                break;

            case PlayerController.RollType.bonk_climbUp_head:
                DebugRay_direction1(color_isMagneticCube, position, direction, scale);
                DebugRay_direction1up1(color_isCube, position, direction, scale);
                DebugRay_up1(color_noCube, position, direction, scale);
                DebugRay_directionMinus1(color_noCube, position, direction, scale); 
                DebugRay_directionMinus1up1(color_isCube, position, direction, scale);
                break;

            #endregion

            //////////////////////////////////////////
            #region Climb Down
            case PlayerController.RollType.climb_Down:
                if (debugClimbDown1)
                {
                    DebugRay_directionMinus1(color_isMagneticCube, position, direction, scale);
                    DebugRay_directionMinus1down1(color_isCube, position, direction, scale);
                    DebugRay_down1(color_noCube, position, direction, scale);
                    DebugRay_direction1(color_noCube, position, direction, scale);
                    DebugRay_direction1down1(color_noCube, position, direction, scale);
                }
                else if(debugClimbDown2)
                {
                    DebugRay_directionMinus1(color_isMagneticCube, position, direction, scale);
                    DebugRay_directionMinus1down1(color_noCube, position, direction, scale);
                    DebugRay_down1(color_noCube, position, direction, scale);
                    DebugRay_direction1(color_noCube, position, direction, scale);
                    DebugRay_direction1down1(color_noCube, position, direction, scale);
                    DebugRay_down2(color_isCube, position, direction, scale);
                }
                else if(debugClimbDown3)
                {
                    DebugRay_directionMinus1(color_isMagneticCube, position, direction, scale);
                    DebugRay_directionMinus1down1(color_noCube, position, direction, scale);
                    DebugRay_down1(color_noCube, position, direction, scale);
                    DebugRay_direction1(color_noCube, position, direction, scale);
                    DebugRay_direction1down1(color_noCube, position, direction, scale);
                    DebugRay_directionMinus1down2(color_isMagneticCube, position, direction, scale);
                }
                else if(debugClimbDown4)
                {
                    DebugRay_directionMinus1(color_noCube, position, direction, scale);
                    DebugRay_directionMinus1down1(color_isMagneticCube, position, direction, scale);
                    DebugRay_down1(color_noCube, position, direction, scale);
                    DebugRay_direction1(color_noCube, position, direction, scale);
                    DebugRay_direction1down1(color_noCube, position, direction, scale);
                }
                break;

            #endregion

            //////////////////////////////////////////
            #region Climb Left
            case PlayerController.RollType.climb_Left:
                DebugRay_leftForward1(color_isMagneticCube, position, direction, scale);
                DebugRay_leftForward1direction1(color_isCube, position, direction, scale);
                DebugRay_down1(color_noCube, position, direction, scale);
                DebugRay_direction1(color_noCube, position, direction, scale);
                DebugRay_leftBack1(color_noCube, position, direction, scale);
                DebugRay_leftBack1direction1(color_noCube, position, direction, scale);

                /*DebugRay_leftForward1(color_isMagneticCube, position, direction, scale);
                DebugRay_leftForward1direction1(color_noCube, position, direction, scale);
                DebugRay_down1(color_noCube, position, direction, scale);
                DebugRay_direction1(color_noCube, position, direction, scale);
                DebugRay_leftBack1(color_noCube, position, direction, scale);
                DebugRay_leftBack1direction1(color_noCube, position, direction, scale);
                DebugRay_leftForward1direction2(color_isMagneticCube, position, direction, scale);*/

                /*DebugRay_leftForward1(color_noCube, position, direction, scale);
                DebugRay_leftForward1direction1(color_isMagneticCube, position, direction, scale);
                DebugRay_down1(color_noCube, position, direction, scale);
                DebugRay_direction1(color_noCube, position, direction, scale);
                DebugRay_leftBack1(color_noCube, position, direction, scale);
                DebugRay_leftBack1direction1(color_noCube, position, direction, scale);*/

                break;

            case PlayerController.RollType.bonk_climbLeft_flat:
                DebugRay_leftForward1(color_isMagneticCube, position, direction, scale);
                DebugRay_leftForward1direction1(color_isCube, position, direction, scale);
                DebugRay_down1(color_noCube, position, direction, scale);
                DebugRay_direction1(color_isCube, position, direction, scale);
                DebugRay_direction1up1(color_isCube, position, direction, scale);
                DebugRay_leftBack1(color_noCube, position, direction, scale);
                break;

            case PlayerController.RollType.bonk_climbLeft_head:
                DebugRay_leftForward1(color_isMagneticCube, position, direction, scale);
                DebugRay_leftForward1direction1(color_isCube, position, direction, scale);
                DebugRay_down1(color_noCube, position, direction, scale);
                DebugRay_direction1(color_noCube, position, direction, scale);
                DebugRay_leftBack1(color_noCube, position, direction, scale);
                DebugRay_leftBack1direction1(color_isCube, position, direction, scale);
                break;

            #endregion

            //////////////////////////////////////////
            #region Climb Right
            case PlayerController.RollType.climb_Right:
                DebugRay_rightForward1(color_isMagneticCube, position, direction, scale);
                DebugRay_rightForward1direction1(color_isCube, position, direction, scale);
                DebugRay_down1(color_noCube, position, direction, scale);
                DebugRay_direction1(color_noCube, position, direction, scale);
                DebugRay_direction1(color_noCube, position, direction, scale);
                DebugRay_rightBack1(color_noCube, position, direction, scale);
                DebugRay_rightBack1direction1(color_noCube, position, direction, scale);

                /*DebugRay_rightForward1(color_isMagneticCube, position, direction, scale);
                DebugRay_rightForward1direction1(color_noCube, position, direction, scale);
                DebugRay_down1(color_noCube, position, direction, scale);
                DebugRay_direction1(color_noCube, position, direction, scale);
                DebugRay_rightBack1(color_noCube, position, direction, scale);
                DebugRay_rightBack1direction1(color_noCube, position, direction, scale);
                DebugRay_rightForward1direction2(color_isMagneticCube, position, direction, scale);

                DebugRay_rightForward1(color_noCube, position, direction, scale);
                DebugRay_rightForward1direction1(color_isMagneticCube, position, direction, scale);
                DebugRay_down1(color_noCube, position, direction, scale);
                DebugRay_direction1(color_noCube, position, direction, scale);
                DebugRay_rightBack1(color_noCube, position, direction, scale);
                DebugRay_rightBack1direction1(color_noCube, position, direction, scale);*/
                break;

            case PlayerController.RollType.bonk_climbRight_flat:
                DebugRay_rightForward1(color_isMagneticCube, position, direction, scale);
                DebugRay_rightForward1direction1(color_isCube, position, direction, scale);
                DebugRay_down1(color_noCube, position, direction, scale);
                DebugRay_direction1(color_noCube, position, direction, scale);
                DebugRay_direction1(color_isCube, position, direction, scale);
                DebugRay_direction1up1(color_isCube, position, direction, scale);
                DebugRay_rightBack1(color_noCube, position, direction, scale);
                break;

            case PlayerController.RollType.bonk_climbRight_head:
                DebugRay_rightForward1(color_isMagneticCube, position, direction, scale);
                DebugRay_rightForward1direction1(color_isCube, position, direction, scale);
                DebugRay_down1(color_noCube, position, direction, scale);
                DebugRay_direction1(color_noCube, position, direction, scale);
                DebugRay_direction1(color_noCube, position, direction, scale);
                DebugRay_rightBack1(color_noCube, position, direction, scale);
                DebugRay_rightBack1direction1(color_isCube, position, direction, scale);
                break;

            #endregion

            #endregion

            //////////////////////////////////////////
            #region flat
            case PlayerController.RollType.bonk_flat:
                DebugRay_direction1(color_noMagneticCube, position, direction, scale);
                DebugRay_direction1up1(color_isCube, position, direction, scale);
                DebugRay_directionMinus1(color_noCube, position, direction, scale);
                DebugRay_directionMinus1up1(color_noCube, position, direction, scale);
                break;

            case PlayerController.RollType.bonk_head:
                DebugRay_direction1(color_noCube, position, direction, scale);
                DebugRay_direction1up1(color_isCube, position, direction, scale);
                DebugRay_up1(color_noCube, position, direction, scale);
                break;

            case PlayerController.RollType.stuck:
                if(debugStuck1)
                {
                    DebugRay_direction1(color_isMagneticCube, position, direction, scale);
                    DebugRay_direction1up1(color_isCube, position, direction, scale);
                    DebugRay_directionMinus1(color_isCube, position, direction, scale);
                }
                else if(debugStuck2)
                {
                    DebugRay_direction1(color_isCube, position, direction, scale);
                    DebugRay_up1(color_isCube, position, direction, scale); ;
                    DebugRay_directionMinus1(color_isCube, position, direction, scale);
                }
                else if(debugStuck3)
                {
                    DebugRay_direction1(color_noCube, position, direction, scale);
                    DebugRay_direction1up1(color_isCube, position, direction, scale);
                    DebugRay_directionMinus1(color_isCube, position, direction, scale);
                    DebugRay_up1(color_isCube, position, direction, scale);
                }
                break;

            case PlayerController.RollType.flat:
                DebugRay_direction1(color_noCube, position, direction, scale);
                DebugRay_up1(color_noCube, position, direction, scale);
                DebugRay_direction1up1(color_noCube, position, direction, scale);
                DebugRay_direction1down1(color_isCube, position, direction, scale);
                break;

            #endregion

        }
    }

    /////////// DEBUG RAYS //////////////
    #region debug rays

    //direction 1
    #region direction 1
    public void DebugRay_direction1(Color color, Vector3 position, Vector3 direction, float scale)
    {
        Debug.DrawRay(position, direction * scale,  color, scale);
    }

    public void DebugRay_direction1up1(Color color, Vector3 position, Vector3 direction, float scale)
    {
        Debug.DrawRay(position + direction * scale, Vector3.up * scale,  color, scale);
    }

    public void DebugRay_direction1up2(Color color, Vector3 position, Vector3 direction, float scale)
    {
        Debug.DrawRay(position + Vector3.up * scale + direction * scale, Vector3.up * scale,  color, scale);
    }

    public void DebugRay_direction1down1(Color color, Vector3 position, Vector3 direction, float scale)
    {
        Debug.DrawRay(position + direction * scale, Vector3.down * scale,  color, scale);
    }

    #endregion

    //direction 2
    #region direction 2
    public void DebugRay_direction2(Color color, Vector3 position, Vector3 direction, float scale)
    {
        Debug.DrawRay(position + direction * scale, direction * scale,  color, scale);
    }

    public void DebugRay_direction2down1(Color color, Vector3 position, Vector3 direction, float scale)
    {
        Debug.DrawRay(position + direction * scale * 2, Vector3.down * scale,  color, scale);
    }

    #endregion

    //up
    #region up
    public void DebugRay_up1(Color color, Vector3 position, Vector3 direction, float scale)
    {
        Debug.DrawRay(position, Vector3.up * scale, color, scale);
    }

    public void DebugRay_up2(Color color, Vector3 position, Vector3 direction, float scale)
    {
        Debug.DrawRay(position + Vector3.up * scale, Vector3.up * scale, color, scale);
    }

    #endregion

    //direction minus 1
    #region direction minus 1
    public void DebugRay_directionMinus1(Color color, Vector3 position, Vector3 direction, float scale)
    {
        Debug.DrawRay(position, -direction * scale, color, scale);
    }
    
    public void DebugRay_directionMinus1up1(Color color, Vector3 position, Vector3 direction, float scale)
    {
        Debug.DrawRay(position + Vector3.up * scale, -direction * scale ,  color, scale);
    }

    public void DebugRay_directionMinus1down1(Color color, Vector3 position, Vector3 direction, float scale)
    {
        Debug.DrawRay(position - direction * scale, Vector3.down * scale ,  color, scale);
    }

    public void DebugRay_directionMinus1down2(Color color, Vector3 position, Vector3 direction, float scale)
    {
        Debug.DrawRay(position - direction * scale + Vector3.down * scale, Vector3.down * scale, color, scale);
    }

    

    #endregion

    //down 1
    #region down 1
    public void DebugRay_down1(Color color, Vector3 position, Vector3 direction, float scale)
    {
        Debug.DrawRay(position, Vector3.down * scale ,  color, scale);
    }

    #endregion

    //down 2
    #region down 2
    public void DebugRay_down2(Color color, Vector3 position, Vector3 direction, float scale)
    {
        Debug.DrawRay(position + Vector3.down * scale, Vector3.down * scale, color, scale);
    }

    #endregion

    //left forward 1
    #region left forward 1
    public void DebugRay_leftForward1(Color color, Vector3 position, Vector3 direction, float scale)
    {
        Debug.DrawRay(position, -Vector3.Cross(direction, Vector3.up) * scale ,  color, scale);
    }

    public void DebugRay_leftForward1direction1(Color color, Vector3 position, Vector3 direction, float scale)
    {
        Debug.DrawRay(position + direction * scale, -Vector3.Cross(direction, Vector3.up) * scale ,  color, scale);
    }

    public void DebugRay_leftForward1direction2(Color color, Vector3 position, Vector3 direction, float scale)
    {
        Debug.DrawRay(position + direction * scale * 2, -Vector3.Cross(direction, Vector3.up) * scale, color, scale);
    }

    #endregion

    //left forward 2
    #region left forward 2
    public void DebugRay_leftForward2direction1(Color color, Vector3 position, Vector3 direction, float scale)
    {
        Debug.DrawRay(position - Vector3.Cross(direction, Vector3.up) * scale * 2, direction * scale ,  color, scale);
    }

    #endregion

    //left back 1
    #region left back 1
    public void DebugRay_leftBack1(Color color, Vector3 position, Vector3 direction, float scale)
    {
        Debug.DrawRay(position, Vector3.Cross(direction, Vector3.up) * scale ,  color, scale);
    }

    public void DebugRay_leftBack1direction1(Color color, Vector3 position, Vector3 direction, float scale)
    {
        Debug.DrawRay(position + direction * scale, Vector3.Cross(direction, Vector3.up) * scale ,  color, scale);
    }

    #endregion

    //right forward 1
    #region right forward 1
    public void DebugRay_rightForward1(Color color, Vector3 position, Vector3 direction, float scale)
    {
        Debug.DrawRay(position, Vector3.Cross(direction, Vector3.up) * scale ,  color, scale);
    }

    public void DebugRay_rightForward1direction1(Color color, Vector3 position, Vector3 direction, float scale)
    {
        Debug.DrawRay(position + direction * scale, Vector3.Cross(direction, Vector3.up) * scale ,  color, scale);
    }

    public void DebugRay_rightForward1direction2(Color color, Vector3 position, Vector3 direction, float scale)
    {
        Debug.DrawRay(position + direction * scale * 2, Vector3.Cross(direction, Vector3.up) * scale, color, scale);
    }
    #endregion

    //right back
    #region right back 1
    public void DebugRay_rightBack1(Color color, Vector3 position, Vector3 direction, float scale)
    {
        Debug.DrawRay(position, -Vector3.Cross(direction, Vector3.up) * scale ,  color, scale);
    }

    public void DebugRay_rightBack1direction1(Color color, Vector3 position, Vector3 direction, float scale)
    {
        Debug.DrawRay(position + direction * scale, -Vector3.Cross(direction, Vector3.up) * scale ,  color, scale);
    }

    #endregion

    #endregion
}