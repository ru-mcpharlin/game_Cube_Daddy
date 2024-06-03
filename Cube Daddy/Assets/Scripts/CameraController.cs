using Cinemachine;
using JetBrains.Annotations;
using Pixelplacement;
using Pixelplacement.TweenSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class CameraController : MonoBehaviour
{
    ////***********************************************//
    #region Variables
    [SerializeField] bool camera_Debug;

    [Header("Player Input")]
    #region player input variables
    [SerializeField] public InputMode inputMode;
    [Space]
    [SerializeField] public float yValue;
    [SerializeField] public float yThreshold_mouse;
    [SerializeField] public float yThreshold_gamepad;
    [Space]
    [SerializeField] public float xValue;
    [SerializeField] public float xThreshold_mouse;
    [SerializeField] public float xThreshold_gamepad;
    #endregion

    [Space]
    [Space]
    [Header("Camera State")]
    [SerializeField] public CameraState cameraState;

    [Space]
    [Space]
    [Header("Transition")]
    #region transition variables
    [Space]
    [SerializeField] public bool isTransitioning;
    //[SerializeField] 
    [Space]
    [SerializeField] public float currentScale;
    [SerializeField] public float nextScale;
    [Space]
    [SerializeField] public float t;
    [SerializeField] public float timer;
    [SerializeField] public float TRANSITION_MAX_TIME;
    [SerializeField] public AnimationCurve transitionCurve;
    [Space]
    [SerializeField] public UniversalRenderPipelineAsset URP_Asset;
    [Space]
    [SerializeField] public float MIN_MAIN_CAMERA_CLIPPING_PLAIN_ISO;
    [SerializeField] public float MAX_MAIN_CAMERA_CLIPPING_PLAIN_ISO;
    [Space]
    [SerializeField] public float SHADOW_DISTANCE;
    [Space]
    [SerializeField] public float LENS_ORTHO_SIZE_SCALE;
    [Space]
    [SerializeField] public float ISO_CAMERA_DISTANCE_SCALE;
    [Space]
    [SerializeField] public float PERSPECTIVE_CAMERA_DISTANCE_SCALE;
    #endregion

    [Space]
    [Space]
    [Header("Cameras")]
    #region cameras
    [SerializeField] CinemachineVirtualCamera[] cameras;
    [SerializeField] List<CinemachineVirtualCamera> camera1_cameras;
    [SerializeField] CinemachineVirtualCamera camera2_camera;
    [SerializeField] List<CinemachineVirtualCamera> camera3_cameras;
    [SerializeField] CinemachineVirtualCamera camera4_camera;
    [SerializeField] CinemachineVirtualCamera camera5_camera;
    [SerializeField] List<CinemachineVirtualCamera> camera6_cameras;
    [SerializeField] CinemachineVirtualCamera camera7_camera;
    [Space]
    [SerializeField] CinemachineVirtualCamera transitionCamera_LandtoSpace;
    #endregion

    [Space]
    [Space]
    [Header("Camera 1")]
    #region camera 1 variables
    [SerializeField] int camera1_index;
    #endregion

    [Space]
    [Space]
    [Header("Camera 3")]
    #region camera 3 variables
    [SerializeField] int camera3_index;
    [SerializeField] public float CAM3_COOLDOWN_MAX;
    [SerializeField] public float cam3_coolDownTimer;
    [SerializeField] public bool hasTurnedCamera3;
    [SerializeField] public UnityEvent hasTurnedCamera3_Event;
    #endregion

    [Space]
    [Space]
    [Header("Camera 5")]
    #region camera 5 variables
    [SerializeField] public CinemachineOrbitalTransposer cam5_orbitalTransposer;
    [Space]
    [SerializeField] public float _cam5_targetHeight;
    [SerializeField] public float _cam5_velocity;
    [SerializeField] public float _cam5_smoothDampDuration;
    [Space]
    [SerializeField] public float _cam5_minHeight;
    [SerializeField] public float _cam5_maxHeight;
    [Space]
    [SerializeField] public float CAM5_MIN_HEIGHT_UNSCALED;
    [SerializeField] public float CAM5_MAX_HEIGHT_UNSCALED;
    [Space]
    [SerializeField] public float CAM5_X_SENSITIVITY_GAMEPAD;
    [SerializeField] public float CAM5_Y_SENSITIVITY_GAMEPAD;
    [SerializeField] public float CAM5_X_SENSITIVITY_MOUSE;
    [SerializeField] public float CAM5_Y_SENSITIVITY_MOUSE;
    #endregion

    [Space]
    [Space]
    [Header("Camera 6")]
    #region camera 6 variables
    [SerializeField] int cam6_index;
    [Space]
    [SerializeField] CinemachineOrbitalTransposer cam6_orbitalTransposer;
    [SerializeField] Transform cam6_follow;
    [SerializeField] float cam6_height;
    [Space]
    [SerializeField] public float cam6_height_smoothDampTime;
    [SerializeField] public float cam6_height_smoothDampVelocity;
    [Space]
    [SerializeField] public Vector3 cam6_pos_smoothDampVelocity;
    [SerializeField] public float cam6_pos_smoothDampTime;
    [Space]
    [Space]
    [SerializeField] float CAM6_HEIGHTCLAMP_MAX;
    [SerializeField] float CAM6_HEIGHTCLAMP_MIN;
    [Space]
    [SerializeField] public float CAM6_X_SENSITIVITY_GAMEPAD;
    [SerializeField] public float CAM6_Y_SENSITIVITY_GAMEPAD;
    [SerializeField] public float CAM6_X_SENSITIVITY_MOUSE;
    [SerializeField] public float CAM6_Y_SENSITIVITY_MOUSE;
    #endregion

    #region misc variables
    [HideInInspector] static int CAMERA_OFF = 0;
    [HideInInspector] static int CAMERA_ON = 1;

    [HideInInspector] PlayerController player;
    [HideInInspector] public CinemachineBrain brain;
    [HideInInspector] Camera mainCamera;
    [HideInInspector] public CameraFollow cameraFollow;
    #endregion

    #endregion

    //ENUM
    #region Enum
    public enum CameraState
    {
        camera1_StaticIsometric,
        camera2_DynamicIsometric_Locked,
        camera3_DynamicIsometric_Unlocked,
        camera4_PerspectiveMatchCut,
        camera5_DynamicPerspective_Limited,
        camera6_DynamicPerspective_Free,
        camera7_BlackHole,
        transitionCamera_landToSpace
    }

    public void SetCameraState(CameraState inputState)
    {
        cameraState = inputState;
    }

    public enum InputMode
    {
        Mouse,
        Gamepad
    }
    #endregion

    //START
    #region Start
    // Start is called before the first frame update
    void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        brain = GetComponentInChildren<CinemachineBrain>();
        cameras = GetComponentsInChildren<CinemachineVirtualCamera>();
        mainCamera = GetComponentInChildren<Camera>();
        cameraFollow = GetComponentInChildren<CameraFollow>();
        

        foreach(CinemachineVirtualCamera vc in cameras)
        {
            switch(vc.GetComponent<VirtualCameraController>().cameraType)
            {
                case CameraState.camera1_StaticIsometric:
                    camera1_cameras.Add(vc);
                    break;

                case CameraState.camera2_DynamicIsometric_Locked:
                    camera2_camera = vc;
                    break;

                case CameraState.camera3_DynamicIsometric_Unlocked:
                    camera3_cameras.Add(vc);
                    break;

                case CameraState.camera4_PerspectiveMatchCut:
                    camera4_camera = vc;
                    break;

                case CameraState.camera5_DynamicPerspective_Limited:
                    camera5_camera = vc;
                    cam5_orbitalTransposer = camera5_camera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
                    break;

                case CameraState.camera6_DynamicPerspective_Free:
                    camera6_cameras.Add(vc);
                    break;

                case CameraState.transitionCamera_landToSpace:
                    transitionCamera_LandtoSpace = vc;
                    break;
            }
        }

        //Shadows
        URP_Asset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
        URP_Asset.shadowDistance = SHADOW_DISTANCE;

        switch (cameraState)
        {
            case CameraState.camera1_StaticIsometric:
                TurnCamera1On();

                break;

            case CameraState.camera2_DynamicIsometric_Locked:
                TurnCamera2On();
                break;

            case CameraState.camera3_DynamicIsometric_Unlocked:
                TurnCamera3On();
                break;

            case CameraState.camera4_PerspectiveMatchCut:
                TurnCamera4On();
                break;

            case CameraState.camera5_DynamicPerspective_Limited:
                TurnCamera5On();
                break;

            case CameraState.camera6_DynamicPerspective_Free:
                TurnCamera6On();
                break;

            case CameraState.camera7_BlackHole:
                break;
        }
    }

    private void Start()
    {
        cameraFollow.currentCubeTransform = player.cubeTransform;
    }
    #endregion

    //UPDATE
    #region Update
    // Update is called once per frame
    void Update()
    {
        //DEBUGGING
        #region Debug
        Camera_Debug();
        #endregion

        //PLAYER INPUT
        #region Player Input
        GetPlayerInput();

        #endregion

        //TRANSITION
        #region transition
        if (isTransitioning)
        {
            TransitionCamera();
        }
        #endregion

        //CAMERA 3
        #region Camera 3 Control
        if (cameraState == CameraController.CameraState.camera3_DynamicIsometric_Unlocked)
        {
            if (cam3_coolDownTimer >= 0)
            {
                cam3_coolDownTimer -= Time.deltaTime;
            }

            if (xValue >= 0.99f)
            {
                DecreaseCamera3Index();
            }
            else if (xValue <= -0.99f)
            {
                IncreaseCamera3Index();
            }
        }

        #endregion       

        //CAMERA 5
        #region Camera 5 Control
        if (cameraState == CameraState.camera5_DynamicPerspective_Limited)
        {
            if(inputMode == InputMode.Mouse)
            {
                //x axis
                cam5_orbitalTransposer.m_XAxis.m_InputAxisValue += xValue * CAM5_X_SENSITIVITY_MOUSE;

                //y axis
                _cam5_targetHeight += yValue * CAM5_Y_SENSITIVITY_MOUSE * player.currentScale;
                _cam5_targetHeight = Mathf.Clamp(_cam5_targetHeight, _cam5_minHeight, _cam5_maxHeight);

                cam5_orbitalTransposer.m_FollowOffset.y = Mathf.SmoothDamp(cam5_orbitalTransposer.m_FollowOffset.y, _cam5_targetHeight, ref _cam5_velocity, _cam5_smoothDampDuration);
                cam5_orbitalTransposer.m_FollowOffset.y = Mathf.Clamp(cam5_orbitalTransposer.m_FollowOffset.y, _cam5_minHeight, _cam5_maxHeight);
            }

            else if(inputMode == InputMode.Gamepad)
            {
                //x axis
                cam5_orbitalTransposer.m_XAxis.m_InputAxisValue += xValue * CAM5_X_SENSITIVITY_GAMEPAD;

                //y axis
                _cam5_targetHeight += yValue * CAM5_Y_SENSITIVITY_GAMEPAD * player.currentScale;
                _cam5_targetHeight = Mathf.Clamp(_cam5_targetHeight, _cam5_minHeight, _cam5_maxHeight);

                cam5_orbitalTransposer.m_FollowOffset.y = Mathf.SmoothDamp(cam5_orbitalTransposer.m_FollowOffset.y, _cam5_targetHeight, ref _cam5_velocity, _cam5_smoothDampDuration);
                cam5_orbitalTransposer.m_FollowOffset.y = Mathf.Clamp(_cam5_targetHeight, _cam5_minHeight, _cam5_maxHeight);
            }
        }

        #endregion
         
        //CAMERA 6
        #region Camera 6 Control
        //if in camera 6 state
        else if (cameraState == CameraState.camera6_DynamicPerspective_Free)
        {
            //mouse
            if (inputMode == InputMode.Mouse)
            {
                cam6_orbitalTransposer.m_XAxis.m_InputAxisValue = xValue * CAM6_X_SENSITIVITY_MOUSE;

                cam6_height = cam6_height + yValue * CAM6_Y_SENSITIVITY_MOUSE;
            }
            //gamepad
            else if (inputMode == InputMode.Gamepad)
            {
                cam6_orbitalTransposer.m_XAxis.m_InputAxisValue = xValue * CAM6_X_SENSITIVITY_GAMEPAD;

                cam6_height = cam6_height + yValue * CAM6_Y_SENSITIVITY_GAMEPAD;
            }

            //height
            cam6_orbitalTransposer.m_FollowOffset.y = Mathf.SmoothDamp(cam6_orbitalTransposer.m_FollowOffset.y, cam6_height, ref cam6_height_smoothDampVelocity, cam6_height_smoothDampTime);

            //clamp the height
            cam6_height = Mathf.Clamp(cam6_height, CAM6_HEIGHTCLAMP_MIN * player.currentScale, CAM6_HEIGHTCLAMP_MAX * player.currentScale);

            //smooth damp the position
            cam6_follow.position = Vector3.SmoothDamp(cam6_follow.position, player.cubeTransform.position, ref cam6_pos_smoothDampVelocity, cam6_pos_smoothDampTime);
        }

        #endregion



    }

    #endregion

    //DEBUGGING
    #region debugging
    private void Camera_Debug()
    {
        if (Input.GetKeyDown(KeyCode.Tilde))
        {
            camera_Debug = !camera_Debug;
        }

        if (camera_Debug)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                TurnCamera1On();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                TurnCamera2On();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                TurnCamera3On();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                TurnCamera4On();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                TurnCamera5On();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                TurnCamera6On();
            }


            if (Input.GetKeyDown(KeyCode.Comma))
            {
                if (cameraState == CameraState.camera1_StaticIsometric)
                {
                    if (camera1_index == 0)
                    {
                        camera1_index = camera1_cameras.Count - 1;
                        TurnCamera1On();
                    }
                    else
                    {
                        camera1_index--;
                        TurnCamera1On();
                    }
                }

                if (cameraState == CameraState.camera3_DynamicIsometric_Unlocked)
                {
                    if (camera3_index == 0)
                    {
                        camera3_index = camera3_cameras.Count - 1;
                        TurnCamera3On();
                    }
                    else
                    {
                        camera3_index--;
                        TurnCamera3On();
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Period))
            {
                if (cameraState == CameraState.camera1_StaticIsometric)
                {
                    if (camera1_index == camera1_cameras.Count - 1)
                    {
                        camera1_index = 0;
                        TurnCamera1On();
                    }
                    else
                    {
                        camera1_index++;
                        TurnCamera1On();
                    }
                }

                if (cameraState == CameraState.camera3_DynamicIsometric_Unlocked)
                {
                    if (camera3_index == camera3_cameras.Count - 1)
                    {
                        camera3_index = 0;
                        TurnCamera3On();
                    }
                    else
                    {
                        camera3_index++;
                        TurnCamera3On();
                    }
                }
            }
        }
    }
    #endregion

    //PLAYER INPUT
    #region Player Input
    private void GetPlayerInput()
    {
        if (Mathf.Abs(player.cameraVector_Mouse.y) >= yThreshold_mouse || Mathf.Abs(player.cameraVector_Gamepad.y) >= yThreshold_gamepad)
        {
            yValue = player.cameraVector_Mouse.y + player.cameraVector_Gamepad.y;
            yValue = Mathf.Clamp(yValue, -1, 1);
        }
        else
        {
            yValue = 0;
        }

        if (Mathf.Abs(player.cameraVector_Mouse.x) >= xThreshold_mouse || Mathf.Abs(player.cameraVector_Gamepad.x) >= xThreshold_gamepad)
        {
            xValue = player.cameraVector_Mouse.x + player.cameraVector_Gamepad.x;
            xValue = Mathf.Clamp(xValue, -1, 1);
        }
        else
        {
            xValue = 0;
        }



        if (Mathf.Abs(player.cameraVector_Mouse.magnitude) > yThreshold_mouse)
        {
            inputMode = InputMode.Mouse;
        }
        if (Mathf.Abs(player.cameraVector_Gamepad.magnitude) > yThreshold_gamepad)
        {
            inputMode = InputMode.Gamepad;
        }
    }
    #endregion

    //CAMERA 1
    #region Camera 1
    public void SetCamera1Index(int inputIndex)
    {
        camera1_index = inputIndex;
        TurnCamera1On();
    }

    #endregion

    //CAMERA 3
    #region Camera 3
    public void IncreaseCamera3Index()
    {
        if (cam3_coolDownTimer <= 0)
        {
            if (camera3_index == camera3_cameras.Count - 1)
            {
                camera3_index = 0;
                TurnCamera3On();
                cam3_coolDownTimer = CAM3_COOLDOWN_MAX;
            }
            else
            {
                camera3_index++;
                TurnCamera3On();
                cam3_coolDownTimer = CAM3_COOLDOWN_MAX;
            }

            if(!hasTurnedCamera3)
            {
                hasTurnedCamera3 = true;
                hasTurnedCamera3_Event.Invoke();
            }
        }
    }

    public void DecreaseCamera3Index()
    {
        if (cam3_coolDownTimer <= 0)
        {
            if (camera3_index == 0)
            {
                camera3_index = camera3_cameras.Count - 1;
                TurnCamera3On();
                cam3_coolDownTimer = CAM3_COOLDOWN_MAX;
            }
            else
            {
                camera3_index--;
                TurnCamera3On();
                cam3_coolDownTimer = CAM3_COOLDOWN_MAX;
            }

            if (!hasTurnedCamera3)
            {
                hasTurnedCamera3 = true;
                hasTurnedCamera3_Event.Invoke();
            }
        }
    }
    #endregion

    //CAMERA 6
    #region Camera 6

    public void SetCamera6Index(int inputIndex)
    {
        cam6_index = inputIndex;
        TurnCamera6On();
    }


    #endregion

    //TURN CAMERAS ON
    #region Turn Cameras On


    public void TurnCamera6On()
    {

        TurnOffAllCameras();
        SetCameraState(CameraState.camera6_DynamicPerspective_Free);
        camera6_cameras[cam6_index].Priority = CAMERA_ON;

        cam6_orbitalTransposer = camera6_cameras[cam6_index].GetCinemachineComponent<CinemachineOrbitalTransposer>();
        player.vc_transform = camera6_cameras[cam6_index].transform;

        mainCamera.orthographic = false;
    }

    public void TurnCamera5On()
    {
        TurnOffAllCameras();
        SetCameraState(CameraState.camera5_DynamicPerspective_Limited);
        camera5_camera.Priority = CAMERA_ON;
        player.vc_transform = camera5_camera.transform;

        mainCamera.orthographic = false;
    }

    public void TurnCamera4On()
    {
        TurnCamera4On_Coroutine();

    }

    public IEnumerator TurnCamera4On_Coroutine()
    {
        TurnOffAllCameras();
        SetCameraState(CameraState.camera4_PerspectiveMatchCut);

        camera4_camera.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset =
            camera3_cameras[camera3_index].GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset / 2;

        yield return new WaitForSeconds(1);

        camera4_camera.Priority = CAMERA_ON;
        mainCamera.orthographic = false;
        player.vc_transform = camera4_camera.transform;

        yield return new WaitForSeconds(1);

        TurnCamera5On();
    }

    public void TurnCamera3On()
    {
        TurnOffAllCameras();
        SetCameraState(CameraState.camera3_DynamicIsometric_Unlocked);
        camera3_cameras[camera3_index].Priority = CAMERA_ON;
        mainCamera.orthographic = true;
        player.vc_transform = camera3_cameras[camera3_index].transform;
    }

    public void TurnCamera2On()
    {
        TurnOffAllCameras();
        SetCameraState(CameraState.camera2_DynamicIsometric_Locked);
        camera2_camera.Priority = CAMERA_ON;
        mainCamera.orthographic = true;
        player.vc_transform = camera2_camera.transform;
    }

    public void TurnCamera1On()
    {
        TurnOffAllCameras();
        SetCameraState(CameraState.camera1_StaticIsometric);
        camera1_cameras[camera1_index].Priority = CAMERA_ON;
        mainCamera.orthographic = true;
        player.vc_transform = camera1_cameras[camera1_index].transform;
    }

    public void TurnOffAllCameras()
    {
        foreach(CinemachineVirtualCamera vc in cameras)
        {
            vc.Priority = CAMERA_OFF;
        }
    }

    public void TurnOnTransitionCamera_LandtoSpace()
    {
        transitionCamera_LandtoSpace.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.Value = camera5_camera.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.Value;
        TurnOffAllCameras();

        transitionCamera_LandtoSpace.Priority = CAMERA_ON;
    }


    #endregion

    //TRANSITION CAMERAS
    #region Transition Cameras
    public IEnumerator StartCameraTransition(float inputCurrentScale, float inputNextScale)
    {
        timer = 0;
        t = 0;
        currentScale = inputCurrentScale;
        nextScale = inputNextScale;

        yield return new WaitForEndOfFrame();

        isTransitioning = true;
    }


    public void TransitionCamera()
    {
        //SHADOWS
        URP_Asset.shadowDistance = Mathf.Lerp(SHADOW_DISTANCE * currentScale, SHADOW_DISTANCE * nextScale, t);

        //CLIPPING PLAIN
        if (cameraState != CameraState.camera1_StaticIsometric)
        {
            foreach (CinemachineVirtualCamera vc in cameras)
            {
                vc.m_Lens.FarClipPlane = Mathf.Lerp(MAX_MAIN_CAMERA_CLIPPING_PLAIN_ISO * currentScale, MAX_MAIN_CAMERA_CLIPPING_PLAIN_ISO * nextScale, t);
                vc.m_Lens.NearClipPlane = Mathf.Lerp(MIN_MAIN_CAMERA_CLIPPING_PLAIN_ISO * currentScale, MIN_MAIN_CAMERA_CLIPPING_PLAIN_ISO * nextScale, t);
            }
        }

        //// CAMERA 1 ////
        foreach (CinemachineVirtualCamera vc in camera1_cameras)
        {
            //vc.m_Lens.OrthographicSize = Mathf.Lerp(LENS_ORTHO_SIZE_SCALE * currentScale, LENS_ORTHO_SIZE_SCALE * nextScale, t);
            //vc.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = Mathf.Lerp(ISO_CAMERA_DISTANCE_SCALE * currentScale, ISO_CAMERA_DISTANCE_SCALE * nextScale, t);
        }

        //// CAMERA 2 ////
        if (camera1_cameras[camera1_index].Priority == 1)
        {
            camera2_camera.m_Lens.OrthographicSize = Mathf.Lerp(LENS_ORTHO_SIZE_SCALE * currentScale, LENS_ORTHO_SIZE_SCALE * nextScale, t);
            camera2_camera.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = Mathf.Lerp(ISO_CAMERA_DISTANCE_SCALE * currentScale, ISO_CAMERA_DISTANCE_SCALE * nextScale, t);
        }
        else
        {
            camera2_camera.m_Lens.OrthographicSize = Mathf.Lerp(LENS_ORTHO_SIZE_SCALE * currentScale, LENS_ORTHO_SIZE_SCALE * nextScale, t);
            camera2_camera.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = Mathf.Lerp(ISO_CAMERA_DISTANCE_SCALE * currentScale, ISO_CAMERA_DISTANCE_SCALE * nextScale, t);
        }
        

        //// CAMERA 3 ////
        foreach (CinemachineVirtualCamera vc in camera3_cameras)
        {
            vc.m_Lens.OrthographicSize = Mathf.Lerp(LENS_ORTHO_SIZE_SCALE * currentScale, LENS_ORTHO_SIZE_SCALE * nextScale, t);
            vc.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = Mathf.Lerp(ISO_CAMERA_DISTANCE_SCALE * currentScale, ISO_CAMERA_DISTANCE_SCALE * nextScale, t);
        }

        //CAMERA 4

        //CAMERA 5
        //scale * -5 orbital transposer Z follow offset
        camera5_camera.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_FollowOffset.z = Mathf.Lerp(PERSPECTIVE_CAMERA_DISTANCE_SCALE * currentScale, PERSPECTIVE_CAMERA_DISTANCE_SCALE * nextScale, t);
        _cam5_minHeight = Mathf.Lerp(CAM5_MIN_HEIGHT_UNSCALED * currentScale, CAM5_MIN_HEIGHT_UNSCALED * nextScale, t);
        _cam5_maxHeight = Mathf.Lerp(CAM5_MAX_HEIGHT_UNSCALED * currentScale, CAM5_MAX_HEIGHT_UNSCALED * nextScale, t);

        //CAMERA 6

        

        //TIMER
        timer += Time.deltaTime;
        t = transitionCurve.Evaluate(timer / TRANSITION_MAX_TIME);

        //IF TIMER EXCEEDS MAX TIME END TRANSITION
        if (timer >= TRANSITION_MAX_TIME)
        {
            isTransitioning = false;
        }

    }

    #endregion
}
