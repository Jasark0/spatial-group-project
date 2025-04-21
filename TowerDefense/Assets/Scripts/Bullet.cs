using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 1f;
    public float maxLifeTime = 0, lifeTime = 0;
    public Vector3 moveDirection;
    bool shot = false;
    public string bulletOwner = "";
    public GameObject explosionPrefab;

    public bool isMissile = false;
    public float explosionRadius = 3f;
    public void Init(Vector3 direction, float shotPower, string owner, float damage = -1)
    {
        moveDirection = direction.normalized; // Normalize to ensure consistent movement speed
        GetComponent<Rigidbody>().AddForce(direction * shotPower);
        bulletOwner = owner;
        shot = true;
        if (damage != -1)
        {
            this.damage = damage;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(bulletOwner))
            return;

        if (other.CompareTag("Enemy") && bulletOwner == "Turret")
        {
            if (isMissile)
            {
                ExplodeAndDamageArea();
            }
            else
            {
                Balloon enemy = other.GetComponent<Balloon>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Turret") && bulletOwner == "Enemy")
        {
            if (isMissile)
            {
                ExplodeAndDamageArea();
            }
            else
            {
                Turret turret = other.GetComponent<Turret>();
                if (turret != null)
                {
                    turret.TakeDamage(damage);
                }
            }
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Plane"))
        {
            if (isMissile)
            {
                ExplodeAndDamageArea();
            }
            Destroy(gameObject);
        }
    }

    private void ExplodeAndDamageArea()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider col in hitColliders)
        {
            if (bulletOwner == "Turret" && col.CompareTag("Enemy"))
            {
                Balloon enemy = col.GetComponent<Balloon>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
            else if (bulletOwner == "Enemy" && col.CompareTag("Turret"))
            {
                Turret turret = col.GetComponent<Turret>();
                if (turret != null)
                {
                    turret.TakeDamage(damage);
                }
            }
        }

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
    }
}
