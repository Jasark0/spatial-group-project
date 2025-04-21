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
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Find all colliders within explosion radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider col in hitColliders)
        {
            // Damage enemies if missile is from a turret
            if (bulletOwner == "Turret" && col.CompareTag("Enemy"))
            {
                Balloon enemy = col.GetComponent<Balloon>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
            // Damage turrets if missile is from an enemy
            else if (bulletOwner == "Enemy" && col.CompareTag("Turret"))
            {
                Turret turret = col.GetComponent<Turret>();
                if (turret != null)
                {
                    turret.TakeDamage(damage);
                }
            }
            // Could add player damage here if needed
            else if (bulletOwner == "Enemy" && col.CompareTag("Player"))
            {
                // Player damage logic if needed
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