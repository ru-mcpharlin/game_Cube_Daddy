using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class MusicController : MonoBehaviour
{
    [Header("Music Land")]
    public AudioSource[] musicLayers_land;
    public float[] musicVolumes_Land;
    [SerializeField] private int activeTrack_Land;
    public AnimationCurve fadeIn_Land;
    public float fadeinDuration_Land;

    [Header("SFX")]
    [Header("Wind")]
    public AudioSource wind;
    float windVolume;
    [Space]
    [Header("Roll")]
    public AudioSource roll_Source;
    public AudioClip[] roll_Clips;
    public float ROLL_VOLUME_MIN;
    public float ROLL_VOLUME_MAX;
    public float ROLL_PITCH_MIN;
    public float ROLL_PITCH_MAX;
    [Header("Merge")]
    public AudioSource mergeSource;
    public float MERGE_VOLUME;
    [Header("DeMerge")]
    public AudioSource demergeSource;
    public float DEMERGE_VOLUME;

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            AddLayer_Land();
        }
    }

    private void OnEnable()
    {
        AddLayer_Land();

    }
    public void AddLayer_Land()
    {
        if(activeTrack_Land < musicLayers_land.Length-1)
        {
            Tween.Volume(musicLayers_land[activeTrack_Land], musicVolumes_Land[activeTrack_Land], fadeinDuration_Land, 0, fadeIn_Land);
            activeTrack_Land++;
        }
    }

    public void RemoveAllLayers()
    {
        foreach(AudioSource _musicLayer in musicLayers_land)
        {
            Tween.Volume(musicLayers_land[activeTrack_Land], 0, fadeinDuration_Land, 0, fadeIn_Land);
            activeTrack_Land--;
        }
    }

    public void PlayRollSFX()
    {
        roll_Source.volume = Random.Range(ROLL_VOLUME_MIN, ROLL_VOLUME_MAX);
        roll_Source.pitch = Random.Range(ROLL_PITCH_MIN, ROLL_PITCH_MAX);
        roll_Source.PlayOneShot(roll_Clips[Random.Range(0, roll_Clips.Length-1)]);
    }

    public void PlayMergeSFX()
    {
        mergeSource.volume = MERGE_VOLUME;
        mergeSource.PlayOneShot(mergeSource.clip);
    }

    public void PlayDemergeSFX()
    {
        demergeSource.volume = DEMERGE_VOLUME;
        demergeSource.PlayOneShot(demergeSource.clip);
    }
}
