using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance;
    [SerializeField] public AudioMixerGroup mixerGroup;
    [SerializeField] private AudioSource soundFXObject;

    private void Awake()
    {
        if(Instance == null) 
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySound(AudioClip audioClip, Transform spawnTransform, float volume, int distance, float pitch = 1.0f, float spatial = 1.0f)
    {

        //spawn in gameObject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        //assign the audioClip
        audioSource.clip = audioClip;

        //assign volume
        audioSource.outputAudioMixerGroup = mixerGroup;
        
        audioSource.volume = volume;

        audioSource.maxDistance = distance;

        audioSource.spatialBlend = spatial;

        audioSource.rolloffMode = AudioRolloffMode.Linear;

        audioSource.pitch = pitch;

        //play sound
        audioSource.Play();

        //get length of sound FX clip
        float clipLength = audioSource.clip.length / Mathf.Abs(pitch);

        //destroy the clip after it is done playing
        Destroy(audioSource.gameObject, clipLength);
    }
}
