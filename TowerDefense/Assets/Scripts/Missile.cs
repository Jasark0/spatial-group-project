using UnityEngine;

public class Missile : Bullet
{
    [Header("Missile Properties")]
    public float explosionRadius = 3f;
    public GameObject explosionEffectPrefab;
    public float missileSpeed = 15f; // Missiles are typically faster than bullets

    private void Start()
    {
        // Override the bullet's speed with missile speed
        speed = missileSpeed;
    }

    protected override void Update()
    {
        // get rid of update logic from bullet
        if (shot)
        {
            return;
        }
    }

    // Override the OnTriggerEnter method from Bullet
    void OnTriggerEnter(Collider other)
    {
        // Don't hit the owner
        if (other.CompareTag(bulletOwner))
            return;

        // Handle missile-specific behavior on impact
        if ((other.CompareTag("Enemy") && bulletOwner == "Turret") ||
            (other.CompareTag("Turret") && bulletOwner == "Enemy") ||
            other.CompareTag("Plane"))
        {
            ExplodeAndDamageArea();
            Destroy(gameObject);
        }
    }

    // Override OnCollisionEnter to handle physical collisions
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(bulletOwner))
            return;

        // Explode on any collision
        ExplodeAndDamageArea();
        Destroy(gameObject);
    }

    // Missile-specific explosion method
    private void ExplodeAndDamageArea()
    {
        // Create explosion effect
        if (explosionEffectPrefab != null)
        {
            GameObject explosion = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            if (explosion.TryGetComponent<Explosion>(out var explosionScript))
            {
                explosionScript.Init(bulletOwner, damage, explosionRadius);
            }
        }
        else if (explosionPrefab != null)
        {
            // Fallback to parent's explosion prefab if specific effect not set
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            if (explosion.TryGetComponent<Explosion>(out var explosionScript))
            {
                explosionScript.Init(bulletOwner, damage, explosionRadius);
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