using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //**********************************************************************************************************//
    #region Variables
    [Header("Components")]
    [SerializeField] PlayerController player;
    [SerializeField] CinemachineBrain brain;
    [SerializeField] Camera mainCamera;

    [Header("Camera State")]
    [SerializeField] public CameraState cameraState;
    [SerializeField] public bool isBlending;

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
    [SerializeField] public float gamepadSpeed;
    [SerializeField] public float mouseSpeed;
    [Space]
    [SerializeField] public float yValue;
    [Space]

    [Header("Index")]
    [SerializeField] int camera1_index;
    [SerializeField] int camera3_index;

    [Header("Static Variables")]
    [SerializeField] int CAMERA_OFF = 0;
    [SerializeField] int CAMERA_ON = 1;

    #endregion

    #region
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
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        brain = GetComponentInChildren<CinemachineBrain>();
        cameras = GetComponentsInChildren<CinemachineVirtualCamera>();
        mainCamera = GetComponentInChildren<Camera>();

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

        #region Camera 5 Control
        if (cameraState == CameraState.camera5_DynamicPerspective)
        {
            //mouse 
            if(player.cameraVector_Mouse.magnitude > player.cameraVector_Gamepad.magnitude)
            {
                _5orbitalTransposer.m_XAxis.m_InputAxisValue = player.cameraVector_Mouse.x * mouseSpeed * Time.deltaTime;
                yValue += player.cameraVector_Mouse.y * Time.deltaTime;
                _5orbitalTransposer.m_FollowOffset.y = Mathf.Clamp(yValue, 3, 10);
            }
            //gamepad
            else
            {
                _5orbitalTransposer.m_XAxis.m_InputAxisValue = player.cameraVector_Gamepad.x * gamepadSpeed * Time.deltaTime;
                yValue += player.cameraVector_Gamepad.y * Time.deltaTime;
                _5orbitalTransposer.m_FollowOffset.y = Mathf.Clamp(yValue, 3, 10);
            }

            

        }

        #endregion

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
    IEnumerator TurnCamera5On()
    {
        TurnOffAllCameras();
        SetCameraState(CameraState.camera5_DynamicPerspective);
        camera5_camera.Priority = CAMERA_ON;
        player.vc_transform = camera5_camera.transform;

        yield return null;
    }


    IEnumerator TurnCamera4On()
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

    private void TurnCamera3On()
    {
        TurnOffAllCameras();
        SetCameraState(CameraState.camera3_DynamicIsometric_Unlocked);
        camera3_cameras[camera3_index].Priority = CAMERA_ON;
        mainCamera.orthographic = true;
        player.vc_transform = camera3_cameras[camera3_index].transform;
    }

    private void TurnCamera2On()
    {
        TurnOffAllCameras();
        SetCameraState(CameraState.camera2_DynamicIsometric_Locked);
        camera2_camera.Priority = CAMERA_ON;
        player.vc_transform = camera2_camera.transform;
    }

    private void TurnCamera1On()
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
}
