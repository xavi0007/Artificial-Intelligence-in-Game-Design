using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SoundManager : MonoBehaviour
{
    // Audio players components.
    public AudioSource EffectsSource;
    public AudioSource MusicSource;
    // Random pitch adjustment range.
    public float LowPitchRange = .95f;
    public float HighPitchRange = 1.05f;
    // Singleton instance.
    public static SoundManager Instance = null;

    [Header("Audio List")]
    public List<AudioClip> audioClips;

    // Initialize the singleton instance.
    private void Awake()
    {
        // If there is not already an instance of SoundManager, set it to this.
        if (Instance == null)
        {
            Instance = this;
        }
        //If an instance already exists, destroy whatever this object is to enforce the singleton.
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);
    }


    // Play a single clip through the sound effects source.
    public void Play(AudioClip clipName)
    {
        EffectsSource.clip = clipName;
        EffectsSource.Play();
    }

    public void PlayName(string clipName)
    {


        EffectsSource.clip = getIndex(clipName);
        EffectsSource.Play();
    }    // Play a single clip through the sound effects source.


    public AudioClip getIndex(string clipName)
    {
        int idx = 0;
        switch (clipName)
        {
            case "Reload":
                idx = 0;
                break;
            case "BigBoss":
                idx = 1;
                break;
            case "GameOver":
                idx = 2;
                break;
            case "Annihilate":
                idx = 3;
                break;
            case "FinalMission":
                idx = 7;
                break;
            case "MissionComplete":
                idx = 4;
                break;
            case "Mission1":
                idx = 8;
                break;
            case "Mission2":
                idx = 6;
                break;
            case "CLassicAlarm":
                idx = 5;
                break;
            default:
             
                break;
        }

        return audioClips[idx];
        
    }


    // Play a single clip through the music source.
    public void PlayMusic(AudioClip clipName)
    {
        MusicSource.clip = clipName; 
        MusicSource.Play();
    }
    // Play a random clip from an array, and randomize the pitch slightly.
    public void RandomSoundEffect(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(LowPitchRange, HighPitchRange);
        EffectsSource.pitch = randomPitch;
        EffectsSource.clip = clips[randomIndex];
        EffectsSource.Play();
    }

}