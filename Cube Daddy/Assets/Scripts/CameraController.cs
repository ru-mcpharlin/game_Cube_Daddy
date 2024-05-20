using Cinemachine;
using JetBrains.Annotations;
using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    ////***********************************************//
    #region Variables
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

    [Header("Camera 3")]
    [SerializeField] public float camera3_mouseThreshold;
    [SerializeField] public float camera3_gamepadThreshold;
    [SerializeField] public float coolDownTimerMax;
    [SerializeField] public float coolDownTimer;

    [Header("Camera 5")]
    [SerializeField] public CinemachineOrbitalTransposer _5orbitalTransposer;
    [SerializeField] public Vector2 inputDelta;
    [Space]
    [SerializeField] public float MIN_HEIGHT;
    [SerializeField] public float MAX_HEIGHT;
    [Space]
    [SerializeField] public float _minHeightClamp;
    [SerializeField] public float _maxHeightClamp;
    [Space]
    [SerializeField] public float gamepadSpeed;
    [SerializeField] public float mouseSpeed;
    [Space]
    [SerializeField] public float _camera5MouseThreshold;
    [SerializeField] public float _camera5GamepadThreshold;
    [Space]
    [SerializeField] public float yValue;
    [SerializeField] public float ySpeed;
    [SerializeField] public float yThreshold;
    [Space]

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
        camera5_DynamicPerspective
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

                case CameraState.camera5_DynamicPerspective:
                    camera5_camera = vc;
                    _5orbitalTransposer = camera5_camera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
                    break;
            }
        }

        TurnCamera1On();

        //Shadows
        URP_Asset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
        URP_Asset.shadowDistance = SHADOW_DISTANCE;
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
            StartCoroutine(TurnCamera4On());
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            StartCoroutine(TurnCamera5On());
        }


        if (Input.GetKeyDown(KeyCode.Comma))
        {
            if(cameraState == CameraState.camera1_StaticIsometric)
            {
                if(camera1_index == 0)
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
                    camera3_index = camera3_cameras.Count-1;
                    TurnCamera3On();
                }
                else
                {
                    camera3_index--;
                    TurnCamera3On();
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.Period))
        {
            if (cameraState == CameraState.camera1_StaticIsometric)
            {
                if (camera1_index == camera1_cameras.Count-1)
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
                if (camera3_index == camera3_cameras.Count-1)
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

        if(coolDownTimer >= 0)
        {
            coolDownTimer -= Time.deltaTime;
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
        if (cameraState == CameraState.camera5_DynamicPerspective)
        {
            //x axis
            _5orbitalTransposer.m_XAxis.m_InputAxisValue = player.cameraVector_Mouse.x * mouseSpeed * Time.deltaTime + player.cameraVector_Gamepad.x * gamepadSpeed * Time.deltaTime;

            //y axis
            if (Mathf.Abs(player.cameraVector_Mouse.y) >= _camera5MouseThreshold || Mathf.Abs(player.cameraVector_Gamepad.y) >= _camera5GamepadThreshold)
            {
                yValue = player.cameraVector_Mouse.y + player.cameraVector_Gamepad.y;
                yValue = Mathf.Clamp(yValue, -1, 1);
                _5orbitalTransposer.m_FollowOffset.y += yValue * Time.deltaTime * ySpeed;
                _5orbitalTransposer.m_FollowOffset.y = Mathf.Clamp(_5orbitalTransposer.m_FollowOffset.y, _minHeightClamp, _maxHeightClamp);
            }

        }

        #endregion

        #region transition
        if(_transitioning)
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
    public IEnumerator TurnCamera5On()
    {
        TurnOffAllCameras();
        SetCameraState(CameraState.camera5_DynamicPerspective);
        camera5_camera.Priority = CAMERA_ON;
        player.vc_transform = camera5_camera.transform;

        yield return null;
    }


    public IEnumerator TurnCamera4On()
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

        StartCoroutine(TurnCamera5On());
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
        player.vc_transform = camera2_camera.transform;
    }

    public void TurnCamera1On()
    {
        TurnOffAllCameras();
        SetCameraState(CameraState.camera1_StaticIsometric);
        camera1_cameras[camera1_index].Priority = CAMERA_ON;
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

        cameraFollow._transitioning = true;
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
        camera2_camera.m_Lens.OrthographicSize = Mathf.Lerp(camera1_cameras[camera1_index].m_Lens.OrthographicSize, LENS_ORTHO_SIZE_SCALE * nextScale, t);
        camera2_camera.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = Mathf.Lerp(ISO_CAMERA_DISTANCE_SCALE * currentScale, ISO_CAMERA_DISTANCE_SCALE * nextScale, t);

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

        //TIMER
        timer += Time.deltaTime;
        t = transitionCurve.Evaluate(timer / TRANSITION_MAX_TIME);

        //IF TIMER EXCEEDS MAX TIME END TRANSITION
        if (timer >= TRANSITION_MAX_TIME)
        {
            cameraFollow._transitioning = false;
            _transitioning = false;
        }

    }

    #endregion
}
