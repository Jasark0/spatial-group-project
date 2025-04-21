using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance;
    public SoundMixerManager soundMixerManager; // Reference to the SoundMixerManager
    [SerializeField] public AudioMixerGroup mixerGroup;
    [SerializeField] private AudioSource soundFXObject;
    
    [Header("Music Ducking Settings")]
    [SerializeField] private AudioMixer audioMixer; // Reference to the same mixer used in SoundMixerManager
    [SerializeField] private float duckingAmount = -8f; // dB to lower music by (negative value)
    [SerializeField] private float duckingFadeTime = 0.2f; // Time to fade music volume
    [SerializeField] private float duckingHoldTime = 1.0f; // Time to hold the lower volume
    
    private float originalMusicVolume;
    private Coroutine currentDuckingCoroutine;

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
        
        // Store the original music volume on startup
        audioMixer.GetFloat("MusicVolume", out originalMusicVolume);
    }

    public void PlaySound(AudioClip audioClip, Transform spawnTransform, float volume, int distance, float pitch = 1.0f, float spatial = 1.0f, bool duckMusic = false)
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
        
        // If requested, duck the music volume while this SFX plays
        if (duckMusic && audioMixer != null)
        {
            // Start the music ducking effect
            DuckMusic(clipLength);
        }
    }
    
    private void DuckMusic(float duration)
    {
        // If already ducking, stop that coroutine first
        if (currentDuckingCoroutine != null)
        {
            StopCoroutine(currentDuckingCoroutine);
        }
        
        // Start new ducking coroutine
        currentDuckingCoroutine = StartCoroutine(DuckMusicCoroutine(duration));
    }
    
    private IEnumerator DuckMusicCoroutine(float clipDuration)
    {
        // Get current music volume
        float currentVolume;
        audioMixer.GetFloat("MusicVolume", out currentVolume);
        
        // Calculate target ducked volume (don't go below -80dB)
        float duckedVolume = Mathf.Max(currentVolume + duckingAmount, -80f);
        
        // Fade music volume down
        float startTime = Time.time;
        float endTime = startTime + duckingFadeTime;
        
        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / duckingFadeTime;
            float newVolume = Mathf.Lerp(currentVolume, duckedVolume, t);
            audioMixer.SetFloat("MusicVolume", newVolume);
            yield return null;
        }
        
        // Ensure we reach the target volume exactly
        audioMixer.SetFloat("MusicVolume", duckedVolume);
        
        // Hold the ducked volume for either the clip duration or the minimum hold time, whichever is longer
        float holdDuration = Mathf.Max(clipDuration - duckingFadeTime * 2, duckingHoldTime);
        yield return new WaitForSeconds(holdDuration);
        
        // Fade back to original volume
        startTime = Time.time;
        endTime = startTime + duckingFadeTime;
        
        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / duckingFadeTime;
            float newVolume = Mathf.Lerp(duckedVolume, Mathf.Log10(soundMixerManager.currentMusicVolume) * 20f, t);
            audioMixer.SetFloat("MusicVolume", newVolume);
            yield return null;
        }
        
        // Ensure we return to the original volume exactly
        audioMixer.SetFloat("MusicVolume", originalMusicVolume);
        
        currentDuckingCoroutine = null;
    }
}
