using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.speed = 2f;
        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length / animator.speed;
        Destroy(gameObject, animationLength);
    }
}

