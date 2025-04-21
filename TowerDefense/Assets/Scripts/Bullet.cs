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

    public AudioClip shotSound;

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

    protected virtual void Update()
    {
        if (shot)
        {
            if (shotSound != null)
            {
                SoundFXManager.Instance.PlaySound(shotSound, transform, 0.5f, 10, 1.0f, 0.8f);
                shotSound = null; // Ensure sound plays only once
            }
            transform.rotation = Quaternion.FromToRotation(Vector3.up, moveDirection); // Align the top (Y-axis) with moveDirection
            transform.Translate(speed * Time.deltaTime * Vector3.up, Space.Self); // Move in the direction of move direction
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
