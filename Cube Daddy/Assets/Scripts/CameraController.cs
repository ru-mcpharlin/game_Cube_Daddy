using Cinemachine;
using JetBrains.Annotations;
using Pixelplacement;
using Pixelplacement.TweenSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class CameraController : MonoBehaviour
{
    ////***********************************************//
    #region Variables
    [Header("Player Input")]
    [Space]
    [SerializeField] public float yValue;
    [SerializeField] public float ySpeed;
    [SerializeField] public float yThreshold_mouse;
    [SerializeField] public float yThreshold_gamepad;
    [Space]
    [SerializeField] public float xValue;
    [SerializeField] public float xSpeed;
    [SerializeField] public float xThreshold_mouse;
    [SerializeField] public float xThreshold_gamepad;

    [Header("Components")]
    [SerializeField] PlayerController player;
    [SerializeField] CinemachineBrain brain;
    [SerializeField] Camera mainCamera;
    [SerializeField] public CameraFollow cameraFollow;

    [Header("Camera State")]
    [SerializeField] public CameraState cameraState;
    [SerializeField] public bool isBlending;

    [Header("Camera Scaling Variables")]
    [Space]
    [SerializeField] public bool _transitioning;
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

    [Header("Cameras")]
    [SerializeField] CinemachineVirtualCamera[] cameras;
    [SerializeField] List<CinemachineVirtualCamera> camera1_cameras;
    [SerializeField] CinemachineVirtualCamera camera2_camera;
    [SerializeField] List<CinemachineVirtualCamera> camera3_cameras;
    [SerializeField] CinemachineVirtualCamera camera4_camera;
    [SerializeField] CinemachineVirtualCamera camera5_camera;
    [SerializeField] CinemachineVirtualCamera camera6_camera;
    [SerializeField] CinemachineVirtualCamera camera7_camera;

    [Header("Camera 3")]
    [SerializeField] public float camera3_mouseThreshold;
    [SerializeField] public float camera3_gamepadThreshold;
    [SerializeField] public float coolDownTimerMax;
    [SerializeField] public float coolDownTimer;

    [Header("Camera 5")]
    [SerializeField] public CinemachineOrbitalTransposer _5orbitalTransposer;
    [Space]
    [SerializeField] public float MIN_HEIGHT;
    [SerializeField] public float MAX_HEIGHT;
    [Space]
    [SerializeField] public float _minHeightClamp;
    [SerializeField] public float _maxHeightClamp;
    [Space]
    [SerializeField] public float _cam5_gamepadSpeed;
    [SerializeField] public float _cam5_mouseSpeed;

    [Space]

    [Header("Camera 6")]
    [SerializeField] CinemachineOrbitalTransposer cam6_orbitalTransposer;
    [SerializeField] Transform cam6_follow;
    [SerializeField] float cam6_height;
    [Space]
    [SerializeField] public float height_smoothDampTime;
    [SerializeField] public float height_smoothDampVelocity;
    [Space]
    [SerializeField] public Vector3 pos_smoothDampVelocity;
    [SerializeField] public float pos_smoothDampTime;

    [Space]
    [SerializeField] float CAM6_HEIGHTCLAMP_MAX;
    [SerializeField] float CAM6_HEIGHTCLAMP_MIN;
    [Space]
    [SerializeField] public float CAM6_SPEED_GAMEPAD;
    [SerializeField] public float CAM6_SPEED_MOUSE;


    [Header("Index")]
    [SerializeField] int camera1_index;
    [SerializeField] int camera3_index;

    [Header("Static Variables")]
    [SerializeField] int CAMERA_OFF = 0;
    [SerializeField] int CAMERA_ON = 1;

    #endregion

    #region Enum
    public enum CameraState
    {
        camera1_StaticIsometric,
        camera2_DynamicIsometric_Locked,
        camera3_DynamicIsometric_Unlocked,
        camera4_PerspectiveMatchCut,
        camera5_DynamicPerspective_Limited,
        camera6_DynamicPerspective_Free,
        camera7_BlackHole
    }

    public void SetCameraState(CameraState inputState)
    {
        cameraState = inputState;
    }
    #endregion

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
                    _5orbitalTransposer = camera5_camera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
                    break;

                case CameraState.camera6_DynamicPerspective_Free:
                    camera6_camera = vc;
                    cam6_orbitalTransposer = camera6_camera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
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

    #region Update
    // Update is called once per frame
    void Update()
    {
        #region Debug

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
        #endregion

        #region isBlending && Timer
        if (brain.ActiveBlend != null)
        {
            isBlending = true;
        }
        else
        {
            isBlending = false;
        }

        if (coolDownTimer >= 0)
        {
            coolDownTimer -= Time.deltaTime;
        }
        #endregion

        #region Player Input
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

        #endregion


        #region Camera 3 Control
        if (cameraState == CameraController.CameraState.camera3_DynamicIsometric_Unlocked)
        {
            if (Mathf.Abs(player.cameraVector_Gamepad.x) >= camera3_gamepadThreshold)
            {
                if (player.cameraVector_Gamepad.x > 0)
                {
                    DecreaseCamera3Index();
                }
                else
                {
                    IncreaseCamera3Index();
                }
            }
            else if (Mathf.Abs(player.cameraVector_Mouse.x) >= camera3_mouseThreshold)
            {
                if (player.cameraVector_Mouse.x > 0)
                {
                    DecreaseCamera3Index();
                }
                else
                {
                    IncreaseCamera3Index();
                }
            }
        }


        #endregion       


        #region Camera 5 Control
        if (cameraState == CameraState.camera5_DynamicPerspective_Limited)
        {
            //x axis
            _5orbitalTransposer.m_XAxis.m_InputAxisValue = player.cameraVector_Mouse.x * _cam5_mouseSpeed * Time.deltaTime + player.cameraVector_Gamepad.x * _cam5_gamepadSpeed * Time.deltaTime;

            //y axis
            if (Mathf.Abs(player.cameraVector_Mouse.y) >= yThreshold_mouse || Mathf.Abs(player.cameraVector_Gamepad.y) >= yThreshold_gamepad)
            {
                _5orbitalTransposer.m_FollowOffset.y += yValue * Time.deltaTime * ySpeed;
                _5orbitalTransposer.m_FollowOffset.y = Mathf.Clamp(_5orbitalTransposer.m_FollowOffset.y, _minHeightClamp, _maxHeightClamp);
            }
        }

        #endregion

        #region Camera 6 Control
        //if in camera 6 state
        else if (cameraState == CameraState.camera6_DynamicPerspective_Free)
        {

            cam6_orbitalTransposer.m_XAxis.m_InputAxisValue = xValue;

            cam6_height = Mathf.SmoothDamp(cam6_height, cam6_height + yValue, ref height_smoothDampVelocity, height_smoothDampTime);

            cam6_height = Mathf.Clamp(cam6_height, CAM6_HEIGHTCLAMP_MIN * player.currentScale, CAM6_HEIGHTCLAMP_MAX * player.currentScale);

            cam6_orbitalTransposer.m_FollowOffset.y = cam6_height;

            cam6_follow.position = Vector3.SmoothDamp(cam6_follow.position, player.cubeTransform.position, ref pos_smoothDampVelocity, pos_smoothDampTime);
           
        }

        #endregion

        #region transition
        if (_transitioning)
        {
            ScaleCamera();
        }


        #endregion

    }
    #endregion

    #region Camera 1
    public void SetCamera1Index(int inputIndex)
    {
        camera1_index = inputIndex;
        TurnCamera1On();
    }

    #endregion

    #region Camera 3
    public void IncreaseCamera3Index()
    {
        if (coolDownTimer <= 0)
        {
            if (camera3_index == camera3_cameras.Count - 1)
            {
                camera3_index = 0;
                TurnCamera3On();
                coolDownTimer = coolDownTimerMax;
            }
            else
            {
                camera3_index++;
                TurnCamera3On();
                coolDownTimer = coolDownTimerMax;
            }
        }
    }

    public void DecreaseCamera3Index()
    {
        if (coolDownTimer <= 0)
        {
            if (camera3_index == 0)
            {
                camera3_index = camera3_cameras.Count - 1;
                TurnCamera3On();
                coolDownTimer = coolDownTimerMax;
            }
            else
            {
                camera3_index--;
                TurnCamera3On();
                coolDownTimer = coolDownTimerMax;
            }
        }
    }
    #endregion

    #region Turn Cameras On

    public void TurnCamera6On()
    {

        TurnOffAllCameras();
        SetCameraState(CameraState.camera6_DynamicPerspective_Free);
        camera6_camera.Priority = CAMERA_ON;
        player.vc_transform = camera6_camera.transform;

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


    #endregion

    #region Scale Cameras
    public IEnumerator StartCameraScale(float inputCurrentScale, float inputNextScale)
    {
        timer = 0;
        t = 0;
        currentScale = inputCurrentScale;
        nextScale = inputNextScale;

        yield return new WaitForEndOfFrame();

        _transitioning = true;
    }


    public void ScaleCamera()
    {
        //CAMERA FOLLOW
        //cameraFollow.transform.position = Vector3.Lerp(cameraFollow.transform.position, player.cubeTransform.position, t);

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
            camera2_camera.m_Lens.OrthographicSize = Mathf.Lerp(camera1_cameras[camera1_index].m_Lens.OrthographicSize, LENS_ORTHO_SIZE_SCALE * nextScale, t);
            camera2_camera.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = Mathf.Lerp(ISO_CAMERA_DISTANCE_SCALE * currentScale, ISO_CAMERA_DISTANCE_SCALE * nextScale, t);
        }
        else
        {
            camera2_camera.m_Lens.OrthographicSize = Mathf.Lerp(camera2_camera.m_Lens.OrthographicSize, LENS_ORTHO_SIZE_SCALE * nextScale, t);
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
        _minHeightClamp = Mathf.Lerp(MIN_HEIGHT * currentScale, MIN_HEIGHT * nextScale, t);
        _maxHeightClamp = Mathf.Lerp(MAX_HEIGHT * currentScale, MAX_HEIGHT * nextScale, t);

        //camera 6
        

        //TIMER
        timer += Time.deltaTime;
        t = transitionCurve.Evaluate(timer / TRANSITION_MAX_TIME);

        //IF TIMER EXCEEDS MAX TIME END TRANSITION
        if (timer >= TRANSITION_MAX_TIME)
        {
            _transitioning = false;
        }

    }

    #endregion
}
