using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 1f;
    public float maxLifeTime = 0, lifeTime = 0;
    public Vector3 moveDirection;
    public bool shot = false;
    public string bulletOwner = "";
    public GameObject explosionPrefab;
    public bool useParabolicPath = false;
    private Vector3 initialVelocity;
    Rigidbody rb;
    public AudioClip shotSound;


    public void InitWithPower(Vector3 direction, float shotPower, string owner, float damage = -1)
    {
        Init(direction, owner, damage);
        rb.AddForce(direction * shotPower);
    }

    public void InitWithVelocity(Vector3 direction, float velocity, string owner, float damage = -1)
    {
        Init(direction, owner, damage);
        useParabolicPath = true;
        rb.velocity = moveDirection * velocity;
        rb.useGravity = true;
    }

    void Init(Vector3 direction, string owner, float damage)
    {
        rb = GetComponent<Rigidbody>();
        moveDirection = direction.normalized;
        bulletOwner = owner;
        shot = true;
        useParabolicPath = false;
        if (damage != -1)
        {
            this.damage = damage;
        }
    }

    protected virtual void Update()
    {
        if (shot)
        {
            if (shotSound != null)
            {
                SoundFXManager.Instance.PlaySound(shotSound, transform, 0.5f, 10, 1.0f, 0.8f);
                shotSound = null; // Ensure sound plays only once
            }

            if (useParabolicPath)
            {
                transform.rotation = Quaternion.FromToRotation(Vector3.up, rb.velocity.normalized);
            }
            else
            {
                // linear movement
                transform.rotation = Quaternion.FromToRotation(Vector3.up, moveDirection);
                transform.Translate(speed * Time.deltaTime * Vector3.up, Space.Self);
            }

            lifeTime += Time.deltaTime;
            if (lifeTime >= maxLifeTime)
                Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(bulletOwner))
            return;

        if (other.CompareTag("Enemy") && bulletOwner == "Turret")
        {
            Balloon enemy = other.GetComponent<Balloon>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Turret") && bulletOwner == "Enemy")
        {
            Turret turret = other.GetComponent<Turret>();
            if (turret != null)
            {
                turret.TakeDamage(damage);
            }
            Destroy(gameObject);
        }

        else if (other.CompareTag("Plane"))
        {
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }

        // Debug.Log(damage);
    }
}
