using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private Animator animator;
    
    public AudioClip explosionSound; // Assign in inspector
    [SerializeField] private float additionalDelay = 0.2f; // Extra time to ensure sound completes
    
    [Header("Area Damage Properties")]
    public float explosionRadius = 5f;
    public float damage = 10f;
    public string explosionOwner = ""; // "Turret" or "Enemy"
    
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
        
        // Apply damage if we have an owner set
        if (!string.IsNullOrEmpty(explosionOwner))
        {
            ApplyAreaDamage();
        }
    }
    
    public void Init(string owner, float damage = -1, float radius = -1)
    {
        explosionOwner = owner;
        
        if (damage > 0)
        {
            this.damage = damage;
        }
        
        if (radius > 0)
        {
            this.explosionRadius = radius;
        }
    }
    
    private void ApplyAreaDamage()
    {
        // Find all colliders within explosion radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        
        foreach (Collider col in hitColliders)
        {
            // Damage enemies if explosion is from a turret
            if (explosionOwner == "Turret" && col.CompareTag("Enemy"))
            {
                Balloon enemy = col.GetComponent<Balloon>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
            // Damage turrets if explosion is from an enemy
            else if (explosionOwner == "Enemy" && col.CompareTag("Turret"))
            {
                Turret turret = col.GetComponent<Turret>();
                if (turret != null)
                {
                    turret.TakeDamage(damage);
                }
            }
        }
    }
    
    // Visual debugging to show explosion radius in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

