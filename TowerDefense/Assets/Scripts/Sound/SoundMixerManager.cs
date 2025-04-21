using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    public float currentMasterVolume = 1.0f; //  volume level for Master
    public float currentSFXVolume = 1.0f; // volume level for Sound Effects
    public float currentMusicVolume = 1.0f; //  volume level for Music

    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(level) * 20f);
        
        // Debug the current value of MasterVolume parameter
        float currentValue;
        bool success = audioMixer.GetFloat("MasterVolume", out currentValue);
        currentMasterVolume = Mathf.Log10(level) * 20f; // Update the current master volume level
        // Debug.Log($"MasterVolume: Input={level}, SetTo={Mathf.Log10(level) * 20f}dB, Actual={currentValue}dB, Success={success}");
    }

    public void SetSoundFXVolume(float level)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(level) * 20f);
        
        // Debug the current value of SFXVolume parameter
        float currentValue;
        bool success = audioMixer.GetFloat("SFXVolume", out currentValue);
        currentSFXVolume = Mathf.Log10(level) * 20f; // Update the current SFX volume level
        // Debug.Log($"SFXVolume: Input={level}, SetTo={Mathf.Log10(level) * 20f}dB, Actual={currentValue}dB, Success={success}");
    }

    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(level) * 20f);
        
        // Debug the current value of MusicVolume parameter
        float currentValue;
        bool success = audioMixer.GetFloat("MusicVolume", out currentValue);
        currentMusicVolume = Mathf.Log10(level) * 20f; // Update the current music volume level
        // Debug.Log($"MusicVolume: Input={level}, SetTo={Mathf.Log10(level) * 20f}dB, Actual={currentValue}dB, Success={success}");
    }
}
