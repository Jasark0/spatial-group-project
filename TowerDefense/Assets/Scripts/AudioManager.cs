using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioClip turretFireClip;
    public AudioClip explosionClip;
    public AudioClip enemyDetectedClip;
    public AudioClip reloadClip;
    public AudioClip backgroundMusicClip;
    public AudioClip laserClip;

    private AudioSource sfxSource;
    private AudioSource musicSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            sfxSource = gameObject.AddComponent<AudioSource>();
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayBackgroundMusic();
    }

    public void PlayTurretFire() => sfxSource.PlayOneShot(turretFireClip);
    public void PlayExplosion() => sfxSource.PlayOneShot(explosionClip);
    public void PlayEnemyDetected() => sfxSource.PlayOneShot(enemyDetectedClip);
    public void PlayReload() => sfxSource.PlayOneShot(reloadClip);
    public void PlayLaser() => sfxSource.PlayOneShot(laserClip);

    public void PlayBackgroundMusic()
    {
        if (backgroundMusicClip != null)
        {
            musicSource.clip = backgroundMusicClip;
            musicSource.Play();
        }
    }
}
