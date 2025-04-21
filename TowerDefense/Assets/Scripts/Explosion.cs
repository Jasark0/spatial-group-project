using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private Animator animator;
    
    public AudioClip explosionSound; // Assign in inspector

    void Start()
    {
        SoundFXManager.Instance.PlaySound(explosionSound, transform, 1.0f, 10, 1.0f, 1.0f);
        animator = GetComponent<Animator>();
        animator.speed = 2f;
        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length / animator.speed;
        Destroy(gameObject, animationLength);
    }
}

