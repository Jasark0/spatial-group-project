using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private Animator animator;
    
    public AudioClip explosionSound; // Assign in inspector
    [SerializeField] private float additionalDelay = 0.2f; // Extra time to ensure sound completes

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.speed = 2f;
        
        if (explosionSound != null)
        {
            // Play the explosion sound
            SoundFXManager.Instance.PlaySound(explosionSound, transform, 1.0f, 10, 1.0f, 1.0f);
            
            // Destroy after sound clip finishes (plus a small buffer)
            float soundDuration = explosionSound.length + additionalDelay;
            Destroy(gameObject, soundDuration);
        }
        else
        {
            // Fallback to animation length if no sound is assigned
            float animationLength = animator.GetCurrentAnimatorStateInfo(0).length / animator.speed;
            Destroy(gameObject, animationLength);
        }
    }
}

