using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Pixelplacement;

public class GameManger : MonoBehaviour
{
    [Header("Scene Indexes")]
    [SerializeField] int landSceneIndex;
    [SerializeField] int spaceSceneIndex;

    [Space]
    [Space]
    [Header("UI Image")]
    [SerializeField] Image whiteOut;
    [SerializeField] Color _transparent;
    [SerializeField] Color _white;
    [Space]
    [SerializeField] float blendLength;
    [SerializeField] float whiteOutDuration_in, fastWhiteOutDuration_in;
    [SerializeField] AnimationCurve whiteOutCurve_IN;
    [SerializeField] AnimationCurve whiteOutCurve_OUT;
    [Space]
    [SerializeField] float whiteOutDuration_out, fastWhiteOutDuration_out;

    [Space]
    [Space]
    [Header("Components")]
    [SerializeField] CameraController cameraController;
    [SerializeField] PlayerController player;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        cameraController = FindObjectOfType<CameraController>();
        player = FindObjectOfType<PlayerController>();
    }


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void EndGame()
    {
        StartCoroutine(EndGame_forrealsies());
    }

    public IEnumerator EndGame_forrealsies()
    {
        player.canMove = false;

        cameraController.TurnOnTransitionCamera_LandtoSpace();

        yield return new WaitUntil(() => cameraController.brain.ActiveBlend != null);

        // Get the length of the current blend as a float
        blendLength = cameraController.brain.ActiveBlend.Duration;

        // Start the white-out effect
        Pixelplacement.Tween.Color(whiteOut, Color.black, blendLength, 0f, whiteOutCurve_IN);

        yield return new WaitUntil(() => cameraController.brain.ActiveBlend == null);
    }

    public void StartTransition_Land_To_Space()
    {

        StartCoroutine(Land_to_Space_Transition());
    }

    private IEnumerator Land_to_Space_Transition()
    {
        player.canMove = false;

        cameraController.TurnOnTransitionCamera_LandtoSpace();
        
        yield return new WaitUntil(() => cameraController.brain.ActiveBlend != null);

        // Get the length of the current blend as a float
        blendLength = cameraController.brain.ActiveBlend.Duration;

        // Start the white-out effect
        Pixelplacement.Tween.Color(whiteOut, _white, blendLength, 0f, whiteOutCurve_IN);

        yield return new WaitUntil(() => cameraController.brain.ActiveBlend == null);

        //Debug.Log("before scene load line");

        LoadScene(spaceSceneIndex);

        //Debug.Log("After scene load line");

        // Wait until the scene is loaded
        yield return new WaitUntil(() => SceneManager.GetActiveScene().buildIndex == spaceSceneIndex);

        //Debug.Log("Scene loaded");

        // Start the white-out effect
        Pixelplacement.Tween.Color(whiteOut, _transparent, whiteOutDuration_out, 0f, whiteOutCurve_OUT);
    }

    public void FastFadeWhiteIn(float fadeOutDelay)
    {
        Pixelplacement.Tween.Color(whiteOut, _white, fastWhiteOutDuration_in, 0f, whiteOutCurve_IN);
        Invoke("FastFadeWhiteOut", fadeOutDelay);
    }

    public void FastFadeWhiteOut()
    {
        Pixelplacement.Tween.Color(whiteOut, _transparent, fastWhiteOutDuration_out, 0f, whiteOutCurve_IN);
    }

}
